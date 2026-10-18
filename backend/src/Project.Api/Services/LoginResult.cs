using System.Collections.Generic;
using Project.Api.DTOs.Responses;

namespace Project.Api.Services;

public class LoginResult
{
    public bool Success { get; set; }

    // field -> list of contract error codes (use empty string key for general errors)
    public Dictionary<string, string[]>? Errors { get; set; }

    // Returned user on success
    public LoggedInUserResponse? User { get; set; }

    // Outcome enum to make result handling explicit
    public LoginOutcome Outcome { get; set; }

    public static LoginResult Succeeded(LoggedInUserResponse user) => new() { Success = true, User = user, Outcome = LoginOutcome.Succeeded };

    public static LoginResult InvalidCredentials() => new()
    {
        Success = false,
        Outcome = LoginOutcome.InvalidCredentials,
        Errors = new Dictionary<string, string[]>
        {
            [""] = new[] { "InvalidCredentials" }
        }
    };

    public static LoginResult Blocked() => new()
    {
        Success = false,
        Outcome = LoginOutcome.Blocked,
        Errors = new Dictionary<string, string[]>
        {
            [""] = new[] { "AccountBlocked" }
        }
    };
}
