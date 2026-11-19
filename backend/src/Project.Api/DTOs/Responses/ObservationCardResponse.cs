using System.Text.Json.Serialization;

namespace Project.Api.DTOs.Responses;

// The card of a confirmed GBIF observation (FR-043). It has no species name and no observation date,
// because they would reveal the answer of a task; the coordinates of a protected species are not included (FR-046)
public class ObservationCardResponse
{
    public required string Label { get; set; }

    public string? DatasetName { get; set; }

    public string? License { get; set; }

    public required string GbifUrl { get; set; }

    public string? Locality { get; set; }

    // Left out of the JSON when null, so a protected species has no coordinate fields at all (FR-005)
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Latitude { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Longitude { get; set; }

    public bool CoordinatesHidden { get; set; }
}
