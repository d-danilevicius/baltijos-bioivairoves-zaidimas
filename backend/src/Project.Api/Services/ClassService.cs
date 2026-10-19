using Microsoft.EntityFrameworkCore;
using Npgsql;
using Project.Api.Data;
using Project.Api.DTOs.Requests;
using Project.Api.DTOs.Responses;
using Project.Api.Entities;

namespace Project.Api.Services;

public class ClassService : IClassService
{
    private const int MaxCodeAttempts = 10;
    private const string CodeIndexName = "IX_Classes_Code";

    private readonly ApplicationDbContext _dbContext;
    private readonly IClassCodeGenerator _codeGenerator;
    private readonly TimeProvider _timeProvider;

    public ClassService(ApplicationDbContext dbContext, IClassCodeGenerator codeGenerator, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _codeGenerator = codeGenerator;
        _timeProvider = timeProvider;
    }

    public async Task<ClassResponse> CreateAsync(string ownerId, CreateClassRequest request, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxCodeAttempts; attempt++)
        {
            var schoolClass = new SchoolClass
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = _codeGenerator.Generate(),
                OwnerId = ownerId,
                CreatedAtUtc = _timeProvider.GetUtcNow().UtcDateTime
            };

            _dbContext.Classes.Add(schoolClass);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                return ToResponse(schoolClass);
            }
            catch (DbUpdateException ex) when (IsCodeCollision(ex))
            {
                // The unique index rejected the code: forget this entity and try another code
                _dbContext.Entry(schoolClass).State = EntityState.Detached;
            }
        }

        throw new InvalidOperationException("Could not generate a unique class code.");
    }

    public async Task<IReadOnlyList<ClassResponse>> ListAsync(string ownerId, CancellationToken cancellationToken)
    {
        var classes = await _dbContext.Classes
            .Where(c => c.OwnerId == ownerId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return classes.Select(ToResponse).ToList();
    }

    public async Task<ClassResult> GetAsync(string ownerId, Guid id, CancellationToken cancellationToken)
    {
        var schoolClass = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (schoolClass is null)
        {
            return ClassResult.NotFound();
        }

        return schoolClass.OwnerId == ownerId
            ? ClassResult.Succeeded(ToResponse(schoolClass))
            : ClassResult.Forbidden();
    }

    public async Task<ClassResult> DeleteAsync(string ownerId, Guid id, CancellationToken cancellationToken)
    {
        var schoolClass = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (schoolClass is null)
        {
            return ClassResult.NotFound();
        }

        if (schoolClass.OwnerId != ownerId)
        {
            return ClassResult.Forbidden();
        }

        _dbContext.Classes.Remove(schoolClass);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another request deleted the class between the load and the save
            return ClassResult.NotFound();
        }

        return ClassResult.Succeeded();
    }

    private static bool IsCodeCollision(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: CodeIndexName };

    private static ClassResponse ToResponse(SchoolClass schoolClass) => new()
    {
        Id = schoolClass.Id,
        Name = schoolClass.Name,
        Code = schoolClass.Code,
        CreatedAtUtc = schoolClass.CreatedAtUtc
    };
}
