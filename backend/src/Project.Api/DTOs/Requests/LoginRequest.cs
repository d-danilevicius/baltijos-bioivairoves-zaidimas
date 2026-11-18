using System.ComponentModel.DataAnnotations;

namespace Project.Api.DTOs.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "Required")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required")]
    public string Password { get; set; } = string.Empty;
}
