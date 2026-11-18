using Project.Api.Entities;

namespace Project.Api.DTOs.Responses;

public class ClassTaskResponse
{
    public Guid Id { get; set; }

    public TaskType Type { get; set; }

    public int DifficultyGroup { get; set; }

    public required string Label { get; set; }
}
