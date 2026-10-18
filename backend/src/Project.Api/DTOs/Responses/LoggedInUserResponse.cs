namespace Project.Api.DTOs.Responses;

public class LoggedInUserResponse
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Role { get; set; } = null!;
}
