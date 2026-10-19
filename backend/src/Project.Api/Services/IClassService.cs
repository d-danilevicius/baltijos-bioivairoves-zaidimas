using Project.Api.DTOs.Requests;
using Project.Api.DTOs.Responses;

namespace Project.Api.Services;

public interface IClassService
{
    Task<ClassResponse> CreateAsync(string ownerId, CreateClassRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassResponse>> ListAsync(string ownerId, CancellationToken cancellationToken);

    Task<ClassResult> GetAsync(string ownerId, Guid id, CancellationToken cancellationToken);

    Task<ClassResult> DeleteAsync(string ownerId, Guid id, CancellationToken cancellationToken);
}
