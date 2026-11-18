using System.ComponentModel.DataAnnotations;
using Project.Api.DTOs.Requests;

namespace Project.UnitTests.Classes;

public class JoinClassRequestValidationTests
{
    private static List<ValidationResult> Validate(JoinClassRequest request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, true);
        return results;
    }

    [Theory]
    [InlineData("ab12cd", "AB12CD")]
    [InlineData(" ab 12 cd ", "AB12CD")]
    [InlineData("AB12CD", "AB12CD")]
    public void JoinClassRequest_WithLowercaseLettersOrSpaces_NormalizesTheCode(string code, string expected)
    {
        var request = new JoinClassRequest { Code = code };

        Assert.Equal(expected, request.Code);
        Assert.Empty(Validate(request));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void JoinClassRequest_WithEmptyCode_IsInvalidWithRequired(string code)
    {
        var results = Validate(new JoinClassRequest { Code = code });

        Assert.Contains(results, r => r.ErrorMessage == "Required");
    }

    [Theory]
    [InlineData("ABC12")]
    [InlineData("ABC1234")]
    [InlineData("ABC-12")]
    [InlineData("ABČ123")]
    public void JoinClassRequest_WithWrongFormat_IsInvalidWithInvalidFormat(string code)
    {
        var results = Validate(new JoinClassRequest { Code = code });

        Assert.Contains(results, r => r.ErrorMessage == "InvalidFormat");
    }

    [Fact]
    public void JoinClassRequest_WithoutConfirmation_HasConfirmChangeFalse()
    {
        var request = new JoinClassRequest { Code = "AB12CD" };

        Assert.False(request.ConfirmChange);
    }
}
