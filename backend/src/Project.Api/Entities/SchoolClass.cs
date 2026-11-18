namespace Project.Api.Entities;

public class SchoolClass
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Code { get; set; }

    public required string OwnerId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    // Empty until the GBIF tasks of the class are created (VT-010)
    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public Season? Season { get; set; }
}
