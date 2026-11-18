namespace Project.Api.DTOs.Responses;

public class LoggedInUserResponse
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required string Role { get; set; }
}
