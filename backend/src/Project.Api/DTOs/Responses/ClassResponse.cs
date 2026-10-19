namespace Project.Api.DTOs.Responses;

public class ClassResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
}
