using Microsoft.EntityFrameworkCore;
using Project.Api.Data;

namespace Project.Api.Services;

public static class CurrentClassLookup
{
    // The current class is the membership that has not ended (VT-026: no class, no tasks)
    public static Task<Guid?> FindCurrentClassIdAsync(
        this ApplicationDbContext dbContext,
        string studentId,
        CancellationToken cancellationToken) =>
        dbContext.ClassMemberships
            .AsNoTracking()
            .Where(m => m.StudentId == studentId && m.EndedAtUtc == null)
            .Select(m => (Guid?)m.ClassId)
            .FirstOrDefaultAsync(cancellationToken);
}
