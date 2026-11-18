namespace Project.Api.Services;

public enum GbifTaskOutcome
{
    Created,
    NotFound,
    Forbidden,
    AlreadyCreated,
    TooFewObservations,
    GbifUnavailable
}
