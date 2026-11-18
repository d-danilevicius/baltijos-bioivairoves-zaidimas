using Project.Api.Entities;

namespace Project.Api.Services;

public interface IGbifTaskService
{
    Task<GbifTaskResult> CreateAsync(
        string ownerId,
        Guid classId,
        double latitude,
        double longitude,
        Season season,
        CancellationToken cancellationToken);
}
