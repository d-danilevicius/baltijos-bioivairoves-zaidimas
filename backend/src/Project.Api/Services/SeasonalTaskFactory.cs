using Project.Api.Entities;

namespace Project.Api.Services;

public class SeasonalTaskFactory
{
    public const int TaskCount = 2;

    private static readonly Dictionary<Season, string> SeasonNames = new()
    {
        [Season.Spring] = "Pavasaris",
        [Season.Summer] = "Vasara",
        [Season.Autumn] = "Ruduo",
        [Season.Winter] = "Žiema"
    };

    private readonly Random _random;

    public SeasonalTaskFactory(Random random)
    {
        _random = random;
    }

    // FR-029: compares the chosen season with the opposite one; skipped when the opposite season has too few observations
    public IReadOnlyList<ClassTask> Create(
        Guid classId,
        Season season,
        IReadOnlyList<Observation> chosen,
        IReadOnlyList<Observation> opposite)
    {
        if (opposite.Count < GbifObservationSelector.MinObservations)
        {
            return Array.Empty<ClassTask>();
        }

        var oppositeCounts = opposite.GroupBy(o => o.Species).ToDictionary(g => g.Key, g => g.Count());

        return chosen
            .GroupBy(o => o.Species)
            .Where(g => oppositeCounts.TryGetValue(g.Key, out var other) && other != g.Count())
            .OrderByDescending(g => g.Count() + oppositeCounts[g.Key])
            .ThenBy(g => g.Key, StringComparer.Ordinal)
            .Take(TaskCount)
            .Select(g => CreateTask(classId, season, g.First(), g.Count(), oppositeCounts[g.Key]))
            .ToList();
    }

    private ClassTask CreateTask(Guid classId, Season season, Observation observation, int chosenCount, int oppositeCount)
    {
        var chosenName = SeasonNames[season];
        var oppositeName = SeasonNames[season.Opposite()];

        return new ClassTask
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            Type = TaskType.SeasonalComparison,
            DifficultyGroup = 3,
            Question = $"Kuriuo sezonu rūšis „{observation.Species}“ šioje vietovėje pastebėta dažniau?",
            CorrectAnswer = chosenCount > oppositeCount ? chosenName : oppositeName,
            Options = new[] { chosenName, oppositeName }.OrderBy(_ => _random.Next()).ToArray(),
            ObservationId = observation.Id
        };
    }
}
