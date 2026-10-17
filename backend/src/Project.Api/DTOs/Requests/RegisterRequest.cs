using System.ComponentModel.DataAnnotations;

namespace Project.Api.DTOs.Requests;

public class RegisterRequest
{
    [Required(ErrorMessage = "Required")]
    [EmailAddress(ErrorMessage = "InvalidFormat")]
    [StringLength(254, ErrorMessage = "TooLong")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [RegularExpression("^[A-Za-z0-9_]{3,30}$", ErrorMessage = "InvalidFormat")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "InvalidLength")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [RegularExpression("^(Student|Teacher)$", ErrorMessage = "NotAllowed")]
    public string Role { get; set; } = null!;
}
