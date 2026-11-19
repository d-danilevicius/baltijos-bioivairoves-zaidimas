using Project.Api.DTOs.Responses;
using Project.Api.Entities;

namespace Project.Api.Services;

public static class ClassTaskExtensions
{
    private const string GbifLabel = "GBIF";
    private const string TeacherLabel = "Mokytojo užduotis";

    public static string ToLabel(this ClassTask task) => task.Source == TaskSource.Teacher ? TeacherLabel : GbifLabel;

    public static ClassTaskResponse ToResponse(this ClassTask task) => new()
    {
        Id = task.Id,
        Type = task.Type,
        DifficultyGroup = task.DifficultyGroup,
        Label = task.ToLabel()
    };

    public static StudentTaskDetailsResponse ToDetails(this ClassTask task, Observation? observation) => new()
    {
        Id = task.Id,
        Type = task.Type,
        DifficultyGroup = task.DifficultyGroup,
        Label = task.ToLabel(),
        Title = task.Title,
        Question = task.Question,
        Options = task.Options ?? [],
        Observation = observation?.ToCard()
    };
}
