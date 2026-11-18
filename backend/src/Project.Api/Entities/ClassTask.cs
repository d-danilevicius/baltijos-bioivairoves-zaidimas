namespace Project.Api.Entities;

public class ClassTask
{
    public Guid Id { get; set; }

    public Guid ClassId { get; set; }

    public TaskType Type { get; set; }

    // 1–3 (VT-013)
    public int DifficultyGroup { get; set; }

    public required string Question { get; set; }

    public required string CorrectAnswer { get; set; }

    // Answer options of a choice task; empty for other tasks
    public string[]? Options { get; set; }

    public Guid? ObservationId { get; set; }
}
