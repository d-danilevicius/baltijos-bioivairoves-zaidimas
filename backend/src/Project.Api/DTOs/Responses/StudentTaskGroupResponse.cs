namespace Project.Api.DTOs.Responses;

public class StudentTaskGroupResponse
{
    public int Group { get; set; }

    public bool IsUnlocked { get; set; }

    // The points the student earned in the group and the most that the group can give now
    public int Points { get; set; }

    public int MaxPoints { get; set; }

    public required IReadOnlyList<StudentTaskResponse> Tasks { get; set; }
}
