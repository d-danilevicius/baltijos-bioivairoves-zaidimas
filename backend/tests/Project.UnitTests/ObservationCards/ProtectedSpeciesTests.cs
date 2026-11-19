using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.ObservationCards;

public class ProtectedSpeciesTests
{
    [Theory]
    [InlineData("VU")]
    [InlineData("EN")]
    [InlineData("CR")]
    public void IsProtected_ForVulnerableEndangeredOrCriticallyEndangered_ReturnsTrue(string category)
    {
        Assert.True(ProtectedSpecies.IsProtected(category));
    }

    [Theory]
    [InlineData("LC")]
    [InlineData("NT")]
    [InlineData("DD")]
    [InlineData("EW")]
    [InlineData("EX")]
    [InlineData("")]
    public void IsProtected_ForOtherCategories_ReturnsFalse(string category)
    {
        Assert.False(ProtectedSpecies.IsProtected(category));
    }

    [Fact]
    public void IsProtected_WhenGbifGaveNoCategory_ReturnsFalse()
    {
        Assert.False(ProtectedSpecies.IsProtected(null));
    }
}
