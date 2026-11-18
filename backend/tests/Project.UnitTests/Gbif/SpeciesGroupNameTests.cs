using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Gbif;

public class SpeciesGroupNameTests
{
    [Theory]
    [InlineData("Aves", "Paukščiai")]
    [InlineData("mammalia", "Žinduoliai")]
    [InlineData("Insecta", "Vabzdžiai")]
    [InlineData("Magnoliopsida", "Žydintys augalai")]
    [InlineData("Liliopsida", "Žydintys augalai")]
    public void For_WithKnownClass_ReturnsLithuanianName(string gbifClass, string expected)
    {
        Assert.Equal(expected, SpeciesGroupName.For(gbifClass));
    }

    [Fact]
    public void For_WithUnknownClass_ReturnsTheGbifName()
    {
        Assert.Equal("Gastropoda", SpeciesGroupName.For("Gastropoda"));
    }
}
