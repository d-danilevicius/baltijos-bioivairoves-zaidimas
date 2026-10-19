namespace Project.Api.Entities;

public class SchoolClass
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string OwnerId { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }
}
