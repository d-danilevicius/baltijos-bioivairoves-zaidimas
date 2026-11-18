using System.ComponentModel.DataAnnotations;

namespace Project.Api.DTOs.Requests;

public class JoinClassRequest
{
    private string _code = string.Empty;

    // Spaces are removed and letters are converted to uppercase before the validation (VT-030)
    [Required(ErrorMessage = "Required")]
    [RegularExpression("^[A-Z0-9]{6}$", ErrorMessage = "InvalidFormat")]
    public string Code
    {
        get => _code;
        set => _code = string.Concat((value ?? string.Empty).Where(c => !char.IsWhiteSpace(c))).ToUpperInvariant();
    }

    // The student confirmed the change of the class (VT-029)
    public bool ConfirmChange { get; set; }
}
