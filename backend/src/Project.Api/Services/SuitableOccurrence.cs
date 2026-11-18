using Project.Api.Entities;

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
    string? IucnCategory)
{
    // NFR-021: the copy of the observation stored for the class
    public Observation ToObservation(Guid classId, Season season) => new()
    {
        Id = Guid.NewGuid(),
        ClassId = classId,
        GbifKey = Key,
        Species = Species,
        GbifClass = GbifClass,
        Family = Family,
        Latitude = Latitude,
        Longitude = Longitude,
        Locality = Locality,
        ObservedOn = EventDate,
        DatasetName = DatasetName,
        License = License,
        IucnCategory = IucnCategory,
        Season = season
    };
}
