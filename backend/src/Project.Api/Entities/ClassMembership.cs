namespace Project.Api.Entities;

public class ClassMembership
{
    public Guid Id { get; set; }

    public required string StudentId { get; set; }

    public Guid ClassId { get; set; }

    public DateTime StartedAtUtc { get; set; }

    // Empty while the student belongs to the class
    public DateTime? EndedAtUtc { get; set; }
}
