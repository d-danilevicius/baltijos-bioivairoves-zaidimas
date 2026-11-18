namespace Project.Api.DTOs.Responses;

public class ClassChangeConfirmationResponse
{
    public required string CurrentClassName { get; set; }

    public required string NewClassName { get; set; }
}
