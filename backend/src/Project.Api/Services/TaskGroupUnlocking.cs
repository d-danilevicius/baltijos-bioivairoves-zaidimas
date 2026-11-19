namespace Project.Api.Services;

// Decides which difficulty groups are unlocked (VT-014).
// There are no points yet (features 014-016), so a group after a group with tasks stays locked.
public static class TaskGroupUnlocking
{
    private const int FirstGroup = 1;

    public static bool IsUnlocked(int group, IReadOnlyCollection<int> groupsWithTasks)
    {
        return group == FirstGroup || !groupsWithTasks.Contains(group - 1);
    }
}
