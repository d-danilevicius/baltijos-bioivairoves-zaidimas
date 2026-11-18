namespace Project.Api.Services;

public interface IClassTaskService
{
    Task<ClassTasksResult> ListAsync(string ownerId, Guid classId, CancellationToken cancellationToken);
}
