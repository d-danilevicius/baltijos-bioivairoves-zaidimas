using Project.Api.DTOs.Responses;
using Project.Api.Entities;

namespace Project.Api.Services;

public static class ClassTaskExtensions
{
    private const string GbifLabel = "GBIF";
    private const string TeacherLabel = "Mokytojo užduotis";

    public static ClassTaskResponse ToResponse(this ClassTask task) => new()
    {
        Id = task.Id,
        Type = task.Type,
        DifficultyGroup = task.DifficultyGroup,
        Label = task.Source == TaskSource.Teacher ? TeacherLabel : GbifLabel
    };
}
