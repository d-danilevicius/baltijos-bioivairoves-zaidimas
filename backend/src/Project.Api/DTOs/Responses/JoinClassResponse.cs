namespace Project.Api.DTOs.Responses;

public class JoinClassResponse
{
    public required string ClassName { get; set; }

    public bool AlreadyMember { get; set; }
}
