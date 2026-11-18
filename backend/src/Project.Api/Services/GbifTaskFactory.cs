using System.Globalization;
using Project.Api.Entities;

namespace Project.Api.Services;

public class GbifTaskFactory
{
    public const int RecognitionTaskCount = 3;
    public const int ObservationQuestionCount = 3;
    private const int MaxOptions = 4;

    private readonly Random _random;

    public GbifTaskFactory(Random random)
    {
        _random = random;
    }

    // Creates the tasks that use the observations of the chosen season; a task type that cannot be made is skipped
    public IReadOnlyList<ClassTask> Create(Guid classId, IReadOnlyList<Observation> observations)
    {
        var tasks = new List<ClassTask>();
        tasks.AddRange(CreateRecognitionTasks(classId, observations));
        tasks.AddRange(CreateCommonSpeciesTask(classId, observations));
        tasks.AddRange(CreateObservationQuestions(classId, observations));
        return tasks;
    }

    private IEnumerable<ClassTask> CreateRecognitionTasks(Guid classId, IReadOnlyList<Observation> observations)
    {
        var withGroup = observations
            .Where(o => !string.IsNullOrWhiteSpace(o.GbifClass))
            .Select(o => (Observation: o, Group: SpeciesGroupName.For(o.GbifClass ?? string.Empty)))
            .ToList();
        var groups = withGroup.Select(o => o.Group).Distinct().ToList();
        if (groups.Count < 2)
        {
            return Enumerable.Empty<ClassTask>();
        }

        return Shuffle(withGroup)
            .DistinctBy(o => o.Observation.Species)
            .Take(RecognitionTaskCount)
            .Select(o => CreateRecognitionTask(classId, o.Observation, o.Group, groups))
            .ToList();
    }

    private ClassTask CreateRecognitionTask(Guid classId, Observation observation, string correct, List<string> groups)
    {
        var others = Shuffle(groups.Where(g => g != correct)).Take(MaxOptions - 1);

        return new ClassTask
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            Type = TaskType.RecogniseSpeciesGroup,
            DifficultyGroup = 1,
            Question = $"Kuriai rūšių grupei priklauso „{observation.Species}“?",
            CorrectAnswer = correct,
            Options = Shuffle(others.Append(correct)).ToArray(),
            ObservationId = observation.Id
        };
    }

    private IEnumerable<ClassTask> CreateCommonSpeciesTask(Guid classId, IReadOnlyList<Observation> observations)
    {
        var counts = observations
            .GroupBy(o => o.Species)
            .Select(g => (Species: g.Key, Count: g.Count(), First: g.First()))
            .OrderByDescending(s => s.Count)
            .ThenBy(s => s.Species, StringComparer.Ordinal)
            .ToList();

        // Two species with the same highest count have no single correct answer
        if (counts.Count < 2 || counts[0].Count == counts[1].Count)
        {
            yield break;
        }

        var correct = counts[0];
        var others = Shuffle(counts.Skip(1).Select(s => s.Species)).Take(MaxOptions - 1);

        yield return new ClassTask
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            Type = TaskType.CommonSpecies,
            DifficultyGroup = 2,
            Question = "Kuri rūšis šioje vietovėje pastebėta dažniausiai?",
            CorrectAnswer = correct.Species,
            Options = Shuffle(others.Append(correct.Species)).ToArray(),
            ObservationId = correct.First.Id
        };
    }

    private IEnumerable<ClassTask> CreateObservationQuestions(Guid classId, IReadOnlyList<Observation> observations)
    {
        var dated = observations
            .Where(o => o.ObservedOn is not null)
            .Select(o => (Observation: o, Month: o.ObservedOn.GetValueOrDefault().Month));

        return Shuffle(dated)
            .DistinctBy(o => o.Observation.Species)
            .Take(ObservationQuestionCount)
            .Select(o => new ClassTask
            {
                Id = Guid.NewGuid(),
                ClassId = classId,
                Type = TaskType.ObservationQuestion,
                DifficultyGroup = 1,
                Question = $"Kurį mėnesį pastebėta rūšis „{o.Observation.Species}“? Įveskite mėnesio numerį (1–12).",
                CorrectAnswer = o.Month.ToString(CultureInfo.InvariantCulture),
                ObservationId = o.Observation.Id
            })
            .ToList();
    }

    private List<T> Shuffle<T>(IEnumerable<T> items) => items.OrderBy(_ => _random.Next()).ToList();
}
