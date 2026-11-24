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

        var progress = await dbContext.LoadStudentProgressAsync(studentId, task.ClassId, cancellationToken);

        return progress.IsUnlocked(task.DifficultyGroup)
            ? (StudentTaskDetailsOutcome.Found, task)
            : (StudentTaskDetailsOutcome.Locked, null);
    }

    // The points of the student by task; the tasks of other classes are dropped later by StudentProgress
    public static async Task<Dictionary<Guid, int>> LoadPointsByTaskAsync(
        this ApplicationDbContext dbContext,
        string studentId,
        CancellationToken cancellationToken)
    {
        var submissions = await dbContext.Submissions
            .AsNoTracking()
            .Where(s => s.StudentId == studentId && s.ClassTaskId != null)
            .Select(s => new { TaskId = s.ClassTaskId!.Value, s.Points })
            .ToListAsync(cancellationToken);

        return submissions.ToDictionary(s => s.TaskId, s => s.Points);
    }

    public static async Task<StudentProgress> LoadStudentProgressAsync(
        this ApplicationDbContext dbContext,
        string studentId,
        Guid classId,
        CancellationToken cancellationToken)
    {
        var tasks = await dbContext.ClassTasks
            .AsNoTracking()
            .Where(t => t.ClassId == classId)
            .Select(t => new { t.Id, t.DifficultyGroup })
            .ToListAsync(cancellationToken);

        var points = await dbContext.LoadPointsByTaskAsync(studentId, cancellationToken);

        return StudentProgress.Create(tasks.Select(t => (t.Id, t.DifficultyGroup)), points);
    }
}
