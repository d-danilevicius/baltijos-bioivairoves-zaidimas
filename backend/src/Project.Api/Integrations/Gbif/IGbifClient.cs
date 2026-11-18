using Project.Api.Entities;

namespace Project.Api.Integrations.Gbif;

public interface IGbifClient
{
    // Occurrences with coordinates in the months of the season, inside the square around the point
    Task<IReadOnlyList<GbifOccurrence>> SearchAsync(
        double latitude,
        double longitude,
        double radiusKm,
        Season season,
        CancellationToken cancellationToken);
}
