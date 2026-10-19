using System.ComponentModel.DataAnnotations;

namespace Project.Api.DTOs.Requests;

public class CreateClassRequest
{
    [Required(ErrorMessage = "Required")]
    [StringLength(50, ErrorMessage = "TooLong")]
    public string Name { get; set; } = null!;
}
