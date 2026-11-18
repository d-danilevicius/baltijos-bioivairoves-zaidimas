namespace Project.Api.Services;

public class GbifTaskResult
{
    public GbifTaskOutcome Outcome { get; private init; }

    public int CreatedCount { get; private init; }

    public static GbifTaskResult Created(int createdCount) =>
        new() { Outcome = GbifTaskOutcome.Created, CreatedCount = createdCount };

    public static GbifTaskResult Failed(GbifTaskOutcome outcome) => new() { Outcome = outcome };
}
