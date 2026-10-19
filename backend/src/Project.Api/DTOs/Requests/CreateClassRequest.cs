using System.ComponentModel.DataAnnotations;

namespace Project.Api.DTOs.Requests;

public class CreateClassRequest
{
    private string _name = null!;

    // The spaces at the beginning and the end are removed before the validation, so the limit applies to the trimmed name
    [Required(ErrorMessage = "Required")]
    [StringLength(50, ErrorMessage = "TooLong")]
    public string Name
    {
        get => _name;
        set => _name = value?.Trim()!;
    }
}
