using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.Entities;

namespace Project.Api.Services;

// The one place that decides whether a student may open or submit a task: the task must belong to the
// student's current class and its difficulty group must be unlocked (VT-014, VT-026)
public static class StudentTaskAccess
{
    public static async Task<(StudentTaskDetailsOutcome Outcome, ClassTask? Task)> FindTaskForStudentAsync(
        this ApplicationDbContext dbContext,
        string studentId,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var classId = await dbContext.FindCurrentClassIdAsync(studentId, cancellationToken);

        // A task of another class is "not found", so the existence of other classes' tasks is not revealed
        var task = classId is null
            ? null
            : await dbContext.ClassTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId && t.ClassId == classId, cancellationToken);

        if (task is null)
        {
            return (StudentTaskDetailsOutcome.NotFound, null);
        }

        var groupsWithTasks = await dbContext.ClassTasks
            .Where(t => t.ClassId == classId)
            .Select(t => t.DifficultyGroup)
            .Distinct()
            .ToListAsync(cancellationToken);

        return TaskGroupUnlocking.IsUnlocked(task.DifficultyGroup, groupsWithTasks)
            ? (StudentTaskDetailsOutcome.Found, task)
            : (StudentTaskDetailsOutcome.Locked, null);
    }
}
