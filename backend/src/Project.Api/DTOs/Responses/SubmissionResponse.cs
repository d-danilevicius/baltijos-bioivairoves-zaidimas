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

    // 0 or the points of the difficulty group of the task (VT-016)
    public int Points { get; set; }

    // Sent only together with the student's own submission, never before it
    public required string CorrectAnswer { get; set; }

    // The number of the group that this submission unlocked; set only in the answer to the submission itself
    public int? UnlockedGroup { get; set; }
}
