using System.ComponentModel.DataAnnotations;
using Project.Api.DTOs.Requests;

namespace Project.UnitTests.Classes;

public class CreateClassRequestValidationTests
{
    private static List<ValidationResult> Validate(CreateClassRequest request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, true);
        return results;
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateClassRequest_WithEmptyName_IsInvalidWithRequired(string name)
    {
        var results = Validate(new CreateClassRequest { Name = name });

        Assert.Contains(results, r => r.ErrorMessage == "Required");
    }

    [Fact]
    public void CreateClassRequest_WithFiftyOneCharacters_IsInvalidWithTooLong()
    {
        var results = Validate(new CreateClassRequest { Name = new string('a', 51) });

        Assert.Contains(results, r => r.ErrorMessage == "TooLong");
    }

    [Fact]
    public void CreateClassRequest_WithFiftyCharacters_IsValid()
    {
        var results = Validate(new CreateClassRequest { Name = new string('a', 50) });

        Assert.Empty(results);
    }
}
