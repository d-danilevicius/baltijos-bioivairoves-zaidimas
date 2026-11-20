using Project.Api.DTOs.Responses;

namespace Project.Api.Services;

public class SubmissionResult
{
    public SubmissionOutcome Outcome { get; private init; }

    public SubmissionResponse? Submission { get; private init; }

    public static SubmissionResult Created(SubmissionResponse submission) =>
        new() { Outcome = SubmissionOutcome.Created, Submission = submission };

    public static SubmissionResult Failed(SubmissionOutcome outcome) => new() { Outcome = outcome };
}
