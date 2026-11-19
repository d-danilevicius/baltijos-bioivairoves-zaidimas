using Microsoft.EntityFrameworkCore;
using Project.Api.Data;

namespace Project.Api.Services;

public class ClassTaskService : IClassTaskService
{
    private readonly ApplicationDbContext _dbContext;

    public ClassTaskService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClassTasksResult> ListAsync(string ownerId, Guid classId, CancellationToken cancellationToken)
    {
        var schoolClass = await _dbContext.Classes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == classId, cancellationToken);
        if (schoolClass is null)
        {
            return ClassTasksResult.NotFound();
        }

        if (schoolClass.OwnerId != ownerId)
        {
            return ClassTasksResult.Forbidden();
        }

        var tasks = await _dbContext.ClassTasks
            .AsNoTracking()
            .Where(t => t.ClassId == classId)
            .OrderBy(t => t.DifficultyGroup)
            .ThenBy(t => t.Type)
            .ThenBy(t => t.Question)
            .ToListAsync(cancellationToken);

        return ClassTasksResult.Succeeded(tasks.Select(t => t.ToResponse()).ToList());
    }
}
