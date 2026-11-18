using Project.Api.Entities;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Gbif;

public class SeasonalTaskFactoryTests
{
    private readonly SeasonalTaskFactory _factory = new(new Random(1));

    [Fact]
    public void Create_WhenSpeciesIsMoreFrequentInTheChosenSeason_AnswerIsTheChosenSeason()
    {
        var chosen = TestObservations.Many("Parus major", 6);
        var opposite = TestObservations.Many("Parus major", 3, season: Season.Winter)
            .Concat(TestObservations.Many("Turdus merula", 8, season: Season.Winter)).ToList();

        var task = Assert.Single(_factory.Create(TestObservations.ClassId, Season.Summer, chosen, opposite));

        Assert.Equal(TaskType.SeasonalComparison, task.Type);
        Assert.Equal(3, task.DifficultyGroup);
        Assert.Equal("Vasara", task.CorrectAnswer);
        Assert.Equal(new[] { "Vasara", "Žiema" }, task.Options!.OrderBy(o => o));
    }

    [Fact]
    public void Create_WhenSpeciesIsMoreFrequentInTheOppositeSeason_AnswerIsTheOppositeSeason()
    {
        var chosen = TestObservations.Many("Parus major", 2);
        var opposite = TestObservations.Many("Parus major", 10, season: Season.Winter);

        var task = Assert.Single(_factory.Create(TestObservations.ClassId, Season.Summer, chosen, opposite));

        Assert.Equal("Žiema", task.CorrectAnswer);
    }

    [Fact]
    public void Create_WhenOppositeSeasonHasFewerThanTenObservations_CreatesNoTasks()
    {
        var chosen = TestObservations.Many("Parus major", 6);
        var opposite = TestObservations.Many("Parus major", 9, season: Season.Winter);

        Assert.Empty(_factory.Create(TestObservations.ClassId, Season.Summer, chosen, opposite));
    }

    [Fact]
    public void Create_WhenCountsAreEqual_SkipsTheSpecies()
    {
        var chosen = TestObservations.Many("Parus major", 5);
        var opposite = TestObservations.Many("Parus major", 5, season: Season.Winter)
            .Concat(TestObservations.Many("Turdus merula", 5, season: Season.Winter)).ToList();

        Assert.Empty(_factory.Create(TestObservations.ClassId, Season.Summer, chosen, opposite));
    }

    [Fact]
    public void Create_WhenSpeciesIsMissingInTheOppositeSeason_SkipsTheSpecies()
    {
        var chosen = TestObservations.Many("Parus major", 5);
        var opposite = TestObservations.Many("Turdus merula", 12, season: Season.Winter);

        Assert.Empty(_factory.Create(TestObservations.ClassId, Season.Summer, chosen, opposite));
    }

    [Fact]
    public void Create_WithManySpecies_CreatesAtMostTwoTasks()
    {
        var species = new[] { "A a", "B b", "C c", "D d" };
        var chosen = species.SelectMany(s => TestObservations.Many(s, 4)).ToList();
        var opposite = species.SelectMany(s => TestObservations.Many(s, 3, season: Season.Winter)).ToList();

        var tasks = _factory.Create(TestObservations.ClassId, Season.Summer, chosen, opposite);

        Assert.Equal(SeasonalTaskFactory.TaskCount, tasks.Count);
    }
}
