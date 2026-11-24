using Project.Api.Entities;

namespace Project.Api.DTOs.Responses;

// One task as the student sees it; the correct answer is not sent before the student submits the task,
// after that it is only inside the student's own submission
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
