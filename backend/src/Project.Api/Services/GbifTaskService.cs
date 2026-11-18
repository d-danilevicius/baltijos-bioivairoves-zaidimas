using Microsoft.EntityFrameworkCore;
using Npgsql;
using Project.Api.Data;
using Project.Api.Entities;
using Project.Api.Integrations.Gbif;

namespace Project.Api.Services;

public class GbifTaskService : IGbifTaskService
{
    private const string ObservationIndexName = "IX_Observations_ClassId_GbifKey";

    private readonly ApplicationDbContext _dbContext;
    private readonly IGbifClient _gbifClient;
    private readonly GbifTaskFactory _taskFactory;
    private readonly SeasonalTaskFactory _seasonalTaskFactory;
    private readonly ILogger<GbifTaskService> _logger;

    public GbifTaskService(
        ApplicationDbContext dbContext,
        IGbifClient gbifClient,
        GbifTaskFactory taskFactory,
        SeasonalTaskFactory seasonalTaskFactory,
        ILogger<GbifTaskService> logger)
    {
        _dbContext = dbContext;
        _gbifClient = gbifClient;
        _taskFactory = taskFactory;
        _seasonalTaskFactory = seasonalTaskFactory;
        _logger = logger;
    }

    public async Task<GbifTaskResult> CreateAsync(
        string ownerId,
        Guid classId,
        double latitude,
        double longitude,
        Season season,
        CancellationToken cancellationToken)
    {
        var schoolClass = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == classId, cancellationToken);
        if (schoolClass is null)
        {
            return GbifTaskResult.Failed(GbifTaskOutcome.NotFound);
        }

        if (schoolClass.OwnerId != ownerId)
        {
            return GbifTaskResult.Failed(GbifTaskOutcome.Forbidden);
        }

        // VT-010: a class has one location and one season
        if (schoolClass.Season is not null)
        {
            return GbifTaskResult.Failed(GbifTaskOutcome.AlreadyCreated);
        }

        var observations = await LoadObservationsAsync(classId, latitude, longitude, season, cancellationToken);
        if (observations is null)
        {
            return GbifTaskResult.Failed(GbifTaskOutcome.GbifUnavailable);
        }

        var tasks = CreateTasks(classId, season, observations);
        if (observations.Chosen.Count < GbifObservationSelector.MinObservations || tasks.Count == 0)
        {
            return GbifTaskResult.Failed(GbifTaskOutcome.TooFewObservations);
        }

        return await SaveAsync(schoolClass, latitude, longitude, season, observations, tasks, cancellationToken);
    }

    // Returns null when GBIF is not available (NFR-020)
    private async Task<GbifObservations?> LoadObservationsAsync(
        Guid classId,
        double latitude,
        double longitude,
        Season season,
        CancellationToken cancellationToken)
    {
        try
        {
            // One request after another: parallel requests made GBIF answer 429 (Too Many Requests)
            var chosen = await _gbifClient.SearchAsync(latitude, longitude, GbifObservationSelector.MaxDistanceKm, season, cancellationToken);
            var opposite = await _gbifClient.SearchAsync(latitude, longitude, GbifObservationSelector.MaxDistanceKm, season.Opposite(), cancellationToken);

            return new GbifObservations(
                ToObservations(chosen, latitude, longitude, classId, season),
                ToObservations(opposite, latitude, longitude, classId, season.Opposite()));
        }
        catch (GbifUnavailableException ex)
        {
            _logger.LogWarning(ex, "GBIF tasks of class {ClassId} were not created because GBIF is not available", classId);
            return null;
        }
    }

    private static List<Observation> ToObservations(
        IEnumerable<GbifOccurrence> occurrences,
        double latitude,
        double longitude,
        Guid classId,
        Season season) =>
        GbifObservationSelector.Select(occurrences, latitude, longitude)
            .Select(o => o.ToObservation(classId, season))
            .ToList();

    private List<ClassTask> CreateTasks(Guid classId, Season season, GbifObservations observations)
    {
        var tasks = new List<ClassTask>();
        tasks.AddRange(_taskFactory.Create(classId, observations.Chosen));
        tasks.AddRange(_seasonalTaskFactory.Create(classId, season, observations.Chosen, observations.Opposite));
        return tasks;
    }

    // Everything is saved in one SaveChanges, so nothing stays when it fails (NFR-020)
    private async Task<GbifTaskResult> SaveAsync(
        SchoolClass schoolClass,
        double latitude,
        double longitude,
        Season season,
        GbifObservations observations,
        List<ClassTask> tasks,
        CancellationToken cancellationToken)
    {
        schoolClass.Latitude = latitude;
        schoolClass.Longitude = longitude;
        schoolClass.Season = season;
        _dbContext.Observations.AddRange(observations.Chosen);
        _dbContext.Observations.AddRange(observations.Opposite);
        _dbContext.ClassTasks.AddRange(tasks);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsCreatedByAnotherRequest(ex))
        {
            // Another request created the tasks of the class first; this save was rolled back
            return GbifTaskResult.Failed(GbifTaskOutcome.AlreadyCreated);
        }

        return GbifTaskResult.Created(tasks.Count);
    }

    // The season of the class was already set (concurrency token) or the same GBIF records were already stored for the class
    private static bool IsCreatedByAnotherRequest(DbUpdateException exception) =>
        exception is DbUpdateConcurrencyException
        || exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: ObservationIndexName };

    private sealed record GbifObservations(List<Observation> Chosen, List<Observation> Opposite);
}
