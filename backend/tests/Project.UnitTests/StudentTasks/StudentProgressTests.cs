using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.StudentTasks;

public class StudentProgressTests
{
    [Fact]
    public void IsUnlocked_ForTheFirstGroupWithoutPoints_ReturnsTrue()
    {
        var progress = CreateProgress(Tasks(1, 5), []);

        Assert.True(progress.IsUnlocked(1));
    }

    [Fact]
    public void IsUnlocked_ForTheSecondGroupWithoutPoints_ReturnsFalse()
    {
        var progress = CreateProgress([.. Tasks(1, 5), .. Tasks(2, 3)], []);

        Assert.False(progress.IsUnlocked(2));
    }

    [Fact]
    public void IsUnlocked_WhenThePreviousGroupHasExactlySixtyPercent_ReturnsTrue()
    {
        var firstGroup = Tasks(1, 5);
        var progress = CreateProgress([.. firstGroup, .. Tasks(2, 3)], PointsFor(firstGroup, 10, 10, 10));

        Assert.True(progress.IsUnlocked(2));
    }

    [Fact]
    public void IsUnlocked_WhenThePreviousGroupHasJustBelowSixtyPercent_ReturnsFalse()
    {
        var firstGroup = Tasks(1, 5);
        var progress = CreateProgress([.. firstGroup, .. Tasks(2, 3)], PointsFor(firstGroup, 10, 10, 9));

        Assert.False(progress.IsUnlocked(2));
    }

    [Fact]
    public void IsUnlocked_ForTheThirdGroup_DependsOnTheSecondGroup()
    {
        var firstGroup = Tasks(1, 2);
        var secondGroup = Tasks(2, 5);
        List<(Guid TaskId, int Group)> tasks = [.. firstGroup, .. secondGroup, .. Tasks(3, 2)];

        var secondNotEarned = CreateProgress(tasks, PointsFor(firstGroup, 10, 10));
        var secondEarned = CreateProgress(tasks, PointsFor(secondGroup, 20, 20, 20));

        Assert.False(secondNotEarned.IsUnlocked(3));
        Assert.True(secondEarned.IsUnlocked(3));
    }

    [Fact]
    public void IsUnlocked_WhenThePreviousGroupHasNoTasks_ReturnsTrue()
    {
        var progress = CreateProgress([.. Tasks(1, 2), .. Tasks(3, 2)], []);

        Assert.True(progress.IsUnlocked(3));
    }

    [Fact]
    public void Points_IgnoresThePointsOfATaskThatNoLongerExists()
    {
        var firstGroup = Tasks(1, 5);
        var points = PointsFor(firstGroup, 10, 10);
        points[Guid.NewGuid()] = 10;
        var progress = CreateProgress(firstGroup, points);

        Assert.Equal(20, progress.Points(1));
    }

    [Fact]
    public void IsUnlocked_WhenAnEarnedTaskWasDeleted_ReturnsFalse()
    {
        var firstGroup = Tasks(1, 5);
        var points = PointsFor(firstGroup, 10, 10, 10);
        var secondGroup = Tasks(2, 1);
        var withTask = CreateProgress([.. firstGroup, .. secondGroup], points);
        var withoutTask = CreateProgress([.. firstGroup.Skip(1), .. secondGroup], points);

        Assert.True(withTask.IsUnlocked(2));
        Assert.Equal(20, withoutTask.Points(1));
        Assert.Equal(40, withoutTask.MaxPoints(1));
        Assert.False(withoutTask.IsUnlocked(2));
    }

    [Fact]
    public void IsUnlocked_WhenATaskIsAdded_ChangesTheMaximumAndCanLockTheNextGroup()
    {
        var firstGroup = Tasks(1, 5);
        var points = PointsFor(firstGroup, 10, 10, 10);
        var secondGroup = Tasks(2, 1);
        var before = CreateProgress([.. firstGroup, .. secondGroup], points);
        var after = CreateProgress([.. firstGroup, .. Tasks(1, 1), .. secondGroup], points);

        Assert.True(before.IsUnlocked(2));
        Assert.Equal(60, after.MaxPoints(1));
        Assert.False(after.IsUnlocked(2));
    }

    [Fact]
    public void MaxPoints_IsTheNumberOfTasksTimesThePointsOfTheGroup()
    {
        var progress = CreateProgress([.. Tasks(1, 5), .. Tasks(2, 3), .. Tasks(3, 2)], []);

        Assert.Equal(50, progress.MaxPoints(1));
        Assert.Equal(60, progress.MaxPoints(2));
        Assert.Equal(60, progress.MaxPoints(3));
    }

    [Fact]
    public void MaxPoints_ForAGroupWithoutTasks_ReturnsZero()
    {
        var progress = CreateProgress(Tasks(1, 2), []);

        Assert.Equal(0, progress.MaxPoints(2));
        Assert.Equal(0, progress.Points(2));
    }

    [Fact]
    public void GroupUnlockedBy_WhenTheSubmissionReachesSixtyPercent_ReturnsTheNextGroup()
    {
        var firstGroup = Tasks(1, 5);
        var progress = CreateProgress([.. firstGroup, .. Tasks(2, 3)], PointsFor(firstGroup, 10, 10));

        Assert.Equal(2, progress.GroupUnlockedBy(firstGroup[2].TaskId, 10));
    }

    [Fact]
    public void GroupUnlockedBy_WhenTheSubmissionStaysBelowSixtyPercent_ReturnsNull()
    {
        var firstGroup = Tasks(1, 5);
        var progress = CreateProgress([.. firstGroup, .. Tasks(2, 3)], PointsFor(firstGroup, 10, 10));

        Assert.Null(progress.GroupUnlockedBy(firstGroup[2].TaskId, 0));
    }

    [Fact]
    public void GroupUnlockedBy_WhenTheNextGroupIsAlreadyUnlocked_ReturnsNull()
    {
        var firstGroup = Tasks(1, 5);
        var progress = CreateProgress([.. firstGroup, .. Tasks(2, 3)], PointsFor(firstGroup, 10, 10, 10));

        Assert.Null(progress.GroupUnlockedBy(firstGroup[3].TaskId, 10));
    }

    [Fact]
    public void GroupUnlockedBy_ForTheLastGroup_ReturnsNull()
    {
        var secondGroup = Tasks(2, 2);
        var progress = CreateProgress([.. Tasks(1, 1), .. secondGroup], []);

        Assert.Null(progress.GroupUnlockedBy(secondGroup[0].TaskId, 20));
    }

    [Fact]
    public void GroupUnlockedBy_WhenTheNextGroupHasNoTasks_ReturnsNull()
    {
        var firstGroup = Tasks(1, 1);
        var progress = CreateProgress(firstGroup, []);

        Assert.Null(progress.GroupUnlockedBy(firstGroup[0].TaskId, 10));
    }

    private static StudentProgress CreateProgress(IEnumerable<(Guid TaskId, int Group)> tasks, Dictionary<Guid, int> points) =>
        StudentProgress.Create(tasks, points);

    private static List<(Guid TaskId, int Group)> Tasks(int group, int count) =>
        Enumerable.Range(0, count).Select(_ => (Guid.NewGuid(), group)).ToList();

    private static Dictionary<Guid, int> PointsFor(IReadOnlyList<(Guid TaskId, int Group)> tasks, params int[] points) =>
        points.Select((value, index) => (tasks[index].TaskId, value)).ToDictionary(p => p.TaskId, p => p.value);
}
