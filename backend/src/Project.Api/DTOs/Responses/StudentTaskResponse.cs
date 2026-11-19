using Project.Api.Entities;

namespace Project.Api.DTOs.Responses;

public class StudentTaskResponse
{
    public Guid Id { get; set; }

    public TaskType Type { get; set; }

    public required string Label { get; set; }

    public StudentTaskStatus Status { get; set; }
}
