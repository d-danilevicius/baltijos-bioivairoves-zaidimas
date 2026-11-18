using Project.Api.Entities;
using Project.Api.Integrations.Gbif;

namespace Project.UnitTests.Gbif;

internal sealed class FakeGbifClient : IGbifClient
{
    private readonly Dictionary<Season, IReadOnlyList<GbifOccurrence>> _bySeason = new();

    public int Calls { get; private set; }

    public bool Fails { get; set; }

    // Runs while GBIF "answers"; used to simulate another request that finishes first
    public Func<Task>? OnSearch { get; set; }

    public FakeGbifClient With(Season season, IEnumerable<GbifOccurrence> occurrences)
    {
        _bySeason[season] = occurrences.ToList();
        return this;
    }

    public async Task<IReadOnlyList<GbifOccurrence>> SearchAsync(
        double latitude,
        double longitude,
        double radiusKm,
        Season season,
        CancellationToken cancellationToken)
    {
        Calls++;
        if (OnSearch is not null)
        {
            await OnSearch();
        }

        if (Fails)
        {
            throw new GbifUnavailableException("GBIF is not available.");
        }

        return _bySeason.TryGetValue(season, out var occurrences) ? occurrences : Array.Empty<GbifOccurrence>();
    }
}
