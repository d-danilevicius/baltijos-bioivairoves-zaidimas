using Project.Api.Entities;
using Xunit;

namespace Project.UnitTests.Gbif;

public class SeasonExtensionsTests
{
    [Theory]
    [InlineData(Season.Spring, new[] { 3, 4, 5 })]
    [InlineData(Season.Summer, new[] { 6, 7, 8 })]
    [InlineData(Season.Autumn, new[] { 9, 10, 11 })]
    [InlineData(Season.Winter, new[] { 12, 1, 2 })]
    public void Months_ForEverySeason_ReturnsMonthsOfVt012(Season season, int[] expectedMonths)
    {
        Assert.Equal(expectedMonths, season.Months());
    }

    [Theory]
    [InlineData(Season.Spring, Season.Autumn)]
    [InlineData(Season.Summer, Season.Winter)]
    [InlineData(Season.Autumn, Season.Spring)]
    [InlineData(Season.Winter, Season.Summer)]
    public void Opposite_ForEverySeason_ReturnsTheOppositeSeason(Season season, Season expected)
    {
        Assert.Equal(expected, season.Opposite());
    }
}
