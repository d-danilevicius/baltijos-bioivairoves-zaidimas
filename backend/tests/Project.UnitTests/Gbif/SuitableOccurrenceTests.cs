using Project.Api.Entities;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Gbif;

public class SuitableOccurrenceTests
{
    [Fact]
    public void ToObservation_CopiesTheOccurrenceForTheClassAndTheSeason()
    {
        var classId = Guid.NewGuid();
        var occurrence = new SuitableOccurrence(
            55, "Parus major", 55.1, 24.2, "Aves", "Paridae", "Panevezys",
            new DateOnly(2025, 6, 15), "iNaturalist", "CC0_1_0", "VU");

        var observation = occurrence.ToObservation(classId, Season.Summer);

        Assert.NotEqual(Guid.Empty, observation.Id);
        Assert.Equal(classId, observation.ClassId);
        Assert.Equal(Season.Summer, observation.Season);
        Assert.Equal(55, observation.GbifKey);
        Assert.Equal("Parus major", observation.Species);
        Assert.Equal(55.1, observation.Latitude);
        Assert.Equal(24.2, observation.Longitude);
        Assert.Equal("Aves", observation.GbifClass);
        Assert.Equal("Paridae", observation.Family);
        Assert.Equal("Panevezys", observation.Locality);
        Assert.Equal(new DateOnly(2025, 6, 15), observation.ObservedOn);
        Assert.Equal("iNaturalist", observation.DatasetName);
        Assert.Equal("CC0_1_0", observation.License);
        Assert.Equal("VU", observation.IucnCategory);
    }
}
