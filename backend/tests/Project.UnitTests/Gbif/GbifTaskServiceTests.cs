using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Project.Api.Data;
using Project.Api.Entities;
using Project.Api.Integrations.Gbif;
using Project.Api.Services;
using Project.UnitTests.Registration;
using Xunit;

namespace Project.UnitTests.Gbif;

public class GbifTaskServiceTests
{
    private const string OwnerId = "teacher-1";
    private const double Latitude = 55.0;
    private const double Longitude = 24.0;

    [Fact]
    public async Task CreateAsync_WithEnoughObservations_SavesLocationSeasonObservationsAndTasks()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var service = CreateService(db, CreateClientWithEnoughObservations());

        var result = await service.CreateAsync(OwnerId, classId, Latitude, Longitude, Season.Summer, CancellationToken.None);

        Assert.Equal(GbifTaskOutcome.Created, result.Outcome);
        var stored = await db.Classes.AsNoTracking().SingleAsync(c => c.Id == classId);
        Assert.Equal(Latitude, stored.Latitude);
        Assert.Equal(Longitude, stored.Longitude);
        Assert.Equal(Season.Summer, stored.Season);
        Assert.Equal(result.CreatedCount, await db.ClassTasks.CountAsync());
        Assert.Equal(22, await db.Observations.CountAsync());
        Assert.Equal(12, await db.Observations.CountAsync(o => o.Season == Season.Summer));
        Assert.Equal(10, await db.Observations.CountAsync(o => o.Season == Season.Winter));
    }

    [Fact]
    public async Task CreateAsync_WithEnoughObservations_CreatesTasksOfAllFourTypes()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var service = CreateService(db, CreateClientWithEnoughObservations());

        await service.CreateAsync(OwnerId, classId, Latitude, Longitude, Season.Summer, CancellationToken.None);

        var types = await db.ClassTasks.Select(t => t.Type).Distinct().ToListAsync();
        Assert.Equal(4, types.Count);
        Assert.InRange(await db.ClassTasks.CountAsync(), 4, 9);
    }

    [Fact]
    public async Task CreateAsync_WithFewerThanTenObservations_SavesNothing()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var client = new FakeGbifClient().With(Season.Summer, Occurrences("Parus major", "Aves", 9, 6));
        var service = CreateService(db, client);

        var result = await service.CreateAsync(OwnerId, classId, Latitude, Longitude, Season.Summer, CancellationToken.None);

        Assert.Equal(GbifTaskOutcome.TooFewObservations, result.Outcome);
        await AssertNothingSavedAsync(db, classId);
    }

    [Fact]
    public async Task CreateAsync_WhenObservationsAreFartherThanTenKilometres_DoesNotCountThem()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var far = Occurrences("Parus major", "Aves", 12, 6, latitude: Latitude + 0.5);
        var service = CreateService(db, new FakeGbifClient().With(Season.Summer, far));

        var result = await service.CreateAsync(OwnerId, classId, Latitude, Longitude, Season.Summer, CancellationToken.None);

        Assert.Equal(GbifTaskOutcome.TooFewObservations, result.Outcome);
    }

    [Fact]
    public async Task CreateAsync_WhenGbifFails_SavesNothing()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var client = CreateClientWithEnoughObservations();
        client.Fails = true;
        var service = CreateService(db, client);

        var result = await service.CreateAsync(OwnerId, classId, Latitude, Longitude, Season.Summer, CancellationToken.None);

        Assert.Equal(GbifTaskOutcome.GbifUnavailable, result.Outcome);
        await AssertNothingSavedAsync(db, classId);
    }

    [Fact]
    public async Task CreateAsync_WhenClassAlreadyHasGbifTasks_ReturnsAlreadyCreatedAndDoesNotCallGbif()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var firstClient = CreateClientWithEnoughObservations();
        await CreateService(db, firstClient).CreateAsync(OwnerId, classId, Latitude, Longitude, Season.Summer, CancellationToken.None);
        var tasksBefore = await db.ClassTasks.CountAsync();
        var secondClient = CreateClientWithEnoughObservations();

        var result = await CreateService(db, secondClient).CreateAsync(OwnerId, classId, Latitude, Longitude, Season.Winter, CancellationToken.None);

        Assert.Equal(GbifTaskOutcome.AlreadyCreated, result.Outcome);
        Assert.Equal(0, secondClient.Calls);
        Assert.Equal(tasksBefore, await db.ClassTasks.CountAsync());
    }

    [Fact]
    public async Task CreateAsync_WhenAnotherRequestFinishesFirst_ReturnsAlreadyCreated()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var slowClient = CreateClientWithEnoughObservations();
        var fired = false;
        slowClient.OnSearch = async () =>
        {
            if (fired) return;
            fired = true;
            using var otherScope = factory.ServiceProvider.CreateScope();
            var otherDb = otherScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await CreateService(otherDb, CreateClientWithEnoughObservations())
                .CreateAsync(OwnerId, classId, Latitude, Longitude, Season.Summer, CancellationToken.None);
        };

        var result = await CreateService(db, slowClient).CreateAsync(OwnerId, classId, Latitude, Longitude, Season.Summer, CancellationToken.None);

        // The in-memory database has no transactions; that nothing is left half-saved is checked with PostgreSQL in the integration tests
        Assert.Equal(GbifTaskOutcome.AlreadyCreated, result.Outcome);
    }

    [Fact]
    public async Task CreateAsync_WhenClassBelongsToAnotherTeacher_ReturnsForbiddenWithoutCallingGbif()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var client = CreateClientWithEnoughObservations();

        var result = await CreateService(db, client).CreateAsync("teacher-2", classId, Latitude, Longitude, Season.Summer, CancellationToken.None);

        Assert.Equal(GbifTaskOutcome.Forbidden, result.Outcome);
        Assert.Equal(0, client.Calls);
        await AssertNothingSavedAsync(db, classId);
    }

    [Fact]
    public async Task CreateAsync_WhenClassDoesNotExist_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var result = await CreateService(db, new FakeGbifClient()).CreateAsync(OwnerId, Guid.NewGuid(), Latitude, Longitude, Season.Summer, CancellationToken.None);

        Assert.Equal(GbifTaskOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task CreateAsync_WhenOppositeSeasonHasFewObservations_CreatesTasksWithoutSeasonalComparison()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var client = new FakeGbifClient().With(Season.Summer, SummerOccurrences());
        var service = CreateService(db, client);

        var result = await service.CreateAsync(OwnerId, classId, Latitude, Longitude, Season.Summer, CancellationToken.None);

        Assert.Equal(GbifTaskOutcome.Created, result.Outcome);
        Assert.False(await db.ClassTasks.AnyAsync(t => t.Type == TaskType.SeasonalComparison));
        Assert.True(await db.ClassTasks.AnyAsync());
    }

    [Fact]
    public async Task CreateAsync_WithObservationOfProtectedSpecies_StoresItsIucnCategory()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var occurrences = SummerOccurrences().Append(
            new GbifOccurrence(9999, "Ciconia nigra", "Aves", "Ciconiidae", Latitude, Longitude, "Miskas", new DateOnly(2025, 6, 1), "Dataset", "CC0_1_0", "VU"));
        var service = CreateService(db, new FakeGbifClient().With(Season.Summer, occurrences));

        await service.CreateAsync(OwnerId, classId, Latitude, Longitude, Season.Summer, CancellationToken.None);

        var stored = await db.Observations.SingleAsync(o => o.GbifKey == 9999);
        Assert.Equal("VU", stored.IucnCategory);
    }

    private static GbifTaskService CreateService(ApplicationDbContext db, IGbifClient client) =>
        new(db, client, new GbifTaskFactory(new Random(1)), new SeasonalTaskFactory(new Random(1)), NullLogger<GbifTaskService>.Instance);

    private static async Task<Guid> AddClassAsync(ApplicationDbContext db)
    {
        var schoolClass = new SchoolClass { Id = Guid.NewGuid(), Name = "7a", Code = "AB12CD", OwnerId = OwnerId, CreatedAtUtc = DateTime.UtcNow };
        db.Classes.Add(schoolClass);
        await db.SaveChangesAsync();
        return schoolClass.Id;
    }

    private static async Task AssertNothingSavedAsync(ApplicationDbContext db, Guid classId)
    {
        db.ChangeTracker.Clear();
        var stored = await db.Classes.AsNoTracking().SingleAsync(c => c.Id == classId);
        Assert.Null(stored.Season);
        Assert.Null(stored.Latitude);
        Assert.Equal(0, await db.Observations.CountAsync());
        Assert.Equal(0, await db.ClassTasks.CountAsync());
    }

    private static FakeGbifClient CreateClientWithEnoughObservations() =>
        new FakeGbifClient()
            .With(Season.Summer, SummerOccurrences())
            .With(Season.Winter, WinterOccurrences());

    // 12 summer observations: Parus major 5, Vulpes vulpes 4, Apis mellifera 3
    private static List<GbifOccurrence> SummerOccurrences() =>
        Occurrences("Parus major", "Aves", 5, 6, 100)
            .Concat(Occurrences("Vulpes vulpes", "Mammalia", 4, 7, 200))
            .Concat(Occurrences("Apis mellifera", "Insecta", 3, 8, 300)).ToList();

    // 10 winter observations: Parus major 3, Vulpes vulpes 6, Apis mellifera 1
    private static List<GbifOccurrence> WinterOccurrences() =>
        Occurrences("Parus major", "Aves", 3, 12, 400)
            .Concat(Occurrences("Vulpes vulpes", "Mammalia", 6, 1, 500))
            .Concat(Occurrences("Apis mellifera", "Insecta", 1, 2, 600)).ToList();

    private static List<GbifOccurrence> Occurrences(string species, string gbifClass, int count, int month, long firstKey = 1, double latitude = Latitude) =>
        Enumerable.Range(0, count)
            .Select(i => new GbifOccurrence(firstKey + i, species, gbifClass, "Family", latitude, Longitude, "Place", new DateOnly(2025, month, 10), "Dataset", "CC0_1_0", null))
            .ToList();
}
