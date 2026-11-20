using Project.Api.DTOs.Requests;

namespace Project.Api.Services;

public interface ISubmissionService
{
    Task<SubmissionResult> CreateAsync(string studentId, Guid taskId, CreateSubmissionRequest request, CancellationToken cancellationToken);
}
