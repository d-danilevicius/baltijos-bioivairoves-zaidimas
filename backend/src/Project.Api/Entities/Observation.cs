namespace Project.Api.Entities;

// A stored copy of a GBIF observation used for the tasks of a class (NFR-021)
public class Observation
{
    public Guid Id { get; set; }

    public Guid ClassId { get; set; }

    public long GbifKey { get; set; }

    public required string Species { get; set; }

    public string? GbifClass { get; set; }

    public string? Family { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Locality { get; set; }

    public DateOnly? ObservedOn { get; set; }

    public string? DatasetName { get; set; }

    public string? License { get; set; }

    // IUCN Red List category given by GBIF; VU, EN and CR mean a protected species (VT-020)
    public string? IucnCategory { get; set; }

    public Season Season { get; set; }
}
