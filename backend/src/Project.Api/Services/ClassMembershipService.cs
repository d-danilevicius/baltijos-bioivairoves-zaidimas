using Microsoft.EntityFrameworkCore;
using Npgsql;
using Project.Api.Data;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;

namespace Project.Api.Services;

public class ClassMembershipService : IClassMembershipService
{
    private const int MaxAttempts = 2;
    private const string ActiveMembershipIndexName = "IX_ClassMemberships_StudentId";

    private readonly ApplicationDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public ClassMembershipService(ApplicationDbContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<JoinClassResult> JoinAsync(string studentId, JoinClassRequest request, CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                return await TryJoinAsync(studentId, request, cancellationToken);
            }
            catch (DbUpdateException ex) when (attempt < MaxAttempts && IsActiveMembershipCollision(ex))
            {
                // Another request of the same student was saved first: forget the changes and evaluate again
                _dbContext.ChangeTracker.Clear();
            }
        }

        throw new InvalidOperationException("Could not join the class.");
    }

    private async Task<JoinClassResult> TryJoinAsync(string studentId, JoinClassRequest request, CancellationToken cancellationToken)
    {
        var newClass = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

        if (newClass is null)
        {
            return JoinClassResult.ClassNotFound();
        }

        var active = await _dbContext.ClassMemberships
            .FirstOrDefaultAsync(m => m.StudentId == studentId && m.EndedAtUtc == null, cancellationToken);

        if (active is null)
        {
            return await StartMembershipAsync(studentId, newClass, cancellationToken);
        }

        if (active.ClassId == newClass.Id)
        {
            return JoinClassResult.AlreadyMember(newClass.Name);
        }

        if (!request.ConfirmChange)
        {
            return await RequireConfirmationAsync(active, newClass, cancellationToken);
        }

        active.EndedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
        return await StartMembershipAsync(studentId, newClass, cancellationToken);
    }

    private async Task<JoinClassResult> StartMembershipAsync(string studentId, SchoolClass newClass, CancellationToken cancellationToken)
    {
        _dbContext.ClassMemberships.Add(new ClassMembership
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ClassId = newClass.Id,
            StartedAtUtc = _timeProvider.GetUtcNow().UtcDateTime
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        return JoinClassResult.Joined(newClass.Name);
    }

    private async Task<JoinClassResult> RequireConfirmationAsync(ClassMembership active, SchoolClass newClass, CancellationToken cancellationToken)
    {
        var currentClassName = await _dbContext.Classes
            .Where(c => c.Id == active.ClassId)
            .Select(c => c.Name)
            .FirstAsync(cancellationToken);

        return JoinClassResult.ConfirmationRequired(newClass.Name, currentClassName);
    }

    private static bool IsActiveMembershipCollision(DbUpdateException exception) =>
        exception.GetBaseException() is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: ActiveMembershipIndexName };
}
