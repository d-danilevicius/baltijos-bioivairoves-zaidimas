using Project.Api.Integrations.Gbif;

namespace Project.Api.Services;

public static class GbifObservationSelector
{
    // VT-011: observations farther than this from the chosen point are not used
    public const double MaxDistanceKm = 10;

    // VT-011: tasks are created only if at least this many suitable observations are found
    public const int MinObservations = 10;

    public static IReadOnlyList<SuitableOccurrence> Select(IEnumerable<GbifOccurrence> occurrences, double latitude, double longitude)
    {
        return occurrences
            .Select(ToSuitable)
            .OfType<SuitableOccurrence>()
            .Where(o => GeoDistance.InKilometres(latitude, longitude, o.Latitude, o.Longitude) <= MaxDistanceKm)
            .DistinctBy(o => o.Key)
            .ToList();
    }

    private static SuitableOccurrence? ToSuitable(GbifOccurrence occurrence)
    {
        var species = occurrence.Species?.Trim();
        if (string.IsNullOrEmpty(species) || occurrence.Latitude is null || occurrence.Longitude is null)
        {
            return null;
        }

        return new SuitableOccurrence(
            occurrence.Key,
            species,
            occurrence.Latitude.Value,
            occurrence.Longitude.Value,
            occurrence.GbifClass,
            occurrence.Family,
            occurrence.Locality,
            occurrence.EventDate,
            occurrence.DatasetName,
            occurrence.License,
            occurrence.IucnCategory);
    }
}
