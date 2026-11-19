using Project.Api.DTOs.Responses;

namespace Project.Api.Services;

public interface IStudentTaskService
{
    Task<StudentTaskListResponse> GetAsync(string studentId, CancellationToken cancellationToken);
}
