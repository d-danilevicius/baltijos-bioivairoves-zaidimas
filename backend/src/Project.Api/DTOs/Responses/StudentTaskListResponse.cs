namespace Project.Api.DTOs.Responses;

public class StudentTaskListResponse
{
    public bool HasClass { get; set; }

    public required IReadOnlyList<StudentTaskGroupResponse> Groups { get; set; }

    // The first unfinished task of an unlocked group; empty when there is none
    public Guid? ContinueTaskId { get; set; }
}
