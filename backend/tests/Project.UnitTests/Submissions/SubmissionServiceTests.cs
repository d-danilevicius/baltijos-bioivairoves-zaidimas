using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;
using Project.Api.Services;
using Project.UnitTests.Registration;
using Xunit;

namespace Project.UnitTests.Submissions;

public class SubmissionServiceTests
{
    private const string StudentId = "student-1";

    [Fact]
    public async Task CreateAsync_WithChosenOption_SavesTheSubmissionAndReturnsIt()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);

        var result = await CreateAsync(db, StudentId, taskId, "A", "Mačiau prie ežero.");

        Assert.Equal(SubmissionOutcome.Created, result.Outcome);
        Assert.Equal("Mokinio pateikti duomenys", result.Submission!.Label);
        Assert.Equal(taskId, result.Submission.TaskId);
        var saved = await db.Submissions.SingleAsync();
        Assert.Equal(StudentId, saved.StudentId);
        Assert.Equal(taskId, saved.ClassTaskId);
        Assert.Equal("A", saved.Answer);
        Assert.Equal("Mačiau prie ežero.", saved.Evidence);
    }

    [Fact]
    public async Task CreateAsync_WithAnswerThatIsNotAnOption_ReturnsInvalidAnswerAndSavesNothing()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);

        var result = await CreateAsync(db, StudentId, taskId, "C", "Įrodymas");

        Assert.Equal(SubmissionOutcome.InvalidAnswer, result.Outcome);
        Assert.Empty(db.Submissions);
    }

    [Fact]
    public async Task CreateAsync_ForTaskWithoutOptions_AcceptsTheTypedAnswer()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1, hasOptions: false);

        var result = await CreateAsync(db, StudentId, taskId, "5", "Mačiau gegužę.");

        Assert.Equal(SubmissionOutcome.Created, result.Outcome);
        Assert.Equal("5", (await db.Submissions.SingleAsync()).Answer);
    }

    [Fact]
    public async Task CreateAsync_WhenTaskIsAlreadySubmitted_ReturnsAlreadySubmittedAndKeepsTheFirstAnswer()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);
        await CreateAsync(db, StudentId, taskId, "A", "Pirmas įrodymas");

        var result = await CreateAsync(db, StudentId, taskId, "B", "Antras įrodymas");

        Assert.Equal(SubmissionOutcome.AlreadySubmitted, result.Outcome);
        var saved = await db.Submissions.SingleAsync();
        Assert.Equal("A", saved.Answer);
        Assert.Equal("Pirmas įrodymas", saved.Evidence);
    }

    [Fact]
    public async Task CreateAsync_WhenTaskIsAlreadySubmittedAndAnswerIsNotAnOption_ReturnsAlreadySubmitted()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);
        await CreateAsync(db, StudentId, taskId, "A", "Pirmas įrodymas");

        var result = await CreateAsync(db, StudentId, taskId, "C", "Antras įrodymas");

        Assert.Equal(SubmissionOutcome.AlreadySubmitted, result.Outcome);
        Assert.Equal("A", (await db.Submissions.SingleAsync()).Answer);
    }

    [Fact]
    public async Task CreateAsync_WhenAnotherStudentSubmittedTheSameTask_SavesTheSubmission()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        await AddMemberAsync(db, classId, "student-2");
        var taskId = await AddTaskAsync(db, classId, 1);
        await CreateAsync(db, "student-2", taskId, "A", "Kito mokinio įrodymas");

        var result = await CreateAsync(db, StudentId, taskId, "B", "Mano įrodymas");

        Assert.Equal(SubmissionOutcome.Created, result.Outcome);
        Assert.Equal(2, await db.Submissions.CountAsync());
    }

    [Fact]
    public async Task CreateAsync_ForTaskOfLockedGroup_ReturnsLockedAndSavesNothing()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        await AddTaskAsync(db, classId, 1);
        var lockedTaskId = await AddTaskAsync(db, classId, 2);

        var result = await CreateAsync(db, StudentId, lockedTaskId, "A", "Įrodymas");

        Assert.Equal(SubmissionOutcome.Locked, result.Outcome);
        Assert.Empty(db.Submissions);
    }

    [Fact]
    public async Task CreateAsync_ForTaskOfAnotherClass_ReturnsNotFoundAndSavesNothing()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await AddClassWithMemberAsync(db, StudentId);
        var otherClassId = await AddClassWithMemberAsync(db, "another-student", "ZZ99ZZ");
        var otherTaskId = await AddTaskAsync(db, otherClassId, 1);

        var result = await CreateAsync(db, StudentId, otherTaskId, "A", "Įrodymas");

        Assert.Equal(SubmissionOutcome.NotFound, result.Outcome);
        Assert.Empty(db.Submissions);
    }

    [Fact]
    public async Task CreateAsync_ForMissingTask_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await AddClassWithMemberAsync(db, StudentId);

        var result = await CreateAsync(db, StudentId, Guid.NewGuid(), "A", "Įrodymas");

        Assert.Equal(SubmissionOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task CreateAsync_ForStudentWithoutClass_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, "another-student");
        var taskId = await AddTaskAsync(db, classId, 1);

        var result = await CreateAsync(db, StudentId, taskId, "A", "Įrodymas");

        Assert.Equal(SubmissionOutcome.NotFound, result.Outcome);
        Assert.Empty(db.Submissions);
    }

    [Fact]
    public async Task CreateAsync_WhenMembershipEnded_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId, endedAtUtc: DateTime.UtcNow);
        var taskId = await AddTaskAsync(db, classId, 1);

        var result = await CreateAsync(db, StudentId, taskId, "A", "Įrodymas");

        Assert.Equal(SubmissionOutcome.NotFound, result.Outcome);
        Assert.Empty(db.Submissions);
    }

    [Fact]
    public async Task CreateAsync_ForStudentWhoChangedClass_CannotSubmitTaskOfTheOldClass()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var oldClassId = await AddClassWithMemberAsync(db, StudentId, endedAtUtc: DateTime.UtcNow);
        var oldTaskId = await AddTaskAsync(db, oldClassId, 1);
        await AddClassWithMemberAsync(db, StudentId, "ZZ99ZZ");

        var result = await CreateAsync(db, StudentId, oldTaskId, "A", "Įrodymas");

        Assert.Equal(SubmissionOutcome.NotFound, result.Outcome);
        Assert.Empty(db.Submissions);
    }

    [Fact]
    public async Task CreateAsync_WithCorrectAnswerInGroupOne_SavesAndReturnsTenPointsAndTheCorrectAnswer()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);

        var result = await CreateAsync(db, StudentId, taskId, "A", "Įrodymas");

        Assert.Equal(10, result.Submission!.Points);
        Assert.Equal("A", result.Submission.CorrectAnswer);
        Assert.Equal(10, (await db.Submissions.SingleAsync()).Points);
    }

    [Fact]
    public async Task CreateAsync_WithCorrectAnswerInGroupTwo_SavesTwentyPoints()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var firstGroupTaskId = await AddTaskAsync(db, classId, 1);
        await AddSubmissionAsync(db, StudentId, firstGroupTaskId, 10);
        var taskId = await AddTaskAsync(db, classId, 2);

        var result = await CreateAsync(db, StudentId, taskId, "A", "Įrodymas");

        Assert.Equal(20, result.Submission!.Points);
        Assert.Equal(20, (await db.Submissions.SingleAsync(s => s.ClassTaskId == taskId)).Points);
    }

    [Fact]
    public async Task CreateAsync_WithCorrectAnswerInGroupThree_SavesThirtyPoints()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var secondGroupTaskId = await AddTaskAsync(db, classId, 2);
        await AddSubmissionAsync(db, StudentId, secondGroupTaskId, 20);
        var taskId = await AddTaskAsync(db, classId, 3);

        var result = await CreateAsync(db, StudentId, taskId, "A", "Įrodymas");

        Assert.Equal(30, result.Submission!.Points);
        Assert.Equal(30, (await db.Submissions.SingleAsync(s => s.ClassTaskId == taskId)).Points);
    }

    [Fact]
    public async Task CreateAsync_WithIncorrectAnswer_SavesZeroPointsAndReturnsTheCorrectAnswer()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);

        var result = await CreateAsync(db, StudentId, taskId, "B", "Įrodymas");

        Assert.Equal(SubmissionOutcome.Created, result.Outcome);
        Assert.Equal(0, result.Submission!.Points);
        Assert.Equal("A", result.Submission.CorrectAnswer);
        Assert.Equal(0, (await db.Submissions.SingleAsync()).Points);
    }

    [Fact]
    public async Task CreateAsync_ForTypedAnswerWithDifferentLetterCase_SavesThePoints()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1, hasOptions: false);

        var result = await CreateAsync(db, StudentId, taskId, "a", "Įrodymas");

        Assert.Equal(10, result.Submission!.Points);
    }

    [Fact]
    public async Task CreateAsync_WhenTheSubmissionReachesSixtyPercent_ReturnsTheUnlockedGroup()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var firstGroupTaskIds = await AddTasksAsync(db, classId, 1, 5);
        await AddTaskAsync(db, classId, 2);
        await AddSubmissionAsync(db, StudentId, firstGroupTaskIds[0], 10);
        await AddSubmissionAsync(db, StudentId, firstGroupTaskIds[1], 10);

        var result = await CreateAsync(db, StudentId, firstGroupTaskIds[2], "A", "Įrodymas");

        Assert.Equal(2, result.Submission!.UnlockedGroup);
    }

    [Fact]
    public async Task CreateAsync_WhenTheSubmissionStaysBelowSixtyPercent_ReturnsNoUnlockedGroup()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var firstGroupTaskIds = await AddTasksAsync(db, classId, 1, 5);
        await AddTaskAsync(db, classId, 2);
        await AddSubmissionAsync(db, StudentId, firstGroupTaskIds[0], 10);
        await AddSubmissionAsync(db, StudentId, firstGroupTaskIds[1], 10);

        var result = await CreateAsync(db, StudentId, firstGroupTaskIds[2], "B", "Įrodymas");

        Assert.Null(result.Submission!.UnlockedGroup);
    }

    [Fact]
    public async Task CreateAsync_WhenTheNextGroupWasAlreadyUnlocked_ReturnsNoUnlockedGroup()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var firstGroupTaskIds = await AddTasksAsync(db, classId, 1, 5);
        await AddTaskAsync(db, classId, 2);
        foreach (var earnedTaskId in firstGroupTaskIds.Take(3))
        {
            await AddSubmissionAsync(db, StudentId, earnedTaskId, 10);
        }

        var result = await CreateAsync(db, StudentId, firstGroupTaskIds[3], "A", "Įrodymas");

        Assert.Null(result.Submission!.UnlockedGroup);
    }

    [Fact]
    public async Task CreateAsync_WhenTaskIsAlreadySubmittedWithCorrectAnswer_KeepsTheFirstPoints()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);
        await CreateAsync(db, StudentId, taskId, "B", "Pirmas įrodymas");

        var result = await CreateAsync(db, StudentId, taskId, "A", "Antras įrodymas");

        Assert.Equal(SubmissionOutcome.AlreadySubmitted, result.Outcome);
        Assert.Equal(0, (await db.Submissions.SingleAsync()).Points);
    }

    private static Task<SubmissionResult> CreateAsync(ApplicationDbContext db, string studentId, Guid taskId, string answer, string evidence)
    {
        var request = new CreateSubmissionRequest { Answer = answer, Evidence = evidence };
        return new SubmissionService(db, TimeProvider.System).CreateAsync(studentId, taskId, request, CancellationToken.None);
    }

    private static async Task<Guid> AddClassWithMemberAsync(
        ApplicationDbContext db,
        string studentId,
        string code = "AB12CD",
        DateTime? endedAtUtc = null)
    {
        var schoolClass = new SchoolClass { Id = Guid.NewGuid(), Name = "7a", Code = code, OwnerId = "teacher-1", CreatedAtUtc = DateTime.UtcNow };
        db.Classes.Add(schoolClass);
        await db.SaveChangesAsync();
        await AddMemberAsync(db, schoolClass.Id, studentId, endedAtUtc);
        return schoolClass.Id;
    }

    private static async Task AddMemberAsync(ApplicationDbContext db, Guid classId, string studentId, DateTime? endedAtUtc = null)
    {
        db.ClassMemberships.Add(new ClassMembership
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ClassId = classId,
            StartedAtUtc = DateTime.UtcNow,
            EndedAtUtc = endedAtUtc
        });
        await db.SaveChangesAsync();
    }

    private static async Task<Guid> AddTaskAsync(ApplicationDbContext db, Guid classId, int group, bool hasOptions = true)
    {
        var task = new ClassTask
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            Type = TaskType.CommonSpecies,
            DifficultyGroup = group,
            Question = "Question",
            CorrectAnswer = "A",
            Options = hasOptions ? new[] { "A", "B" } : null
        };
        db.ClassTasks.Add(task);
        await db.SaveChangesAsync();
        return task.Id;
    }

    private static async Task<List<Guid>> AddTasksAsync(ApplicationDbContext db, Guid classId, int group, int count)
    {
        var taskIds = new List<Guid>();
        for (var i = 0; i < count; i++)
        {
            taskIds.Add(await AddTaskAsync(db, classId, group));
        }

        return taskIds;
    }

    private static async Task AddSubmissionAsync(ApplicationDbContext db, string studentId, Guid taskId, int points)
    {
        db.Submissions.Add(new Submission
        {
            Id = Guid.NewGuid(),
            ClassTaskId = taskId,
            StudentId = studentId,
            Answer = "A",
            Evidence = "Įrodymas",
            Points = points,
            SubmittedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }
}
