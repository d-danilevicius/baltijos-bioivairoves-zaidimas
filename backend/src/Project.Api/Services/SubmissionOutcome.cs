namespace Project.Api.Services;

public enum SubmissionOutcome
{
    Created,
    NotFound,
    Locked,
    InvalidAnswer,
    AlreadySubmitted
}
