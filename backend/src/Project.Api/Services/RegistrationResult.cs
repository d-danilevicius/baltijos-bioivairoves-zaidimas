using Project.Api.DTOs.Responses;

namespace Project.Api.Services;

public class RegistrationResult
{
    public bool Success { get; set; }
    public string[]? Errors { get; set; }

    // Created user data to be returned in the API response (id, userName, role)
    public RegisteredUserResponse? CreatedUser { get; set; }
}
