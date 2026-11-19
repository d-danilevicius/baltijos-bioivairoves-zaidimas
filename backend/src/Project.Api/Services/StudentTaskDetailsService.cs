using Microsoft.EntityFrameworkCore;
using Project.Api.Data;

namespace Project.Api.Services;

public class StudentTaskDetailsService : IStudentTaskDetailsService
{
    private readonly ApplicationDbContext _dbContext;

    public StudentTaskDetailsService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StudentTaskDetailsResult> GetAsync(string studentId, Guid taskId, CancellationToken cancellationToken)
    {
        var classId = await _dbContext.FindCurrentClassIdAsync(studentId, cancellationToken);

        // A task of another class is "not found", so the existence of other classes' tasks is not revealed
        var task = classId is null
            ? null
            : await _dbContext.ClassTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId && t.ClassId == classId, cancellationToken);

        if (task is null)
        {
            return StudentTaskDetailsResult.NotFound();
        }

        var groupsWithTasks = await _dbContext.ClassTasks
            .Where(t => t.ClassId == classId)
            .Select(t => t.DifficultyGroup)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (!TaskGroupUnlocking.IsUnlocked(task.DifficultyGroup, groupsWithTasks))
        {
            return StudentTaskDetailsResult.Locked();
        }

        var observation = await _dbContext.Observations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == task.ObservationId, cancellationToken);

        return StudentTaskDetailsResult.Found(task.ToDetails(observation));
    }
}
