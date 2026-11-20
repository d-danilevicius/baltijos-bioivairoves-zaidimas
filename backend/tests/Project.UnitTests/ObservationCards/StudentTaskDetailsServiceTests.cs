using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.DTOs.Responses;
using Project.Api.Entities;
using Project.Api.Services;
using Project.UnitTests.Registration;
using Xunit;

namespace Project.UnitTests.ObservationCards;

public class StudentTaskDetailsServiceTests
{
    private const string StudentId = "student-1";

    [Fact]
    public async Task GetAsync_ForGbifTask_ReturnsQuestionOptionsAndObservationCard()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var observationId = await AddObservationAsync(db, classId);
        var taskId = await AddTaskAsync(db, classId, 1, observationId: observationId);

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, taskId, CancellationToken.None);

        Assert.Equal(StudentTaskDetailsOutcome.Found, result.Outcome);
        var task = result.Task!;
        Assert.Equal("GBIF", task.Label);
        Assert.Equal("Question", task.Question);
        Assert.Equal(new[] { "A", "B" }, task.Options);
        Assert.Equal("Patvirtintas GBIF stebėjimas", task.Observation!.Label);
    }

    [Fact]
    public async Task GetAsync_ForTeacherTask_ReturnsTitleAndQuestionWithoutObservationCard()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1, source: TaskSource.Teacher, type: TaskType.TeacherTask, title: "Mokyklos kiemas");

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, taskId, CancellationToken.None);

        var task = result.Task!;
        Assert.Equal("Mokytojo užduotis", task.Label);
        Assert.Equal("Mokyklos kiemas", task.Title);
        Assert.Equal("Question", task.Question);
        Assert.Null(task.Observation);
    }

    [Fact]
    public void StudentTaskDetailsResponse_HasNoCorrectAnswer()
    {
        var properties = typeof(StudentTaskDetailsResponse).GetProperties().Select(p => p.Name);

        Assert.DoesNotContain("CorrectAnswer", properties);
    }

    [Fact]
    public async Task GetAsync_ForTaskOfLockedGroup_ReturnsLocked()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        await AddTaskAsync(db, classId, 1);
        var lockedTaskId = await AddTaskAsync(db, classId, 2);

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, lockedTaskId, CancellationToken.None);

        Assert.Equal(StudentTaskDetailsOutcome.Locked, result.Outcome);
        Assert.Null(result.Task);
    }

    [Fact]
    public async Task GetAsync_WhenGroupBeforeHasNoTasks_ReturnsTaskOfGroupTwoAtOnce()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 2);

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, taskId, CancellationToken.None);

        Assert.Equal(StudentTaskDetailsOutcome.Found, result.Outcome);
    }

    [Fact]
    public async Task GetAsync_ForTaskOfAnotherClass_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await AddClassWithMemberAsync(db, StudentId);
        var otherClassId = await AddClassWithMemberAsync(db, "another-student", code: "ZZ99ZZ");
        var otherTaskId = await AddTaskAsync(db, otherClassId, 1);

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, otherTaskId, CancellationToken.None);

        Assert.Equal(StudentTaskDetailsOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task GetAsync_ForMissingTask_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await AddClassWithMemberAsync(db, StudentId);

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(StudentTaskDetailsOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task GetAsync_ForStudentWithoutClass_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, "another-student");
        var taskId = await AddTaskAsync(db, classId, 1);

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, taskId, CancellationToken.None);

        Assert.Equal(StudentTaskDetailsOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task GetAsync_WhenMembershipEnded_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId, endedAtUtc: DateTime.UtcNow);
        var taskId = await AddTaskAsync(db, classId, 1);

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, taskId, CancellationToken.None);

        Assert.Equal(StudentTaskDetailsOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task GetAsync_ForStudentWhoChangedClass_CannotOpenTaskOfTheOldClass()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var oldClassId = await AddClassWithMemberAsync(db, StudentId, endedAtUtc: DateTime.UtcNow);
        var oldTaskId = await AddTaskAsync(db, oldClassId, 1);
        await AddClassWithMemberAsync(db, StudentId, code: "ZZ99ZZ");

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, oldTaskId, CancellationToken.None);

        Assert.Equal(StudentTaskDetailsOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task GetAsync_ForSubmittedTask_ReturnsTheStudentsSubmission()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);
        await AddSubmissionAsync(db, taskId, StudentId, "A", "Mačiau prie ežero.");

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, taskId, CancellationToken.None);

        var submission = result.Task!.Submission!;
        Assert.Equal("A", submission.Answer);
        Assert.Equal("Mačiau prie ežero.", submission.Evidence);
        Assert.Equal("Mokinio pateikti duomenys", submission.Label);
    }

    [Fact]
    public async Task GetAsync_ForTaskThatIsNotSubmitted_ReturnsNoSubmission()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, taskId, CancellationToken.None);

        Assert.Null(result.Task!.Submission);
    }

    [Fact]
    public async Task GetAsync_WhenAnotherStudentSubmittedTheTask_DoesNotReturnTheirSubmission()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);
        await AddSubmissionAsync(db, taskId, "another-student", "B", "Kito mokinio įrodymas");

        var result = await new StudentTaskDetailsService(db).GetAsync(StudentId, taskId, CancellationToken.None);

        Assert.Null(result.Task!.Submission);
    }

    private static async Task AddSubmissionAsync(ApplicationDbContext db, Guid taskId, string studentId, string answer, string evidence)
    {
        db.Submissions.Add(new Submission
        {
            Id = Guid.NewGuid(),
            ClassTaskId = taskId,
            StudentId = studentId,
            Answer = answer,
            Evidence = evidence,
            SubmittedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    private static async Task<Guid> AddClassWithMemberAsync(
        ApplicationDbContext db,
        string studentId,
        string code = "AB12CD",
        DateTime? endedAtUtc = null)
    {
        var schoolClass = new SchoolClass { Id = Guid.NewGuid(), Name = "7a", Code = code, OwnerId = "teacher-1", CreatedAtUtc = DateTime.UtcNow };
        db.Classes.Add(schoolClass);
        db.ClassMemberships.Add(new ClassMembership
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ClassId = schoolClass.Id,
            StartedAtUtc = DateTime.UtcNow,
            EndedAtUtc = endedAtUtc
        });
        await db.SaveChangesAsync();
        return schoolClass.Id;
    }

    private static async Task<Guid> AddObservationAsync(ApplicationDbContext db, Guid classId)
    {
        var observation = new Observation
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            GbifKey = 4242,
            Species = "Parus major",
            Latitude = 54.7,
            Longitude = 25.3,
            Locality = "Vilnius",
            ObservedOn = new DateOnly(2026, 5, 12),
            DatasetName = "iNaturalist",
            License = "CC_BY_4_0"
        };
        db.Observations.Add(observation);
        await db.SaveChangesAsync();
        return observation.Id;
    }

    private static async Task<Guid> AddTaskAsync(
        ApplicationDbContext db,
        Guid classId,
        int group,
        TaskSource source = TaskSource.Gbif,
        TaskType type = TaskType.CommonSpecies,
        string? title = null,
        Guid? observationId = null)
    {
        var task = new ClassTask
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            Type = type,
            Source = source,
            DifficultyGroup = group,
            Title = title,
            Question = "Question",
            CorrectAnswer = "Answer",
            Options = new[] { "A", "B" },
            ObservationId = observationId
        };
        db.ClassTasks.Add(task);
        await db.SaveChangesAsync();
        return task.Id;
    }
}
