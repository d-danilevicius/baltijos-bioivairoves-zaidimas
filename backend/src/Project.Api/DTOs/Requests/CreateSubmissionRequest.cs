using System.ComponentModel.DataAnnotations;

namespace Project.Api.DTOs.Requests;

public class CreateSubmissionRequest
{
    private string _answer = string.Empty;
    private string _evidence = string.Empty;

    // The spaces at the beginning and the end are removed before the validation, so the limits apply to the trimmed text (VT-015)
    [Required(ErrorMessage = "Required")]
    [StringLength(100, ErrorMessage = "TooLong")]
    public string Answer
    {
        get => _answer;
        set => _answer = value?.Trim() ?? string.Empty;
    }

    [Required(ErrorMessage = "Required")]
    [StringLength(500, ErrorMessage = "TooLong")]
    public string Evidence
    {
        get => _evidence;
        set => _evidence = value?.Trim() ?? string.Empty;
    }
}
