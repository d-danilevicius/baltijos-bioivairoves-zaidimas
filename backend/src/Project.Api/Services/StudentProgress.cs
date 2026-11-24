using Project.Api.Entities;

namespace Project.Api.Services;

// The points of one student in each difficulty group and the unlocking of the groups (VT-014).
// Calculated from the current tasks each time; nothing about the unlocking is stored.
public sealed class StudentProgress
{
    private const int FirstGroup = 1;
    private const int RequiredPercent = 60;

    private readonly Dictionary<Guid, int> _groupOfTask;
    private readonly Dictionary<int, int> _taskCountOfGroup;
    private readonly Dictionary<int, int> _pointsOfGroup;

    private StudentProgress(
        Dictionary<Guid, int> groupOfTask,
        Dictionary<int, int> taskCountOfGroup,
        Dictionary<int, int> pointsOfGroup)
    {
        _groupOfTask = groupOfTask;
        _taskCountOfGroup = taskCountOfGroup;
        _pointsOfGroup = pointsOfGroup;
    }

    // Points of a task that no longer exists in the class are not counted
    public static StudentProgress Create(IEnumerable<(Guid TaskId, int Group)> tasks, IReadOnlyDictionary<Guid, int> points)
    {
        var groupOfTask = tasks.ToDictionary(task => task.TaskId, task => task.Group);
        var taskCountOfGroup = groupOfTask.Values.GroupBy(group => group).ToDictionary(g => g.Key, g => g.Count());
        var pointsOfGroup = points
            .Where(entry => groupOfTask.ContainsKey(entry.Key))
            .GroupBy(entry => groupOfTask[entry.Key])
            .ToDictionary(g => g.Key, g => g.Sum(entry => entry.Value));

        return new StudentProgress(groupOfTask, taskCountOfGroup, pointsOfGroup);
    }

    public int Points(int group) => _pointsOfGroup.GetValueOrDefault(group);

    public int MaxPoints(int group) => _taskCountOfGroup.GetValueOrDefault(group) * ClassTask.PointsOfGroup(group);

    // The first group is always unlocked and so is a group after a group without tasks
    public bool IsUnlocked(int group)
    {
        if (group == FirstGroup)
        {
            return true;
        }

        var previousMaxPoints = MaxPoints(group - 1);
        return previousMaxPoints == 0 || ReachesRequiredPercent(Points(group - 1), previousMaxPoints);
    }

    // The number of the next group that the points of this submission unlock, or null when it unlocks none
    public int? GroupUnlockedBy(Guid taskId, int points)
    {
        if (!_groupOfTask.TryGetValue(taskId, out var group))
        {
            return null;
        }

        var nextGroup = group + 1;
        if (MaxPoints(nextGroup) == 0 || IsUnlocked(nextGroup))
        {
            return null;
        }

        return ReachesRequiredPercent(Points(group) + points, MaxPoints(group)) ? nextGroup : null;
    }

    private static bool ReachesRequiredPercent(int points, int maxPoints) => points * 100 >= RequiredPercent * maxPoints;
}
