using Project.Api.DTOs.Responses;
using Project.Api.Entities;

namespace Project.Api.Services;

public static class SubmissionExtensions
{
    private const string SubmittedLabel = "Mokinio pateikti duomenys";

    // The correct answer is part of the student's own submission only; it is never sent before the submission exists
    public static SubmissionResponse ToResponse(this Submission submission, ClassTask task, int? unlockedGroup = null) => new()
    {
        Id = submission.Id,
        TaskId = task.Id,
        Answer = submission.Answer,
        Evidence = submission.Evidence,
        Label = SubmittedLabel,
        SubmittedAtUtc = submission.SubmittedAtUtc,
        Points = submission.Points,
        CorrectAnswer = task.CorrectAnswer,
        UnlockedGroup = unlockedGroup
    };
}
