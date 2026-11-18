using Project.Api.Entities;
using Project.Api.Integrations.Gbif;

namespace Project.IntegrationTests.Gbif;

// Replaces the real GBIF client in the integration tests; the tests never call the real GBIF
public sealed class FakeGbifClient : IGbifClient
{
    private const double Latitude = 55.0;
    private const double Longitude = 24.0;

    private readonly Dictionary<Season, IReadOnlyList<GbifOccurrence>> _bySeason = new();

    public bool Fails { get; set; }

    // Makes the answer slow, so two requests at the same time overlap
    public TimeSpan Delay { get; set; }

    public void Reset()
    {
        _bySeason.Clear();
        Fails = false;
        Delay = TimeSpan.Zero;
    }

    // 12 summer and 10 winter observations at the point (22 stored observations)
    public void UseEnoughObservations()
    {
        _bySeason[Season.Summer] = Create("Parus major", "Aves", 4, 6, 100)
            .Concat(Create("Vulpes vulpes", "Mammalia", 3, 7, 200))
            .Concat(Create("Apis mellifera", "Insecta", 3, 8, 300))
            .Concat(Create("Rana temporaria", "Amphibia", 2, 6, 350)).ToList();
        _bySeason[Season.Winter] = Create("Parus major", "Aves", 3, 12, 400)
            .Concat(Create("Vulpes vulpes", "Mammalia", 6, 1, 500))
            .Concat(Create("Apis mellifera", "Insecta", 1, 2, 600)).ToList();
    }

    public void UseObservations(Season season, int count) =>
        _bySeason[season] = Create("Parus major", "Aves", count, 6, 700);

    public async Task<IReadOnlyList<GbifOccurrence>> SearchAsync(
        double latitude,
        double longitude,
        double radiusKm,
        Season season,
        CancellationToken cancellationToken)
    {
        if (Delay > TimeSpan.Zero)
        {
            await Task.Delay(Delay, cancellationToken);
        }

        if (Fails)
        {
            throw new GbifUnavailableException("GBIF is not available.");
        }

        return _bySeason.TryGetValue(season, out var occurrences) ? occurrences : Array.Empty<GbifOccurrence>();
    }

    private static List<GbifOccurrence> Create(string species, string gbifClass, int count, int month, long firstKey) =>
        Enumerable.Range(0, count)
            .Select(i => new GbifOccurrence(firstKey + i, species, gbifClass, "Family", Latitude, Longitude, "Place", new DateOnly(2025, month, 10), "Dataset", "CC0_1_0", null))
            .ToList();
}
