using Project.Api.DTOs.Requests;

namespace Project.Api.Services;

public interface ITeacherTaskService
{
    Task<TeacherTaskResult> CreateAsync(string ownerId, Guid classId, CreateTeacherTaskRequest request, CancellationToken cancellationToken);

    Task<TeacherTaskResult> DeleteAsync(string ownerId, Guid classId, Guid taskId, CancellationToken cancellationToken);
}
