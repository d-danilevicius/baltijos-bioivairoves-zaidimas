namespace Project.Api.Services;

public class JoinClassResult
{
    public JoinClassOutcome Outcome { get; private init; }

    // The class the student joins or tries to join
    public string ClassName { get; private init; } = string.Empty;

    // The class the student belongs to now (set when the confirmation is required)
    public string CurrentClassName { get; private init; } = string.Empty;

    public static JoinClassResult Joined(string className) =>
        new() { Outcome = JoinClassOutcome.Joined, ClassName = className };

    public static JoinClassResult AlreadyMember(string className) =>
        new() { Outcome = JoinClassOutcome.AlreadyMember, ClassName = className };

    public static JoinClassResult ConfirmationRequired(string className, string currentClassName) =>
        new() { Outcome = JoinClassOutcome.ConfirmationRequired, ClassName = className, CurrentClassName = currentClassName };

    public static JoinClassResult ClassNotFound() => new() { Outcome = JoinClassOutcome.ClassNotFound };
}
