using Project.Api.DTOs.Responses;

namespace Project.Api.Services;

public class ClassResult
{
    public ClassOutcome Outcome { get; private init; }

    public ClassResponse? Class { get; private init; }

    public static ClassResult Succeeded(ClassResponse? schoolClass = null) =>
        new() { Outcome = ClassOutcome.Succeeded, Class = schoolClass };

    public static ClassResult NotFound() => new() { Outcome = ClassOutcome.NotFound };

    public static ClassResult Forbidden() => new() { Outcome = ClassOutcome.Forbidden };
}
