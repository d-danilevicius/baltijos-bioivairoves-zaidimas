namespace Project.Api.Entities;

public class ClassTask
{
    public Guid Id { get; set; }

    public Guid ClassId { get; set; }

    public TaskType Type { get; set; }

    // 1–3 (VT-013)
    public int DifficultyGroup { get; set; }

    // Used by teacher tasks only (VT-018)
    public string? Title { get; set; }

    public TaskSource Source { get; set; } = TaskSource.Gbif;

    public required string Question { get; set; }

    public required string CorrectAnswer { get; set; }

    // Answer options of a choice task; empty for other tasks
    public string[]? Options { get; set; }

    public Guid? ObservationId { get; set; }

    // The only place of the points of a difficulty group (VT-016)
    public static int PointsOfGroup(int group) => 10 * group;

    // Full points of the group for a correct answer, otherwise 0; letter case and outer spaces are ignored (VT-018)
    public int PointsFor(string answer) =>
        string.Equals(answer.Trim(), CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase)
            ? PointsOfGroup(DifficultyGroup)
            : 0;
}
