using Project.Api.Entities;

namespace Project.UnitTests.Gbif;

internal static class TestObservations
{
    public static readonly Guid ClassId = Guid.NewGuid();

    public static Observation Create(string species, string? gbifClass = "Aves", int? month = 6, Season season = Season.Summer) => new()
    {
        Id = Guid.NewGuid(),
        ClassId = ClassId,
        GbifKey = Random.Shared.NextInt64(),
        Species = species,
        GbifClass = gbifClass,
        Latitude = 55.0,
        Longitude = 24.0,
        ObservedOn = month is null ? null : new DateOnly(2025, month.Value, 15),
        Season = season
    };

    public static List<Observation> Many(string species, int count, string? gbifClass = "Aves", int? month = 6, Season season = Season.Summer) =>
        Enumerable.Range(0, count).Select(_ => Create(species, gbifClass, month, season)).ToList();
}
