using Project.Api.Entities;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Gbif;

public class GbifTaskFactoryTests
{
    private static readonly string[] ForbiddenWords = { "rink", "gaud", "pagauk", "palies", "trikdy", "nuskink", "paimk" };

    private readonly GbifTaskFactory _factory = new(new Random(1));

    [Fact]
    public void Create_WithSeveralGroups_CreatesRecognitionTasksWithTheCorrectGroupAmongOptions()
    {
        var observations = MixedObservations();

        var tasks = _factory.Create(TestObservations.ClassId, observations)
            .Where(t => t.Type == TaskType.RecogniseSpeciesGroup).ToList();

        Assert.Equal(GbifTaskFactory.RecognitionTaskCount, tasks.Count);
        Assert.All(tasks, t =>
        {
            Assert.Equal(1, t.DifficultyGroup);
            Assert.NotNull(t.Options);
            Assert.Contains(t.CorrectAnswer, t.Options);
            Assert.InRange(t.Options.Length, 2, 4);
            Assert.Equal(t.Options.Length, t.Options.Distinct().Count());
        });
        Assert.Equal(tasks.Count, tasks.Select(t => t.ObservationId).Distinct().Count());
    }

    [Fact]
    public void Create_RecognitionAnswerIsTheLithuanianNameOfTheGbifClass()
    {
        var observations = new List<Observation>
        {
            TestObservations.Create("Parus major", "Aves"),
            TestObservations.Create("Vulpes vulpes", "Mammalia")
        };

        var tasks = _factory.Create(TestObservations.ClassId, observations)
            .Where(t => t.Type == TaskType.RecogniseSpeciesGroup).ToList();

        Assert.All(tasks, t =>
        {
            var expected = t.Question.Contains("Parus major") ? "Paukščiai" : "Žinduoliai";
            Assert.Equal(expected, t.CorrectAnswer);
        });
    }

    [Fact]
    public void Create_WhenAllObservationsHaveOneGroup_SkipsRecognitionTasks()
    {
        var observations = TestObservations.Many("Parus major", 12, "Aves");

        var tasks = _factory.Create(TestObservations.ClassId, observations);

        Assert.DoesNotContain(tasks, t => t.Type == TaskType.RecogniseSpeciesGroup);
    }

    [Fact]
    public void Create_CommonSpeciesTaskAnswerIsTheMostFrequentSpecies()
    {
        var observations = TestObservations.Many("Parus major", 6)
            .Concat(TestObservations.Many("Erithacus rubecula", 3))
            .Concat(TestObservations.Many("Turdus merula", 2)).ToList();

        var task = Assert.Single(_factory.Create(TestObservations.ClassId, observations), t => t.Type == TaskType.CommonSpecies);

        Assert.Equal("Parus major", task.CorrectAnswer);
        Assert.Equal(2, task.DifficultyGroup);
        Assert.NotNull(task.Options);
        Assert.Contains("Parus major", task.Options);
        Assert.Equal(3, task.Options.Length);
    }

    [Fact]
    public void Create_WhenTwoSpeciesShareTheHighestCount_SkipsCommonSpeciesTask()
    {
        var observations = TestObservations.Many("Parus major", 5)
            .Concat(TestObservations.Many("Erithacus rubecula", 5)).ToList();

        var tasks = _factory.Create(TestObservations.ClassId, observations);

        Assert.DoesNotContain(tasks, t => t.Type == TaskType.CommonSpecies);
    }

    [Fact]
    public void Create_WhenThereIsOneSpecies_SkipsCommonSpeciesTask()
    {
        var tasks = _factory.Create(TestObservations.ClassId, TestObservations.Many("Parus major", 12));

        Assert.DoesNotContain(tasks, t => t.Type == TaskType.CommonSpecies);
    }

    [Fact]
    public void Create_ObservationQuestionAnswerIsTheMonthOfTheObservation()
    {
        var observations = new List<Observation>
        {
            TestObservations.Create("Parus major", month: 6),
            TestObservations.Create("Erithacus rubecula", month: 7),
            TestObservations.Create("Turdus merula", month: 8)
        };

        var tasks = _factory.Create(TestObservations.ClassId, observations)
            .Where(t => t.Type == TaskType.ObservationQuestion).ToList();

        Assert.Equal(3, tasks.Count);
        Assert.All(tasks, t =>
        {
            var observation = observations.Single(o => o.Id == t.ObservationId);
            Assert.Equal(observation.ObservedOn!.Value.Month.ToString(), t.CorrectAnswer);
            Assert.Equal(1, t.DifficultyGroup);
        });
    }

    [Fact]
    public void Create_ObservationQuestionIgnoresObservationsWithoutDate()
    {
        var observations = TestObservations.Many("Parus major", 12, month: null);

        var tasks = _factory.Create(TestObservations.ClassId, observations);

        Assert.DoesNotContain(tasks, t => t.Type == TaskType.ObservationQuestion);
    }

    [Fact]
    public void Create_WithManyObservations_CreatesAtMostNineTasks()
    {
        var tasks = _factory.Create(TestObservations.ClassId, MixedObservations());

        Assert.InRange(tasks.Count, 1, 9);
    }

    [Fact]
    public void Create_TaskTextsDoNotAskToCollectOrDisturbOrganisms()
    {
        var tasks = _factory.Create(TestObservations.ClassId, MixedObservations());

        Assert.All(tasks, t =>
            Assert.DoesNotContain(ForbiddenWords, w => t.Question.Contains(w, StringComparison.OrdinalIgnoreCase)));
    }

    private static List<Observation> MixedObservations() =>
        TestObservations.Many("Parus major", 6, "Aves", 6)
            .Concat(TestObservations.Many("Vulpes vulpes", 3, "Mammalia", 7))
            .Concat(TestObservations.Many("Apis mellifera", 2, "Insecta", 8))
            .Concat(TestObservations.Many("Rana temporaria", 2, "Amphibia", 6))
            .Concat(TestObservations.Many("Bellis perennis", 2, "Magnoliopsida", 7))
            .ToList();
}
