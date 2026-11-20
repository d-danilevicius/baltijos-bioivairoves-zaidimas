using System.ComponentModel.DataAnnotations;
using Project.Api.DTOs.Requests;
using Xunit;

namespace Project.UnitTests.Submissions;

public class CreateSubmissionRequestValidationTests
{
    [Fact]
    public void Validate_WithAnswerAndEvidence_IsValid()
    {
        Assert.Empty(Validate(ValidRequest()));
    }

    [Fact]
    public void Validate_WithLimitLengths_IsValid()
    {
        var request = new CreateSubmissionRequest
        {
            Answer = new string('a', 100),
            Evidence = new string('b', 500)
        };

        Assert.Empty(Validate(request));
    }

    [Theory]
    [InlineData(nameof(CreateSubmissionRequest.Answer))]
    [InlineData(nameof(CreateSubmissionRequest.Evidence))]
    public void Validate_WithEmptyText_IsInvalidWithRequired(string field)
    {
        var request = ValidRequest();
        Set(request, field, string.Empty);

        Assert.Contains(Validate(request), r => r.ErrorMessage == "Required" && r.MemberNames.Contains(field));
    }

    [Theory]
    [InlineData(nameof(CreateSubmissionRequest.Answer))]
    [InlineData(nameof(CreateSubmissionRequest.Evidence))]
    public void Validate_WithOnlySpaces_IsInvalidWithRequired(string field)
    {
        var request = ValidRequest();
        Set(request, field, "   ");

        Assert.Contains(Validate(request), r => r.ErrorMessage == "Required" && r.MemberNames.Contains(field));
    }

    [Theory]
    [InlineData(nameof(CreateSubmissionRequest.Answer), 101)]
    [InlineData(nameof(CreateSubmissionRequest.Evidence), 501)]
    public void Validate_WithTooLongText_IsInvalidWithTooLong(string field, int length)
    {
        var request = ValidRequest();
        Set(request, field, new string('x', length));

        Assert.Contains(Validate(request), r => r.ErrorMessage == "TooLong" && r.MemberNames.Contains(field));
    }

    [Fact]
    public void Validate_WithLimitLengthsAndSpacesAround_IsValid()
    {
        var request = new CreateSubmissionRequest
        {
            Answer = "  " + new string('a', 100) + "  ",
            Evidence = "  " + new string('b', 500) + "  "
        };

        Assert.Empty(Validate(request));
    }

    [Fact]
    public void Validate_WithSpacesAroundAnswerAndEvidence_RemovesTheSpaces()
    {
        var request = new CreateSubmissionRequest { Answer = "  Gandras ", Evidence = " Mačiau prie ežero  " };

        Assert.Equal("Gandras", request.Answer);
        Assert.Equal("Mačiau prie ežero", request.Evidence);
    }

    private static CreateSubmissionRequest ValidRequest() => new()
    {
        Answer = "Gandras",
        Evidence = "Mačiau prie ežero."
    };

    private static void Set(CreateSubmissionRequest request, string field, string value)
    {
        if (field == nameof(CreateSubmissionRequest.Answer))
        {
            request.Answer = value;
            return;
        }

        request.Evidence = value;
    }

    private static List<ValidationResult> Validate(CreateSubmissionRequest request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
        return results;
    }
}
