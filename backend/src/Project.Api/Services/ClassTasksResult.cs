using Project.Api.DTOs.Responses;

namespace Project.Api.Services;

public class ClassTasksResult
{
    public ClassOutcome Outcome { get; private init; }

    public IReadOnlyList<ClassTaskResponse> Tasks { get; private init; } = Array.Empty<ClassTaskResponse>();

    public static ClassTasksResult Succeeded(IReadOnlyList<ClassTaskResponse> tasks) =>
        new() { Outcome = ClassOutcome.Succeeded, Tasks = tasks };

    public static ClassTasksResult NotFound() => new() { Outcome = ClassOutcome.NotFound };

    public static ClassTasksResult Forbidden() => new() { Outcome = ClassOutcome.Forbidden };
}
