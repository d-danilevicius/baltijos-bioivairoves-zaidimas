namespace Project.Api.DTOs.Responses;

public class StudentTaskGroupResponse
{
    public int Group { get; set; }

    public bool IsUnlocked { get; set; }

    public required IReadOnlyList<StudentTaskResponse> Tasks { get; set; }
}
