namespace Project.Api.Entities;

// The answer and the evidence of a student for a task (VT-015); a student submits a task only once
public class Submission
{
    public Guid Id { get; set; }

    // Empty after the task is deleted, so the student's data stay (VT-018)
    public Guid? ClassTaskId { get; set; }

    public required string StudentId { get; set; }

    public required string Answer { get; set; }

    public required string Evidence { get; set; }

    public DateTime SubmittedAtUtc { get; set; }
}
