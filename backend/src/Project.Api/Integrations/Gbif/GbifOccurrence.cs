namespace Project.Api.Integrations.Gbif;

// One occurrence (observation) as returned by the GBIF Occurrence API; any field can be missing
public record GbifOccurrence(
    long Key,
    string? Species,
    string? GbifClass,
    string? Family,
    double? Latitude,
    double? Longitude,
    string? Locality,
    DateOnly? EventDate,
    string? DatasetName,
    string? License,
    string? IucnCategory);
