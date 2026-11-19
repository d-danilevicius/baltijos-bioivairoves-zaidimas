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
        // The current class is the membership that has not ended (VT-026: no class, no tasks)
        var classId = await _dbContext.ClassMemberships
            .AsNoTracking()
            .Where(m => m.StudentId == studentId && m.EndedAtUtc == null)
            .Select(m => (Guid?)m.ClassId)
            .FirstOrDefaultAsync(cancellationToken);

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
            .ToListAsync(cancellationToken);

        var groups = BuildGroups(tasks);

        return new StudentTaskListResponse
        {
            HasClass = true,
            Groups = groups,
            ContinueTaskId = FindTaskToContinue(groups)
        };
    }

    private static List<StudentTaskGroupResponse> BuildGroups(List<ClassTask> tasks)
    {
        var groupsWithTasks = tasks.Select(t => t.DifficultyGroup).Distinct().ToList();

        return tasks
            .GroupBy(t => t.DifficultyGroup)
            .Select(group =>
            {
                var isUnlocked = TaskGroupUnlocking.IsUnlocked(group.Key, groupsWithTasks);
                return new StudentTaskGroupResponse
                {
                    Group = group.Key,
                    IsUnlocked = isUnlocked,
                    Tasks = group.Select(t => ToStudentTask(t, isUnlocked)).ToList()
                };
            })
            .ToList();
    }

    private static StudentTaskResponse ToStudentTask(ClassTask task, bool isUnlocked) => new()
    {
        Id = task.Id,
        Type = task.Type,
        Label = task.ToLabel(),
        Status = isUnlocked ? StudentTaskStatus.NotDone : StudentTaskStatus.Locked
    };

    // Until feature 014 adds the status "done", the first task of the first unlocked group is the one to continue from (FR-049)
    private static Guid? FindTaskToContinue(List<StudentTaskGroupResponse> groups) => groups
        .Where(g => g.IsUnlocked)
        .SelectMany(g => g.Tasks)
        .Select(t => (Guid?)t.Id)
        .FirstOrDefault();
}
