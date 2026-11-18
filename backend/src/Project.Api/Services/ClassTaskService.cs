using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.DTOs.Responses;

namespace Project.Api.Services;

public class ClassTaskService : IClassTaskService
{
    // The only tasks that exist now come from GBIF; the later features add other labels
    private const string GbifLabel = "GBIF";

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

        return ClassTasksResult.Succeeded(tasks.Select(t => new ClassTaskResponse
        {
            Id = t.Id,
            Type = t.Type,
            DifficultyGroup = t.DifficultyGroup,
            Label = GbifLabel
        }).ToList());
    }
}
