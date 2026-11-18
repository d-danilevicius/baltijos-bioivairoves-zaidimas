using System.ComponentModel.DataAnnotations;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;
using Xunit;

namespace Project.UnitTests.Gbif;

public class CreateGbifTasksRequestValidationTests
{
    [Fact]
    public void Validate_WithPointInLithuaniaAndSeason_IsValid()
    {
        var request = new CreateGbifTasksRequest { Latitude = 55.7, Longitude = 21.1, Season = Season.Summer };

        Assert.Empty(Validate(request));
    }

    [Theory]
    [InlineData(53.9, 20.9)]
    [InlineData(56.5, 26.9)]
    public void Validate_WithPointOnTheBorderOfTheRectangle_IsValid(double latitude, double longitude)
    {
        var request = new CreateGbifTasksRequest { Latitude = latitude, Longitude = longitude, Season = Season.Winter };

        Assert.Empty(Validate(request));
    }

    [Theory]
    [InlineData(53.8, 24.0)]
    [InlineData(56.6, 24.0)]
    [InlineData(55.0, 20.8)]
    [InlineData(55.0, 27.0)]
    public void Validate_WithPointOutsideLithuania_IsInvalidWithOutsideLithuania(double latitude, double longitude)
    {
        var request = new CreateGbifTasksRequest { Latitude = latitude, Longitude = longitude, Season = Season.Spring };

        Assert.Contains(Validate(request), r => r.ErrorMessage == "OutsideLithuania");
    }

    [Fact]
    public void Validate_WithoutLocationAndSeason_IsInvalidWithRequiredForEachField()
    {
        var results = Validate(new CreateGbifTasksRequest());

        Assert.Equal(3, results.Count(r => r.ErrorMessage == "Required"));
    }

    [Fact]
    public void Validate_WithUndefinedSeason_IsInvalid()
    {
        var request = new CreateGbifTasksRequest { Latitude = 55.0, Longitude = 24.0, Season = (Season)99 };

        Assert.Contains(Validate(request), r => r.ErrorMessage == "Invalid");
    }

    private static List<ValidationResult> Validate(CreateGbifTasksRequest request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, true);
        return results;
    }
}
