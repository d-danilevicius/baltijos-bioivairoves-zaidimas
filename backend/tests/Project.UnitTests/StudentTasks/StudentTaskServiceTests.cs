using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.DTOs.Responses;
using Project.Api.Entities;
using Project.Api.Services;
using Project.UnitTests.Registration;
using Xunit;

namespace Project.UnitTests.StudentTasks;

public class StudentTaskServiceTests
{
    private const string StudentId = "student-1";

    [Fact]
    public async Task GetAsync_ForStudentOfClassWithTasks_ReturnsGroupsOneToThree()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        await AddTaskAsync(db, classId, 3);
        await AddTaskAsync(db, classId, 1);
        await AddTaskAsync(db, classId, 2);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.True(result.HasClass);
        Assert.Equal(new[] { 1, 2, 3 }, result.Groups.Select(g => g.Group));
    }

    [Fact]
    public async Task GetAsync_ForGroupOne_ReturnsUnlockedGroupWithNotDoneTasks()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        await AddTaskAsync(db, classId, 1);
        await AddTaskAsync(db, classId, 2);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        var groupOne = result.Groups.Single(g => g.Group == 1);
        Assert.True(groupOne.IsUnlocked);
        Assert.All(groupOne.Tasks, t => Assert.Equal(StudentTaskStatus.NotDone, t.Status));
    }

    [Fact]
    public async Task GetAsync_WhenStudentHasNoPoints_ReturnsGroupsTwoAndThreeLocked()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        await AddTaskAsync(db, classId, 1);
        await AddTaskAsync(db, classId, 2);
        await AddTaskAsync(db, classId, 3);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        foreach (var group in result.Groups.Where(g => g.Group > 1))
        {
            Assert.False(group.IsUnlocked);
            Assert.All(group.Tasks, t => Assert.Equal(StudentTaskStatus.Locked, t.Status));
        }
    }

    [Fact]
    public async Task GetAsync_WithGbifAndTeacherTasks_LabelsEachTaskBySource()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        await AddTaskAsync(db, classId, 1, question: "A", source: TaskSource.Gbif);
        await AddTaskAsync(db, classId, 1, question: "B", source: TaskSource.Teacher, type: TaskType.TeacherTask);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        var labels = result.Groups.Single().Tasks.Select(t => t.Label);
        Assert.Equal(new[] { "GBIF", "Mokytojo užduotis" }, labels);
    }

    [Fact]
    public async Task GetAsync_ForStudentWithoutClass_ReturnsNoClassAndNoGroups()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, "another-student");
        await AddTaskAsync(db, classId, 1);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.False(result.HasClass);
        Assert.Empty(result.Groups);
        Assert.Null(result.ContinueTaskId);
    }

    [Fact]
    public async Task GetAsync_WhenMembershipEnded_ReturnsNoClassAndNoGroups()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId, endedAtUtc: DateTime.UtcNow);
        await AddTaskAsync(db, classId, 1);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.False(result.HasClass);
        Assert.Empty(result.Groups);
    }

    [Fact]
    public async Task GetAsync_ForClassWithoutTasks_ReturnsClassAndNoGroups()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await AddClassWithMemberAsync(db, StudentId);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.True(result.HasClass);
        Assert.Empty(result.Groups);
        Assert.Null(result.ContinueTaskId);
    }

    [Fact]
    public async Task GetAsync_WhenGroupTwoHasNoTasks_DoesNotReturnGroupTwoAndUnlocksGroupThree()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        await AddTaskAsync(db, classId, 1);
        await AddTaskAsync(db, classId, 3);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.Equal(new[] { 1, 3 }, result.Groups.Select(g => g.Group));
        Assert.True(result.Groups.Single(g => g.Group == 3).IsUnlocked);
    }

    [Fact]
    public async Task GetAsync_WhenOnlyGroupTwoHasTasks_UnlocksGroupTwoAtOnce()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 2, source: TaskSource.Teacher, type: TaskType.TeacherTask);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        var group = Assert.Single(result.Groups);
        Assert.True(group.IsUnlocked);
        Assert.Equal(StudentTaskStatus.NotDone, Assert.Single(group.Tasks).Status);
        Assert.Equal(taskId, result.ContinueTaskId);
    }

    [Fact]
    public async Task GetAsync_WithTasksOfAnotherClass_DoesNotReturnThem()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var ownClassId = await AddClassWithMemberAsync(db, StudentId);
        var ownTaskId = await AddTaskAsync(db, ownClassId, 1);
        var otherClassId = await AddClassWithMemberAsync(db, "another-student", code: "ZZ99ZZ");
        await AddTaskAsync(db, otherClassId, 1);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.Equal(ownTaskId, Assert.Single(result.Groups.Single().Tasks).Id);
    }

    [Fact]
    public async Task GetAsync_ForStudentWhoChangedClass_ReturnsOnlyTasksOfTheNewClass()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var oldClassId = await AddClassWithMemberAsync(db, StudentId, endedAtUtc: DateTime.UtcNow);
        await AddTaskAsync(db, oldClassId, 1);
        var newClassId = await AddClassWithMemberAsync(db, StudentId, code: "ZZ99ZZ");
        var newTaskId = await AddTaskAsync(db, newClassId, 1);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.Equal(newTaskId, Assert.Single(result.Groups.Single().Tasks).Id);
    }

    [Fact]
    public async Task GetAsync_WithTasksInGroupOne_ReturnsFirstTaskOfGroupOneToContinueFrom()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        await AddTaskAsync(db, classId, 2, question: "A");
        await AddTaskAsync(db, classId, 1, question: "B");
        var firstTaskId = await AddTaskAsync(db, classId, 1, question: "A");

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.Equal(firstTaskId, result.ContinueTaskId);
    }

    [Fact]
    public async Task GetAsync_WithTasksOfSameGroupTypeAndQuestion_ReturnsThemOrderedByIdAndContinuesFromTheFirst()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var ids = new[]
        {
            Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Guid.Parse("00000000-0000-0000-0000-000000000003")
        };
        foreach (var id in ids.Reverse())
        {
            await AddTaskAsync(db, classId, 1, id: id);
        }

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.Equal(ids, result.Groups.Single().Tasks.Select(t => t.Id));
        Assert.Equal(ids[0], result.ContinueTaskId);
    }

    [Fact]
    public async Task GetAsync_ForSubmittedTask_ReturnsStatusDoneAndKeepsOtherTasksNotDone()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var doneTaskId = await AddTaskAsync(db, classId, 1, question: "A");
        var openTaskId = await AddTaskAsync(db, classId, 1, question: "B");
        await AddSubmissionAsync(db, doneTaskId, StudentId);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        var tasks = result.Groups.Single().Tasks;
        Assert.Equal(StudentTaskStatus.Done, tasks.Single(t => t.Id == doneTaskId).Status);
        Assert.Equal(StudentTaskStatus.NotDone, tasks.Single(t => t.Id == openTaskId).Status);
    }

    [Fact]
    public async Task GetAsync_WhenAnotherStudentSubmittedTheTask_DoesNotMarkItDone()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);
        await AddSubmissionAsync(db, taskId, "another-student");

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.Equal(StudentTaskStatus.NotDone, result.Groups.Single().Tasks.Single().Status);
        Assert.Equal(taskId, result.ContinueTaskId);
    }

    [Fact]
    public async Task GetAsync_WhenFirstTaskIsDone_ContinuesFromTheNextTask()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var firstTaskId = await AddTaskAsync(db, classId, 1, id: new Guid("00000000-0000-0000-0000-000000000001"));
        var secondTaskId = await AddTaskAsync(db, classId, 1, id: new Guid("00000000-0000-0000-0000-000000000002"));
        await AddSubmissionAsync(db, firstTaskId, StudentId);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.Equal(secondTaskId, result.ContinueTaskId);
    }

    [Fact]
    public async Task GetAsync_WhenAllTasksOfUnlockedGroupsAreDone_HasNothingToContinueFrom()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);
        await AddTaskAsync(db, classId, 2);
        await AddSubmissionAsync(db, taskId, StudentId);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.Null(result.ContinueTaskId);
    }

    [Fact]
    public async Task GetAsync_ForDoneTask_ReturnsItsPointsAndNoPointsForTasksNotDone()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var doneTaskId = await AddTaskAsync(db, classId, 1, question: "A");
        var openTaskId = await AddTaskAsync(db, classId, 1, question: "B");
        await AddSubmissionAsync(db, doneTaskId, StudentId, 10);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        var tasks = result.Groups.Single().Tasks;
        Assert.Equal(10, tasks.Single(t => t.Id == doneTaskId).Points);
        Assert.Null(tasks.Single(t => t.Id == openTaskId).Points);
    }

    [Fact]
    public async Task GetAsync_ForDoneTaskWithIncorrectAnswer_ReturnsZeroPoints()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var taskId = await AddTaskAsync(db, classId, 1);
        await AddSubmissionAsync(db, taskId, StudentId, 0);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        var task = result.Groups.Single().Tasks.Single();
        Assert.Equal(StudentTaskStatus.Done, task.Status);
        Assert.Equal(0, task.Points);
    }

    [Fact]
    public async Task GetAsync_ForGroup_ReturnsTheEarnedAndTheMaximumPoints()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var firstGroupTaskIds = await AddTasksAsync(db, classId, 1, 5);
        await AddTaskAsync(db, classId, 2);
        await AddSubmissionAsync(db, firstGroupTaskIds[0], StudentId, 10);
        await AddSubmissionAsync(db, firstGroupTaskIds[1], StudentId, 10);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        var groupOne = result.Groups.Single(g => g.Group == 1);
        Assert.Equal(20, groupOne.Points);
        Assert.Equal(50, groupOne.MaxPoints);
        var groupTwo = result.Groups.Single(g => g.Group == 2);
        Assert.Equal(0, groupTwo.Points);
        Assert.Equal(20, groupTwo.MaxPoints);
    }

    [Fact]
    public async Task GetAsync_WhenPreviousGroupHasSixtyPercent_UnlocksTheNextGroup()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var firstGroupTaskIds = await AddTasksAsync(db, classId, 1, 5);
        await AddTaskAsync(db, classId, 2);
        foreach (var taskId in firstGroupTaskIds.Take(3))
        {
            await AddSubmissionAsync(db, taskId, StudentId, 10);
        }

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        var groupTwo = result.Groups.Single(g => g.Group == 2);
        Assert.True(groupTwo.IsUnlocked);
        Assert.All(groupTwo.Tasks, t => Assert.Equal(StudentTaskStatus.NotDone, t.Status));
    }

    [Fact]
    public async Task GetAsync_WhenPreviousGroupHasLessThanSixtyPercent_KeepsTheNextGroupLocked()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var firstGroupTaskIds = await AddTasksAsync(db, classId, 1, 5);
        await AddTaskAsync(db, classId, 2);
        foreach (var taskId in firstGroupTaskIds.Take(3))
        {
            await AddSubmissionAsync(db, taskId, StudentId, taskId == firstGroupTaskIds[2] ? 0 : 10);
        }

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        var groupTwo = result.Groups.Single(g => g.Group == 2);
        Assert.False(groupTwo.IsUnlocked);
        Assert.All(groupTwo.Tasks, t => Assert.Equal(StudentTaskStatus.Locked, t.Status));
    }

    [Fact]
    public async Task GetAsync_WhenGroupOneIsEarned_ContinuesFromTheFirstTaskOfGroupTwo()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var firstGroupTaskId = await AddTaskAsync(db, classId, 1);
        var secondGroupTaskId = await AddTaskAsync(db, classId, 2);
        await AddSubmissionAsync(db, firstGroupTaskId, StudentId, 10);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.Equal(secondGroupTaskId, result.ContinueTaskId);
    }

    [Fact]
    public async Task GetAsync_DoesNotUseThePointsOfAnotherStudent()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        var firstGroupTaskIds = await AddTasksAsync(db, classId, 1, 5);
        await AddTaskAsync(db, classId, 2);
        foreach (var taskId in firstGroupTaskIds)
        {
            await AddSubmissionAsync(db, taskId, "another-student", 10);
        }

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.Equal(0, result.Groups.Single(g => g.Group == 1).Points);
        Assert.False(result.Groups.Single(g => g.Group == 2).IsUnlocked);
    }

    [Fact]
    public async Task GetAsync_DoesNotCountTheSubmissionOfADeletedTask()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var classId = await AddClassWithMemberAsync(db, StudentId);
        await AddTaskAsync(db, classId, 1);
        await AddTaskAsync(db, classId, 2);
        await AddSubmissionAsync(db, null, StudentId, 10);

        var result = await new StudentTaskService(db).GetAsync(StudentId, CancellationToken.None);

        Assert.Equal(0, result.Groups.Single(g => g.Group == 1).Points);
        Assert.False(result.Groups.Single(g => g.Group == 2).IsUnlocked);
    }

    [Fact]
    public void StudentTaskResponse_HasOnlyIdentifierTypeLabelStatusAndPoints()
    {
        var properties = typeof(StudentTaskResponse).GetProperties().Select(p => p.Name);

        Assert.Equal(new[] { "Id", "Label", "Points", "Status", "Type" }, properties.OrderBy(name => name));
    }

    private static async Task AddSubmissionAsync(ApplicationDbContext db, Guid? taskId, string studentId, int points = 0)
    {
        db.Submissions.Add(new Submission
        {
            Id = Guid.NewGuid(),
            ClassTaskId = taskId,
            StudentId = studentId,
            Answer = "Answer",
            Evidence = "Evidence",
            Points = points,
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

    private static async Task<List<Guid>> AddTasksAsync(ApplicationDbContext db, Guid classId, int group, int count)
    {
        var taskIds = new List<Guid>();
        for (var i = 0; i < count; i++)
        {
            taskIds.Add(await AddTaskAsync(db, classId, group));
        }

        return taskIds;
    }

    private static async Task<Guid> AddTaskAsync(
        ApplicationDbContext db,
        Guid classId,
        int group,
        string question = "Question",
        TaskSource source = TaskSource.Gbif,
        TaskType type = TaskType.CommonSpecies,
        Guid? id = null)
    {
        var task = new ClassTask
        {
            Id = id ?? Guid.NewGuid(),
            ClassId = classId,
            Type = type,
            Source = source,
            DifficultyGroup = group,
            Question = question,
            CorrectAnswer = "Answer"
        };
        db.ClassTasks.Add(task);
        await db.SaveChangesAsync();
        return task.Id;
    }
}
