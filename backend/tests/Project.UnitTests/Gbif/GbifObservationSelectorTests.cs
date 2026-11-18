using Project.Api.Integrations.Gbif;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Gbif;

public class GbifObservationSelectorTests
{
    private const double Latitude = 55.0;
    private const double Longitude = 24.0;
    private const double DegreesPerKilometre = 1 / 111.195;

    [Theory]
    [InlineData(0.1)]
    [InlineData(9.9)]
    public void Select_WhenObservationIsWithinTenKilometres_KeepsIt(double kilometres)
    {
        var occurrence = CreateOccurrence(1, "Parus major", Latitude + kilometres * DegreesPerKilometre, Longitude);

        var selected = GbifObservationSelector.Select(new[] { occurrence }, Latitude, Longitude);

        Assert.Single(selected);
    }

    [Fact]
    public void Select_WhenObservationIsFartherThanTenKilometres_RemovesIt()
    {
        var occurrence = CreateOccurrence(1, "Parus major", Latitude + 10.1 * DegreesPerKilometre, Longitude);

        var selected = GbifObservationSelector.Select(new[] { occurrence }, Latitude, Longitude);

        Assert.Empty(selected);
    }

    [Fact]
    public void Select_WhenCoordinatesAreMissing_RemovesTheObservation()
    {
        var withoutLatitude = CreateOccurrence(1, "Parus major", null, Longitude);
        var withoutLongitude = CreateOccurrence(2, "Parus major", Latitude, null);

        var selected = GbifObservationSelector.Select(new[] { withoutLatitude, withoutLongitude }, Latitude, Longitude);

        Assert.Empty(selected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Select_WhenSpeciesNameIsMissing_RemovesTheObservation(string? species)
    {
        var occurrence = CreateOccurrence(1, species, Latitude, Longitude);

        var selected = GbifObservationSelector.Select(new[] { occurrence }, Latitude, Longitude);

        Assert.Empty(selected);
    }

    [Fact]
    public void Select_WhenTheSameGbifRecordRepeats_KeepsItOnce()
    {
        var first = CreateOccurrence(7, "Parus major", Latitude, Longitude);
        var repeated = CreateOccurrence(7, "Parus major", Latitude, Longitude);

        var selected = GbifObservationSelector.Select(new[] { first, repeated }, Latitude, Longitude);

        Assert.Single(selected);
    }

    [Fact]
    public void Select_TrimsTheSpeciesName()
    {
        var occurrence = CreateOccurrence(1, "  Parus major ", Latitude, Longitude);

        var selected = GbifObservationSelector.Select(new[] { occurrence }, Latitude, Longitude);

        Assert.Equal("Parus major", selected[0].Species);
    }

    private static GbifOccurrence CreateOccurrence(long key, string? species, double? latitude, double? longitude) =>
        new(key, species, "Aves", "Paridae", latitude, longitude, "Panevezys", new DateOnly(2026, 6, 15), "Dataset", "CC0_1_0", null);
}
