using System.Collections.Generic;
using Project.Api.DTOs.Responses;

namespace Project.Api.Services;

public class RegistrationResult
{
    public bool Success { get; set; }

    // field -> list of contract error codes (field names: email, userName, password, role)
    public Dictionary<string, string[]>? Errors { get; set; }

    // Whether the result maps to an HTTP 409 Conflict
    public bool IsConflict { get; set; }

    // Created user data to be returned in the API response (id, userName, role)
    public RegisteredUserResponse? CreatedUser { get; set; }

    public static RegistrationResult Created(RegisteredUserResponse created) => new() { Success = true, CreatedUser = created };

    public static RegistrationResult Conflict(Dictionary<string, string[]> errors) => new() { Success = false, Errors = errors, IsConflict = true };

    public static RegistrationResult Invalid(Dictionary<string, string[]> errors) => new() { Success = false, Errors = errors, IsConflict = false };
}
