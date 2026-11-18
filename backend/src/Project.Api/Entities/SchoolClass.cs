namespace Project.Api.Entities;

public class SchoolClass
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Code { get; set; }

    public required string OwnerId { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
