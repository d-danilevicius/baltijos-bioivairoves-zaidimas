using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.Entities;
using Project.Api.Services;
using Project.UnitTests.Registration;
using Xunit;

namespace Project.UnitTests.Gbif;

public class ClassTaskServiceTests
{
    [Fact]
    public async Task ListAsync_ForOwner_ReturnsTasksOrderedByGroupWithGbifLabel()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithTasksAsync(db);

        var result = await new ClassTaskService(db).ListAsync("teacher-1", classId, CancellationToken.None);

        Assert.Equal(ClassOutcome.Succeeded, result.Outcome);
        Assert.Equal(new[] { 1, 2, 3 }, result.Tasks.Select(t => t.DifficultyGroup));
        Assert.All(result.Tasks, t => Assert.Equal("GBIF", t.Label));
    }

    [Fact]
    public async Task ListAsync_ForAnotherTeacher_ReturnsForbidden()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithTasksAsync(db);

        var result = await new ClassTaskService(db).ListAsync("teacher-2", classId, CancellationToken.None);

        Assert.Equal(ClassOutcome.Forbidden, result.Outcome);
        Assert.Empty(result.Tasks);
    }

    [Fact]
    public async Task ListAsync_ForMissingClass_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var result = await new ClassTaskService(db).ListAsync("teacher-1", Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(ClassOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task ListAsync_ForClassWithoutTasks_ReturnsEmptyList()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var schoolClass = new SchoolClass { Id = Guid.NewGuid(), Name = "7b", Code = "ZZ99ZZ", OwnerId = "teacher-1", CreatedAtUtc = DateTime.UtcNow };
        db.Classes.Add(schoolClass);
        await db.SaveChangesAsync();

        var result = await new ClassTaskService(db).ListAsync("teacher-1", schoolClass.Id, CancellationToken.None);

        Assert.Equal(ClassOutcome.Succeeded, result.Outcome);
        Assert.Empty(result.Tasks);
    }

    [Fact]
    public async Task ListAsync_WithTeacherAndGbifTasks_LabelsEachTaskBySource()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithTasksAsync(db);
        var teacherTask = CreateTask(classId, 1, TaskType.TeacherTask);
        teacherTask.Source = TaskSource.Teacher;
        teacherTask.Title = "Mokyklos kiemas";
        db.ClassTasks.Add(teacherTask);
        await db.SaveChangesAsync();

        var result = await new ClassTaskService(db).ListAsync("teacher-1", classId, CancellationToken.None);

        Assert.Equal("Mokytojo užduotis", Assert.Single(result.Tasks, t => t.Id == teacherTask.Id).Label);
        Assert.Equal(3, result.Tasks.Count(t => t.Label == "GBIF"));
    }

    private static async Task<Guid> AddClassWithTasksAsync(ApplicationDbContext db)
    {
        var schoolClass = new SchoolClass { Id = Guid.NewGuid(), Name = "7a", Code = "AB12CD", OwnerId = "teacher-1", CreatedAtUtc = DateTime.UtcNow };
        db.Classes.Add(schoolClass);
        db.ClassTasks.AddRange(
            CreateTask(schoolClass.Id, 3, TaskType.SeasonalComparison),
            CreateTask(schoolClass.Id, 1, TaskType.ObservationQuestion),
            CreateTask(schoolClass.Id, 2, TaskType.CommonSpecies));
        await db.SaveChangesAsync();
        return schoolClass.Id;
    }

    private static ClassTask CreateTask(Guid classId, int group, TaskType type) => new()
    {
        Id = Guid.NewGuid(),
        ClassId = classId,
        Type = type,
        DifficultyGroup = group,
        Question = "Klausimas",
        CorrectAnswer = "Atsakymas"
    };
}
