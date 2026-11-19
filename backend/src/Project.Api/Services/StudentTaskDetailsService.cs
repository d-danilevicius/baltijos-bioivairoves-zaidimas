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
        var (outcome, task) = await _dbContext.FindTaskForStudentAsync(studentId, taskId, cancellationToken);

        if (task is null)
        {
            return outcome == StudentTaskDetailsOutcome.Locked
                ? StudentTaskDetailsResult.Locked()
                : StudentTaskDetailsResult.NotFound();
        }

        // A teacher task has no observation
        var observation = task.ObservationId is null
            ? null
            : await _dbContext.Observations
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == task.ObservationId, cancellationToken);

        var submission = await _dbContext.Submissions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ClassTaskId == task.Id && s.StudentId == studentId, cancellationToken);

        var details = task.ToDetails(observation);
        details.Submission = submission?.ToResponse(task.Id);

        return StudentTaskDetailsResult.Found(details);
    }
}
