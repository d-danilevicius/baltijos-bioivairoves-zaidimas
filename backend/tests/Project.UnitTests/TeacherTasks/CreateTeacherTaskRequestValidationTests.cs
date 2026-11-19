using System.ComponentModel.DataAnnotations;
using Project.Api.DTOs.Requests;
using Xunit;

namespace Project.UnitTests.TeacherTasks;

public class CreateTeacherTaskRequestValidationTests
{
    [Fact]
    public void Validate_WithAllFields_IsValid()
    {
        Assert.Empty(Validate(ValidRequest()));
    }

    [Fact]
    public void Validate_WithLimitLengths_IsValid()
    {
        var request = ValidRequest();
        request.Title = new string('a', 100);
        request.Question = new string('b', 500);
        request.CorrectAnswer = new string('c', 100);

        Assert.Empty(Validate(request));
    }

    [Theory]
    [InlineData(nameof(CreateTeacherTaskRequest.Title))]
    [InlineData(nameof(CreateTeacherTaskRequest.Question))]
    [InlineData(nameof(CreateTeacherTaskRequest.CorrectAnswer))]
    public void Validate_WithEmptyText_IsInvalidWithRequired(string field)
    {
        var request = ValidRequest();
        Set(request, field, string.Empty);

        Assert.Contains(Validate(request), r => r.ErrorMessage == "Required" && r.MemberNames.Contains(field));
    }

    [Theory]
    [InlineData(nameof(CreateTeacherTaskRequest.Title))]
    [InlineData(nameof(CreateTeacherTaskRequest.Question))]
    [InlineData(nameof(CreateTeacherTaskRequest.CorrectAnswer))]
    public void Validate_WithOnlySpaces_IsInvalidWithRequired(string field)
    {
        var request = ValidRequest();
        Set(request, field, "   ");

        Assert.Contains(Validate(request), r => r.ErrorMessage == "Required" && r.MemberNames.Contains(field));
    }

    [Theory]
    [InlineData(nameof(CreateTeacherTaskRequest.Title), 101)]
    [InlineData(nameof(CreateTeacherTaskRequest.Question), 501)]
    [InlineData(nameof(CreateTeacherTaskRequest.CorrectAnswer), 101)]
    public void Validate_WithTooLongText_IsInvalidWithTooLong(string field, int length)
    {
        var request = ValidRequest();
        Set(request, field, new string('x', length));

        Assert.Contains(Validate(request), r => r.ErrorMessage == "TooLong" && r.MemberNames.Contains(field));
    }

    [Fact]
    public void Validate_WithTitleOfLimitLengthAndSpacesAround_IsValid()
    {
        var request = ValidRequest();
        request.Title = "  " + new string('a', 100) + "  ";

        Assert.Empty(Validate(request));
    }

    [Fact]
    public void Validate_WithSpacesAroundQuestionAndAnswer_RemovesTheSpaces()
    {
        var request = ValidRequest();
        request.Question = "  Kas čia? ";
        request.CorrectAnswer = " Gandras  ";

        Assert.Equal("Kas čia?", request.Question);
        Assert.Equal("Gandras", request.CorrectAnswer);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Validate_WithDifficultyGroupFromOneToThree_IsValid(int group)
    {
        var request = ValidRequest();
        request.DifficultyGroup = group;

        Assert.Empty(Validate(request));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(-1)]
    public void Validate_WithDifficultyGroupOutsideOneToThree_IsInvalid(int group)
    {
        var request = ValidRequest();
        request.DifficultyGroup = group;

        Assert.Contains(Validate(request), r => r.ErrorMessage == "Invalid" && r.MemberNames.Contains(nameof(request.DifficultyGroup)));
    }

    [Fact]
    public void Validate_WithoutDifficultyGroup_IsInvalidWithRequired()
    {
        var request = ValidRequest();
        request.DifficultyGroup = null;

        Assert.Contains(Validate(request), r => r.ErrorMessage == "Required" && r.MemberNames.Contains(nameof(request.DifficultyGroup)));
    }

    [Fact]
    public void Validate_WithoutConfirmation_IsInvalidWithNotConfirmed()
    {
        var request = ValidRequest();
        request.NoOrganismDisturbanceConfirmed = false;

        Assert.Contains(Validate(request), r => r.ErrorMessage == "NotConfirmed");
    }

    private static CreateTeacherTaskRequest ValidRequest() => new()
    {
        Title = "Mokyklos kiemas",
        Question = "Kokį paukštį matote ant stogo?",
        CorrectAnswer = "Žvirblis",
        DifficultyGroup = 1,
        NoOrganismDisturbanceConfirmed = true
    };

    private static void Set(CreateTeacherTaskRequest request, string field, string value)
    {
        switch (field)
        {
            case nameof(CreateTeacherTaskRequest.Title): request.Title = value; break;
            case nameof(CreateTeacherTaskRequest.Question): request.Question = value; break;
            default: request.CorrectAnswer = value; break;
        }
    }

    private static List<ValidationResult> Validate(CreateTeacherTaskRequest request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, true);
        return results;
    }
}
