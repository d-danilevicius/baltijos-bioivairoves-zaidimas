using Project.Api.Entities;
using Xunit;

namespace Project.UnitTests.Submissions;

public class ClassTaskPointsTests
{
    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 20)]
    [InlineData(3, 30)]
    public void PointsFor_WithCorrectAnswer_ReturnsPointsOfTheGroup(int group, int expectedPoints)
    {
        var task = CreateTask(group, "Paprastoji lapė");

        Assert.Equal(expectedPoints, task.PointsFor("Paprastoji lapė"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void PointsFor_WithIncorrectAnswer_ReturnsZero(int group)
    {
        var task = CreateTask(group, "Paprastoji lapė");

        Assert.Equal(0, task.PointsFor("Pilkoji pelėda"));
    }

    [Fact]
    public void PointsFor_WithDifferentLetterCase_ReturnsPoints()
    {
        var task = CreateTask(1, "Paprastoji lapė");

        Assert.Equal(10, task.PointsFor("PAPRASTOJI LAPĖ"));
    }

    [Fact]
    public void PointsFor_WithSpacesAtTheBeginningAndTheEnd_ReturnsPoints()
    {
        var task = CreateTask(2, "Paprastoji lapė");

        Assert.Equal(20, task.PointsFor("  Paprastoji lapė  "));
    }

    [Fact]
    public void PointsFor_WithSpacesAroundTheCorrectAnswer_ReturnsPoints()
    {
        var task = CreateTask(1, " Paprastoji lapė ");

        Assert.Equal(10, task.PointsFor("Paprastoji lapė"));
    }

    [Fact]
    public void PointsFor_WithDifferentInnerSpaces_ReturnsZero()
    {
        var task = CreateTask(1, "Paprastoji lapė");

        Assert.Equal(0, task.PointsFor("Paprastoji  lapė"));
    }

    [Fact]
    public void PointsFor_WithSameNumber_ReturnsPoints()
    {
        var task = CreateTask(1, "7");

        Assert.Equal(10, task.PointsFor("7"));
    }

    [Fact]
    public void PointsFor_WithNumberWrittenWithLeadingZero_ReturnsZero()
    {
        var task = CreateTask(1, "7");

        Assert.Equal(0, task.PointsFor("07"));
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 20)]
    [InlineData(3, 30)]
    public void PointsOfGroup_ReturnsTenTimesTheGroup(int group, int expectedPoints)
    {
        Assert.Equal(expectedPoints, ClassTask.PointsOfGroup(group));
    }

    private static ClassTask CreateTask(int group, string correctAnswer) => new()
    {
        Id = Guid.NewGuid(),
        ClassId = Guid.NewGuid(),
        Type = TaskType.CommonSpecies,
        DifficultyGroup = group,
        Question = "Klausimas",
        CorrectAnswer = correctAnswer
    };
}
