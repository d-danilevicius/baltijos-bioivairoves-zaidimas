using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.StudentTasks;

public class TaskGroupUnlockingTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3 })]
    [InlineData(new[] { 1 })]
    [InlineData(new[] { 1, 3 })]
    public void IsUnlocked_ForGroupOne_ReturnsTrue(int[] groupsWithTasks)
    {
        Assert.True(TaskGroupUnlocking.IsUnlocked(1, groupsWithTasks));
    }

    [Theory]
    [InlineData(2, new[] { 1, 2, 3 })]
    [InlineData(2, new[] { 1, 2 })]
    [InlineData(3, new[] { 1, 2, 3 })]
    [InlineData(3, new[] { 2, 3 })]
    public void IsUnlocked_WhenPreviousGroupHasTasksAndStudentHasNoPoints_ReturnsFalse(int group, int[] groupsWithTasks)
    {
        Assert.False(TaskGroupUnlocking.IsUnlocked(group, groupsWithTasks));
    }

    [Theory]
    [InlineData(2, new[] { 2, 3 })]
    [InlineData(2, new[] { 2 })]
    [InlineData(3, new[] { 1, 3 })]
    [InlineData(3, new[] { 3 })]
    public void IsUnlocked_WhenPreviousGroupHasNoTasks_ReturnsTrue(int group, int[] groupsWithTasks)
    {
        Assert.True(TaskGroupUnlocking.IsUnlocked(group, groupsWithTasks));
    }

    [Fact]
    public void IsUnlocked_WhenThereAreNoTasks_ReturnsTrueOnlyForGroupsAfterAnEmptyGroup()
    {
        var groupsWithTasks = Array.Empty<int>();

        Assert.True(TaskGroupUnlocking.IsUnlocked(1, groupsWithTasks));
        Assert.True(TaskGroupUnlocking.IsUnlocked(2, groupsWithTasks));
        Assert.True(TaskGroupUnlocking.IsUnlocked(3, groupsWithTasks));
    }
}
