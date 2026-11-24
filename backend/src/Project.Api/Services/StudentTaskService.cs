using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.DTOs.Responses;
using Project.Api.Entities;

namespace Project.Api.Services;

public class StudentTaskService : IStudentTaskService
{
    private readonly ApplicationDbContext _dbContext;

    public StudentTaskService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StudentTaskListResponse> GetAsync(string studentId, CancellationToken cancellationToken)
    {
        var classId = await _dbContext.FindCurrentClassIdAsync(studentId, cancellationToken);

        if (classId is null)
        {
            return new StudentTaskListResponse { HasClass = false, Groups = [] };
        }

        var tasks = await _dbContext.ClassTasks
            .AsNoTracking()
            .Where(t => t.ClassId == classId)
            .OrderBy(t => t.DifficultyGroup)
            .ThenBy(t => t.Type)
            .ThenBy(t => t.Question)
            .ThenBy(t => t.Id)
            .ToListAsync(cancellationToken);

        var pointsByTask = await _dbContext.LoadPointsByTaskAsync(studentId, cancellationToken);
        var progress = StudentProgress.Create(tasks.Select(t => (t.Id, t.DifficultyGroup)), pointsByTask);

        var groups = BuildGroups(tasks, pointsByTask, progress);

        return new StudentTaskListResponse
        {
            HasClass = true,
            Groups = groups,
            ContinueTaskId = FindTaskToContinue(groups)
        };
    }

    private static List<StudentTaskGroupResponse> BuildGroups(
        List<ClassTask> tasks,
        Dictionary<Guid, int> pointsByTask,
        StudentProgress progress)
    {
        return tasks
            .GroupBy(t => t.DifficultyGroup)
            .Select(group =>
            {
                var isUnlocked = progress.IsUnlocked(group.Key);
                return new StudentTaskGroupResponse
                {
                    Group = group.Key,
                    IsUnlocked = isUnlocked,
                    Points = progress.Points(group.Key),
                    MaxPoints = progress.MaxPoints(group.Key),
                    Tasks = group.Select(t => ToStudentTask(t, isUnlocked, pointsByTask)).ToList()
                };
            })
            .ToList();
    }

    private static StudentTaskResponse ToStudentTask(ClassTask task, bool isUnlocked, Dictionary<Guid, int> pointsByTask)
    {
        int? points = pointsByTask.TryGetValue(task.Id, out var earned) ? earned : null;

        return new StudentTaskResponse
        {
            Id = task.Id,
            Type = task.Type,
            Label = task.ToLabel(),
            Status = !isUnlocked ? StudentTaskStatus.Locked : points is null ? StudentTaskStatus.NotDone : StudentTaskStatus.Done,
            Points = points
        };
    }

    // The first task that is not done in an unlocked group is the one to continue from (FR-049)
    private static Guid? FindTaskToContinue(List<StudentTaskGroupResponse> groups) => groups
        .Where(g => g.IsUnlocked)
        .SelectMany(g => g.Tasks)
        .Where(t => t.Status == StudentTaskStatus.NotDone)
        .Select(t => (Guid?)t.Id)
        .FirstOrDefault();
}
