using Project.Api.Entities;

namespace Project.Api.DTOs.Responses;

public class ClassResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Season? Season { get; set; }
}
