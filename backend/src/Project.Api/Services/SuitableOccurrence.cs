namespace Project.Api.Services;

// A GBIF occurrence that has a species name and coordinates (VT-011)
public record SuitableOccurrence(
    long Key,
    string Species,
    double Latitude,
    double Longitude,
    string? GbifClass,
    string? Family,
    string? Locality,
    DateOnly? EventDate,
    string? DatasetName,
    string? License,
    string? IucnCategory);
