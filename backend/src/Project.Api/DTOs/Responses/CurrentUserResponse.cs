namespace Project.Api.DTOs.Responses;

public class CurrentUserResponse
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Role { get; set; } = null!;
}
