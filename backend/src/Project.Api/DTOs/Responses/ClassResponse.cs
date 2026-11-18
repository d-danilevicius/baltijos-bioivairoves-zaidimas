namespace Project.Api.DTOs.Responses;

public class ClassResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
