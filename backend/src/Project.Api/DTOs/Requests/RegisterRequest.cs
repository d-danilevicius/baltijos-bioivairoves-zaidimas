using System.ComponentModel.DataAnnotations;

namespace Project.Api.DTOs.Requests;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [StringLength(254, ErrorMessage = "TooLong")]
    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression("^[A-Za-z0-9_]{3,30}$", ErrorMessage = "InvalidFormat")]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "InvalidLength")]
    public string Password { get; set; } = null!;

    [Required]
    [RegularExpression("^(Student|Teacher)$", ErrorMessage = "NotAllowed")]
    public string Role { get; set; } = null!;
}
