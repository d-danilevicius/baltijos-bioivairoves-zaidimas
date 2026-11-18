using System.ComponentModel.DataAnnotations;
using Project.Api.Entities;

namespace Project.Api.DTOs.Requests;

public class CreateGbifTasksRequest
{
    // The territory of Lithuania is checked as a rectangle (VT-010)
    public const double MinLatitude = 53.9;
    public const double MaxLatitude = 56.5;
    public const double MinLongitude = 20.9;
    public const double MaxLongitude = 26.9;

    [Required(ErrorMessage = "Required")]
    [Range(MinLatitude, MaxLatitude, ErrorMessage = "OutsideLithuania")]
    public double? Latitude { get; set; }

    [Required(ErrorMessage = "Required")]
    [Range(MinLongitude, MaxLongitude, ErrorMessage = "OutsideLithuania")]
    public double? Longitude { get; set; }

    [Required(ErrorMessage = "Required")]
    [EnumDataType(typeof(Season), ErrorMessage = "Invalid")]
    public Season? Season { get; set; }
}
