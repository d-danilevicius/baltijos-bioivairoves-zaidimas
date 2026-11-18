using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Gbif;

public class GeoDistanceTests
{
    [Fact]
    public void InKilometres_ForTheSamePoint_ReturnsZero()
    {
        Assert.Equal(0, GeoDistance.InKilometres(55.0, 24.0, 55.0, 24.0), 6);
    }

    [Fact]
    public void InKilometres_ForOneDegreeOfLatitude_ReturnsAboutOneHundredElevenKilometres()
    {
        var distance = GeoDistance.InKilometres(55.0, 24.0, 56.0, 24.0);

        Assert.InRange(distance, 110.9, 111.5);
    }

    [Fact]
    public void InKilometres_IsTheSameInBothDirections()
    {
        var forward = GeoDistance.InKilometres(54.7, 25.3, 55.7, 21.1);
        var backward = GeoDistance.InKilometres(55.7, 21.1, 54.7, 25.3);

        Assert.Equal(forward, backward, 9);
    }
}
