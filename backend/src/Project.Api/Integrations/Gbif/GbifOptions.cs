using System.ComponentModel.DataAnnotations;

namespace Project.Api.Integrations.Gbif;

public class GbifOptions
{
    public const string SectionName = "Gbif";

    // NFR-002: only a secure address is allowed
    [Required]
    [RegularExpression("^https://.+", ErrorMessage = "The GBIF address must start with https://.")]
    public string BaseUrl { get; set; } = string.Empty;

    // NFR-020: GBIF must answer within 10 seconds
    [Range(1, 10)]
    public int TimeoutSeconds { get; set; } = 10;
}
