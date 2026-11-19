using Project.Api.DTOs.Responses;
using Project.Api.Entities;

namespace Project.Api.Services;

public static class ObservationCardExtensions
{
    private const string CardLabel = "Patvirtintas GBIF stebėjimas";
    private const string GbifRecordUrl = "https://www.gbif.org/occurrence/";

    public static ObservationCardResponse ToCard(this Observation observation)
    {
        var isProtected = ProtectedSpecies.IsProtected(observation.IucnCategory);

        return new ObservationCardResponse
        {
            Label = CardLabel,
            DatasetName = observation.DatasetName,
            License = observation.License,
            GbifUrl = GbifRecordUrl + observation.GbifKey,
            Locality = observation.Locality,
            Latitude = isProtected ? null : observation.Latitude,
            Longitude = isProtected ? null : observation.Longitude,
            CoordinatesHidden = isProtected
        };
    }
}
