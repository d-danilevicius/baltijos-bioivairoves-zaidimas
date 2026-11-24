using Microsoft.EntityFrameworkCore;
using Npgsql;
using Project.Api.Data;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;

namespace Project.Api.Services;

public class SubmissionService : ISubmissionService
{
    private const string StudentTaskIndexName = "IX_Submissions_ClassTaskId_StudentId";
    private const string TaskForeignKeyName = "FK_Submissions_ClassTasks_ClassTaskId";

    private readonly ApplicationDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public SubmissionService(ApplicationDbContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<SubmissionResult> CreateAsync(string studentId, Guid taskId, CreateSubmissionRequest request, CancellationToken cancellationToken)
    {
        var (access, task) = await _dbContext.FindTaskForStudentAsync(studentId, taskId, cancellationToken);

        if (task is null)
        {
            return SubmissionResult.Failed(access == StudentTaskDetailsOutcome.Locked ? SubmissionOutcome.Locked : SubmissionOutcome.NotFound);
        }

        // Any second submission is "already submitted", whatever the answer is (VT-015)
        var alreadySubmitted = await _dbContext.Submissions
            .AnyAsync(s => s.ClassTaskId == task.Id && s.StudentId == studentId, cancellationToken);

        if (alreadySubmitted)
        {
            return SubmissionResult.Failed(SubmissionOutcome.AlreadySubmitted);
        }

        // A task with options accepts only one of them; a task without options accepts the typed answer
        if (task.Options is { Length: > 0 } options && !options.Contains(request.Answer))
        {
            return SubmissionResult.Failed(SubmissionOutcome.InvalidAnswer);
        }

        // The points are calculated by the server and saved together with the answer (VT-016, NFR-018)
        var points = task.PointsFor(request.Answer);
        var progress = await _dbContext.LoadStudentProgressAsync(studentId, task.ClassId, cancellationToken);

        return await SaveAsync(studentId, task, request, points, progress.GroupUnlockedBy(task.Id, points), cancellationToken);
    }

    private async Task<SubmissionResult> SaveAsync(
        string studentId,
        ClassTask task,
        CreateSubmissionRequest request,
        int points,
        int? unlockedGroup,
        CancellationToken cancellationToken)
    {
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            ClassTaskId = task.Id,
            StudentId = studentId,
            Answer = request.Answer,
            Evidence = request.Evidence,
            Points = points,
            SubmittedAtUtc = _timeProvider.GetUtcNow().UtcDateTime
        };

        _dbContext.Submissions.Add(submission);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsStudentTaskCollision(exception))
        {
            // Two requests of the same student arrived at the same time; the first one is kept
            return SubmissionResult.Failed(SubmissionOutcome.AlreadySubmitted);
        }
        catch (DbUpdateException exception) when (IsTaskDeletedMeanwhile(exception))
        {
            // The teacher deleted the task after it was read and before the submission was saved
            return SubmissionResult.Failed(SubmissionOutcome.NotFound);
        }

        return SubmissionResult.Created(submission.ToResponse(task, unlockedGroup));
    }

    private static bool IsTaskDeletedMeanwhile(DbUpdateException exception) =>
        exception.GetBaseException() is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation, ConstraintName: TaskForeignKeyName };

    private static bool IsStudentTaskCollision(DbUpdateException exception) =>
        exception.GetBaseException() is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: StudentTaskIndexName };
}
