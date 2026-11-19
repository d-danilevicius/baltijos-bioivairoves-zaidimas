using Project.Api.DTOs.Responses;
using Project.Api.Entities;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.ObservationCards;

public class ObservationCardExtensionsTests
{
    [Fact]
    public void ToCard_ForObservation_ReturnsLabelDatasetLicenceAndGbifLink()
    {
        var observation = CreateObservation(gbifKey: 4242, datasetName: "iNaturalist", license: "CC_BY_4_0");

        var card = observation.ToCard();

        Assert.Equal("Patvirtintas GBIF stebėjimas", card.Label);
        Assert.Equal("iNaturalist", card.DatasetName);
        Assert.Equal("CC_BY_4_0", card.License);
        Assert.Equal("https://www.gbif.org/occurrence/4242", card.GbifUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("LC")]
    [InlineData("NT")]
    public void ToCard_ForSpeciesThatIsNotProtected_ReturnsPlaceNameAndCoordinates(string? iucnCategory)
    {
        var observation = CreateObservation(iucnCategory: iucnCategory, locality: "Verkių regioninis parkas", latitude: 54.74, longitude: 25.29);

        var card = observation.ToCard();

        Assert.Equal("Verkių regioninis parkas", card.Locality);
        Assert.Equal(54.74, card.Latitude);
        Assert.Equal(25.29, card.Longitude);
        Assert.False(card.CoordinatesHidden);
    }

    [Theory]
    [InlineData("VU")]
    [InlineData("EN")]
    [InlineData("CR")]
    public void ToCard_ForProtectedSpecies_ReturnsPlaceNameWithoutCoordinates(string iucnCategory)
    {
        var observation = CreateObservation(iucnCategory: iucnCategory, locality: "Verkių regioninis parkas", latitude: 54.74, longitude: 25.29);

        var card = observation.ToCard();

        Assert.Equal("Verkių regioninis parkas", card.Locality);
        Assert.Null(card.Latitude);
        Assert.Null(card.Longitude);
        Assert.True(card.CoordinatesHidden);
    }

    [Fact]
    public void ToCard_ForProtectedSpeciesWithoutPlaceName_ReturnsNoPlaceNameAndNoCoordinates()
    {
        var observation = CreateObservation(iucnCategory: "EN", locality: null);

        var card = observation.ToCard();

        Assert.Null(card.Locality);
        Assert.Null(card.Latitude);
        Assert.Null(card.Longitude);
        Assert.True(card.CoordinatesHidden);
    }

    [Fact]
    public void ObservationCardResponse_HasNoSpeciesNameAndNoObservationDate()
    {
        var properties = typeof(ObservationCardResponse).GetProperties().Select(p => p.Name);

        Assert.DoesNotContain("Species", properties);
        Assert.DoesNotContain("ObservedOn", properties);
    }

    private static Observation CreateObservation(
        long gbifKey = 1,
        string? datasetName = "Dataset",
        string? license = "CC0_1_0",
        string? iucnCategory = null,
        string? locality = "Vilnius",
        double latitude = 54.7,
        double longitude = 25.3) => new()
        {
            Id = Guid.NewGuid(),
            ClassId = Guid.NewGuid(),
            GbifKey = gbifKey,
            Species = "Parus major",
            Latitude = latitude,
            Longitude = longitude,
            Locality = locality,
            ObservedOn = new DateOnly(2026, 5, 12),
            DatasetName = datasetName,
            License = license,
            IucnCategory = iucnCategory
        };
}
