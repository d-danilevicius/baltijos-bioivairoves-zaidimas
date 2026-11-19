using Project.Api.Entities;

namespace Project.Api.DTOs.Responses;

// One task as the student sees it; it never contains the correct answer
public class StudentTaskDetailsResponse
{
    public Guid Id { get; set; }

    public TaskType Type { get; set; }

    public int DifficultyGroup { get; set; }

    public required string Label { get; set; }

    // Used by teacher tasks only
    public string? Title { get; set; }

    public required string Question { get; set; }

    public required IReadOnlyList<string> Options { get; set; }

    // Empty for a teacher task
    public ObservationCardResponse? Observation { get; set; }

    // Empty until the student submits the task
    public SubmissionResponse? Submission { get; set; }
}
