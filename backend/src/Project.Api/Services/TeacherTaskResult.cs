using Project.Api.DTOs.Responses;

namespace Project.Api.Services;

public class TeacherTaskResult
{
    public TeacherTaskOutcome Outcome { get; private init; }

    public ClassTaskResponse? Task { get; private init; }

    public static TeacherTaskResult Created(ClassTaskResponse task) =>
        new() { Outcome = TeacherTaskOutcome.Created, Task = task };

    public static TeacherTaskResult Deleted() => new() { Outcome = TeacherTaskOutcome.Deleted };

    public static TeacherTaskResult Failed(TeacherTaskOutcome outcome) => new() { Outcome = outcome };
}
