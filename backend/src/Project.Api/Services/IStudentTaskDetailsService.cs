namespace Project.Api.Services;

public interface IStudentTaskDetailsService
{
    Task<StudentTaskDetailsResult> GetAsync(string studentId, Guid taskId, CancellationToken cancellationToken);
}
