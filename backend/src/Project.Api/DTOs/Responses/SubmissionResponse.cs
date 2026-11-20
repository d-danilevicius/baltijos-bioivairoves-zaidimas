namespace Project.Api.DTOs.Responses;

// The data a student submitted; the label separates them from confirmed external observations (FR-006)
public class SubmissionResponse
{
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }

    public required string Answer { get; set; }

    public required string Evidence { get; set; }

    public required string Label { get; set; }

    public DateTime SubmittedAtUtc { get; set; }
}
