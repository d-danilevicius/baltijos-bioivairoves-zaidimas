using Project.Api.DTOs.Responses;
using Project.Api.Entities;

namespace Project.Api.Services;

public static class SubmissionExtensions
{
    private const string SubmittedLabel = "Mokinio pateikti duomenys";

    public static SubmissionResponse ToResponse(this Submission submission, Guid taskId) => new()
    {
        Id = submission.Id,
        TaskId = taskId,
        Answer = submission.Answer,
        Evidence = submission.Evidence,
        Label = SubmittedLabel,
        SubmittedAtUtc = submission.SubmittedAtUtc
    };
}
