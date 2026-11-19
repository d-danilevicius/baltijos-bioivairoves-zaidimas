using System.ComponentModel.DataAnnotations;

namespace Project.Api.DTOs.Requests;

public class CreateTeacherTaskRequest
{
    private string _title = string.Empty;
    private string _question = string.Empty;
    private string _correctAnswer = string.Empty;

    // The spaces at the beginning and the end are removed before the validation, so the limits apply to the trimmed text (VT-018)
    [Required(ErrorMessage = "Required")]
    [StringLength(100, ErrorMessage = "TooLong")]
    public string Title
    {
        get => _title;
        set => _title = value?.Trim() ?? string.Empty;
    }

    [Required(ErrorMessage = "Required")]
    [StringLength(500, ErrorMessage = "TooLong")]
    public string Question
    {
        get => _question;
        set => _question = value?.Trim() ?? string.Empty;
    }

    [Required(ErrorMessage = "Required")]
    [StringLength(100, ErrorMessage = "TooLong")]
    public string CorrectAnswer
    {
        get => _correctAnswer;
        set => _correctAnswer = value?.Trim() ?? string.Empty;
    }

    // The three difficulty groups (VT-013)
    [Required(ErrorMessage = "Required")]
    [Range(1, 3, ErrorMessage = "Invalid")]
    public int? DifficultyGroup { get; set; }

    // The teacher confirms that the task does not require collecting or disturbing organisms (FR-032, VT-021)
    [Range(typeof(bool), "true", "true", ErrorMessage = "NotConfirmed")]
    public bool NoOrganismDisturbanceConfirmed { get; set; }
}
