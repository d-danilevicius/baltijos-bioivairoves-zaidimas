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

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public bool CoordinatesHidden { get; set; }
}
