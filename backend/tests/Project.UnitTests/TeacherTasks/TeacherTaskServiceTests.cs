using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;
using Project.Api.Services;
using Project.UnitTests.Registration;
using Xunit;

namespace Project.UnitTests.TeacherTasks;

public class TeacherTaskServiceTests
{
    private const string OwnerId = "teacher-1";

    [Fact]
    public async Task CreateAsync_ForOwner_StoresTeacherTaskWithEnteredValues()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);

        var result = await new TeacherTaskService(db).CreateAsync(OwnerId, classId, ValidRequest(), CancellationToken.None);

        Assert.Equal(TeacherTaskOutcome.Created, result.Outcome);
        var task = await db.ClassTasks.SingleAsync();
        Assert.Equal(classId, task.ClassId);
        Assert.Equal(TaskSource.Teacher, task.Source);
        Assert.Equal(TaskType.TeacherTask, task.Type);
        Assert.Equal("Mokyklos kiemas", task.Title);
        Assert.Equal("Kokį paukštį matote?", task.Question);
        Assert.Equal("Žvirblis", task.CorrectAnswer);
        Assert.Equal(2, task.DifficultyGroup);
        Assert.Null(task.ObservationId);
    }

    [Fact]
    public async Task CreateAsync_ForOwner_ReturnsTaskWithTeacherLabel()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);

        var result = await new TeacherTaskService(db).CreateAsync(OwnerId, classId, ValidRequest(), CancellationToken.None);

        Assert.Equal("Mokytojo užduotis", result.Task?.Label);
        Assert.Equal(TaskType.TeacherTask, result.Task?.Type);
    }

    [Fact]
    public async Task CreateAsync_ForAnotherTeacher_ReturnsForbiddenAndSavesNothing()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);

        var result = await new TeacherTaskService(db).CreateAsync("teacher-2", classId, ValidRequest(), CancellationToken.None);

        Assert.Equal(TeacherTaskOutcome.Forbidden, result.Outcome);
        Assert.Empty(db.ClassTasks);
    }

    [Fact]
    public async Task CreateAsync_ForMissingClass_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var result = await new TeacherTaskService(db).CreateAsync(OwnerId, Guid.NewGuid(), ValidRequest(), CancellationToken.None);

        Assert.Equal(TeacherTaskOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task DeleteAsync_ForOwnTeacherTask_RemovesTheTask()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var task = await AddTaskAsync(db, classId, TaskSource.Teacher);

        var result = await new TeacherTaskService(db).DeleteAsync(OwnerId, classId, task.Id, CancellationToken.None);

        Assert.Equal(TeacherTaskOutcome.Deleted, result.Outcome);
        Assert.Empty(db.ClassTasks);
    }

    [Fact]
    public async Task DeleteAsync_ForAnotherTeacher_ReturnsForbiddenAndKeepsTheTask()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var task = await AddTaskAsync(db, classId, TaskSource.Teacher);

        var result = await new TeacherTaskService(db).DeleteAsync("teacher-2", classId, task.Id, CancellationToken.None);

        Assert.Equal(TeacherTaskOutcome.Forbidden, result.Outcome);
        Assert.Single(db.ClassTasks);
    }

    [Fact]
    public async Task DeleteAsync_ForTaskOfAnotherClass_ReturnsNotFoundAndKeepsTheTask()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var otherClassId = await AddClassAsync(db, "CD34EF");
        var task = await AddTaskAsync(db, otherClassId, TaskSource.Teacher);

        var result = await new TeacherTaskService(db).DeleteAsync(OwnerId, classId, task.Id, CancellationToken.None);

        Assert.Equal(TeacherTaskOutcome.NotFound, result.Outcome);
        Assert.Single(db.ClassTasks);
    }

    [Fact]
    public async Task DeleteAsync_ForGbifTask_ReturnsNotTeacherTaskAndKeepsTheTask()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);
        var task = await AddTaskAsync(db, classId, TaskSource.Gbif);

        var result = await new TeacherTaskService(db).DeleteAsync(OwnerId, classId, task.Id, CancellationToken.None);

        Assert.Equal(TeacherTaskOutcome.NotTeacherTask, result.Outcome);
        Assert.Single(db.ClassTasks);
    }

    [Fact]
    public async Task DeleteAsync_ForMissingTask_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassAsync(db);

        var result = await new TeacherTaskService(db).DeleteAsync(OwnerId, classId, Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(TeacherTaskOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task DeleteAsync_ForMissingClass_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var result = await new TeacherTaskService(db).DeleteAsync(OwnerId, Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(TeacherTaskOutcome.NotFound, result.Outcome);
    }

    private static CreateTeacherTaskRequest ValidRequest() => new()
    {
        Title = "  Mokyklos kiemas ",
        Question = "Kokį paukštį matote?",
        CorrectAnswer = "Žvirblis",
        DifficultyGroup = 2,
        NoOrganismDisturbanceConfirmed = true
    };

    private static async Task<Guid> AddClassAsync(ApplicationDbContext db, string code = "AB12CD")
    {
        var schoolClass = new SchoolClass { Id = Guid.NewGuid(), Name = "7a", Code = code, OwnerId = OwnerId, CreatedAtUtc = DateTime.UtcNow };
        db.Classes.Add(schoolClass);
        await db.SaveChangesAsync();
        return schoolClass.Id;
    }

    private static async Task<ClassTask> AddTaskAsync(ApplicationDbContext db, Guid classId, TaskSource source)
    {
        var task = new ClassTask
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            Source = source,
            Type = source == TaskSource.Teacher ? TaskType.TeacherTask : TaskType.CommonSpecies,
            DifficultyGroup = 1,
            Question = "Klausimas",
            CorrectAnswer = "Atsakymas"
        };
        db.ClassTasks.Add(task);
        await db.SaveChangesAsync();
        return task;
    }
}
