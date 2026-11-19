using Project.Api.DTOs.Responses;

namespace Project.Api.Services;

public class StudentTaskDetailsResult
{
    public StudentTaskDetailsOutcome Outcome { get; private init; }

    public StudentTaskDetailsResponse? Task { get; private init; }

    public static StudentTaskDetailsResult Found(StudentTaskDetailsResponse task) =>
        new() { Outcome = StudentTaskDetailsOutcome.Found, Task = task };

    public static StudentTaskDetailsResult NotFound() => new() { Outcome = StudentTaskDetailsOutcome.NotFound };

    public static StudentTaskDetailsResult Locked() => new() { Outcome = StudentTaskDetailsOutcome.Locked };
}
