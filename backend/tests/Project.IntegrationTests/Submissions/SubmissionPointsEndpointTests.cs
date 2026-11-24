using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.Entities;
using Project.IntegrationTests.Classes;
using Project.IntegrationTests.Gbif;

namespace Project.IntegrationTests.Submissions;

public class SubmissionPointsEndpointTests : IClassFixture<GbifApiFixture>
{
    private const string CorrectAnswer = "Žvirblis";
    private const string Evidence = "Mačiau prie ežero.";

    private readonly GbifApiFixture _fixture;

    public SubmissionPointsEndpointTests(GbifApiFixture fixture)
    {
        _fixture = fixture;
    }

    private static object Body(string answer) => new { answer, evidence = Evidence };

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response) =>
        JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.Clone();

    private async Task<(ClassTestClient Teacher, ClassTestClient Student, string ClassId, string Code)> CreateClassWithStudentAsync()
    {
        var teacher = await ClassTestClient.LoginAsync(_fixture, RoleName.Teacher);
        var created = await ReadJsonAsync(await teacher.CreateAsync("7a"));
        var code = created.GetProperty("code").GetString()!;
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);
        (await student.JoinAsync(code)).EnsureSuccessStatusCode();
        return (teacher, student, created.GetProperty("id").GetString()!, code);
    }

    // A teacher task is answered by typing the correct answer
    private static async Task<string> CreateTaskAsync(ClassTestClient teacher, string classId, int group)
    {
        var response = await teacher.CreateTeacherTaskAsync(classId, new
        {
            title = "Mokyklos kiemas",
            question = "Kokį paukštį matote ant stogo?",
            correctAnswer = CorrectAnswer,
            difficultyGroup = group,
            noOrganismDisturbanceConfirmed = true
        });
        response.EnsureSuccessStatusCode();
        return (await ReadJsonAsync(response)).GetProperty("id").GetString()!;
    }

    private static async Task<List<string>> CreateTasksAsync(ClassTestClient teacher, string classId, int group, int count)
    {
        var taskIds = new List<string>();
        for (var i = 0; i < count; i++)
        {
            taskIds.Add(await CreateTaskAsync(teacher, classId, group));
        }

        return taskIds;
    }

    private async Task<int> ReadStoredPointsAsync(string taskId)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await db.Submissions.AsNoTracking().Where(s => s.ClassTaskId == Guid.Parse(taskId)).Select(s => s.Points).SingleAsync();
    }

    private static JsonElement FindGroup(JsonElement list, int group) =>
        list.GetProperty("groups").EnumerateArray().Single(g => g.GetProperty("group").GetInt32() == group);

    [Fact]
    public async Task SubmitAnswer_WithCorrectAnswer_Returns201WithTenPointsAndTheCorrectAnswer()
    {
        var (teacher, student, classId, _) = await CreateClassWithStudentAsync();
        var taskId = await CreateTaskAsync(teacher, classId, 1);

        var response = await student.SubmitAnswerAsync(taskId, Body(CorrectAnswer));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal(10, json.GetProperty("points").GetInt32());
        Assert.Equal(CorrectAnswer, json.GetProperty("correctAnswer").GetString());
        Assert.Equal(10, await ReadStoredPointsAsync(taskId));
    }

    [Fact]
    public async Task SubmitAnswer_WithIncorrectAnswer_Returns201WithZeroPointsAndTheCorrectAnswer()
    {
        var (teacher, student, classId, _) = await CreateClassWithStudentAsync();
        var taskId = await CreateTaskAsync(teacher, classId, 1);

        var response = await student.SubmitAnswerAsync(taskId, Body("Pelėda"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal(0, json.GetProperty("points").GetInt32());
        Assert.Equal(CorrectAnswer, json.GetProperty("correctAnswer").GetString());
        Assert.Equal(0, await ReadStoredPointsAsync(taskId));
    }

    [Fact]
    public async Task SubmitAnswer_WithDifferentLetterCaseAndOuterSpaces_AwardsThePoints()
    {
        var (teacher, student, classId, _) = await CreateClassWithStudentAsync();
        var taskId = await CreateTaskAsync(teacher, classId, 1);

        var json = await ReadJsonAsync(await student.SubmitAnswerAsync(taskId, Body("  žVIRBLIS ")));

        Assert.Equal(10, json.GetProperty("points").GetInt32());
    }

    [Fact]
    public async Task SubmitAnswer_AcrossTheGroups_AwardsTenTwentyAndThirtyPointsAndReturnsTheUnlockedGroups()
    {
        var (teacher, student, classId, _) = await CreateClassWithStudentAsync();
        var firstTaskId = await CreateTaskAsync(teacher, classId, 1);
        var secondTaskId = await CreateTaskAsync(teacher, classId, 2);
        var thirdTaskId = await CreateTaskAsync(teacher, classId, 3);

        var first = await ReadJsonAsync(await student.SubmitAnswerAsync(firstTaskId, Body(CorrectAnswer)));
        var second = await ReadJsonAsync(await student.SubmitAnswerAsync(secondTaskId, Body(CorrectAnswer)));
        var third = await ReadJsonAsync(await student.SubmitAnswerAsync(thirdTaskId, Body(CorrectAnswer)));

        Assert.Equal(10, first.GetProperty("points").GetInt32());
        Assert.Equal(2, first.GetProperty("unlockedGroup").GetInt32());
        Assert.Equal(20, second.GetProperty("points").GetInt32());
        Assert.Equal(3, second.GetProperty("unlockedGroup").GetInt32());
        Assert.Equal(30, third.GetProperty("points").GetInt32());
        Assert.Equal(JsonValueKind.Null, third.GetProperty("unlockedGroup").ValueKind);
    }

    [Fact]
    public async Task SubmitAnswer_ReachingSixtyPercentOfTheGroup_UnlocksTheNextGroupInTheTaskList()
    {
        var (teacher, student, classId, _) = await CreateClassWithStudentAsync();
        var firstGroupTaskIds = await CreateTasksAsync(teacher, classId, 1, 5);
        await CreateTaskAsync(teacher, classId, 2);
        (await student.SubmitAnswerAsync(firstGroupTaskIds[0], Body(CorrectAnswer))).EnsureSuccessStatusCode();
        var beforeThreshold = await ReadJsonAsync(await student.SubmitAnswerAsync(firstGroupTaskIds[1], Body(CorrectAnswer)));
        var listBefore = await ReadJsonAsync(await student.ListMyTasksAsync());

        var atThreshold = await ReadJsonAsync(await student.SubmitAnswerAsync(firstGroupTaskIds[2], Body(CorrectAnswer)));
        var listAfter = await ReadJsonAsync(await student.ListMyTasksAsync());

        Assert.Equal(JsonValueKind.Null, beforeThreshold.GetProperty("unlockedGroup").ValueKind);
        Assert.False(FindGroup(listBefore, 2).GetProperty("isUnlocked").GetBoolean());
        Assert.Equal(20, FindGroup(listBefore, 1).GetProperty("points").GetInt32());
        Assert.Equal(50, FindGroup(listBefore, 1).GetProperty("maxPoints").GetInt32());
        Assert.Equal(2, atThreshold.GetProperty("unlockedGroup").GetInt32());
        Assert.True(FindGroup(listAfter, 2).GetProperty("isUnlocked").GetBoolean());
        Assert.Equal(30, FindGroup(listAfter, 1).GetProperty("points").GetInt32());
    }

    [Fact]
    public async Task SubmitAnswer_BelowSixtyPercentOfTheGroup_KeepsTheNextGroupLocked()
    {
        var (teacher, student, classId, _) = await CreateClassWithStudentAsync();
        var firstGroupTaskIds = await CreateTasksAsync(teacher, classId, 1, 5);
        var lockedTaskId = await CreateTaskAsync(teacher, classId, 2);
        (await student.SubmitAnswerAsync(firstGroupTaskIds[0], Body(CorrectAnswer))).EnsureSuccessStatusCode();
        (await student.SubmitAnswerAsync(firstGroupTaskIds[1], Body(CorrectAnswer))).EnsureSuccessStatusCode();
        (await student.SubmitAnswerAsync(firstGroupTaskIds[2], Body("Pelėda"))).EnsureSuccessStatusCode();

        var response = await student.SubmitAnswerAsync(lockedTaskId, Body(CorrectAnswer));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.False(FindGroup(await ReadJsonAsync(await student.ListMyTasksAsync()), 2).GetProperty("isUnlocked").GetBoolean());
    }

    [Fact]
    public async Task SubmitAnswer_ReturnsThePointsWithinOneSecond()
    {
        var (teacher, student, classId, _) = await CreateClassWithStudentAsync();
        var warmUpTaskId = await CreateTaskAsync(teacher, classId, 1);
        var taskId = await CreateTaskAsync(teacher, classId, 1);
        (await student.SubmitAnswerAsync(warmUpTaskId, Body(CorrectAnswer))).EnsureSuccessStatusCode();

        var stopwatch = Stopwatch.StartNew();
        var response = await student.SubmitAnswerAsync(taskId, Body(CorrectAnswer));
        stopwatch.Stop();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(1), $"The answer took {stopwatch.Elapsed.TotalMilliseconds} ms.");
    }

    [Fact]
    public async Task GetMyTask_AfterSubmitting_ReturnsTheSavedPointsAndTheCorrectAnswer()
    {
        var (teacher, student, classId, _) = await CreateClassWithStudentAsync();
        var taskId = await CreateTaskAsync(teacher, classId, 1);
        (await student.SubmitAnswerAsync(taskId, Body(CorrectAnswer))).EnsureSuccessStatusCode();

        var submission = (await ReadJsonAsync(await student.GetMyTaskAsync(taskId))).GetProperty("submission");

        Assert.Equal(10, submission.GetProperty("points").GetInt32());
        Assert.Equal(CorrectAnswer, submission.GetProperty("correctAnswer").GetString());
    }

    [Fact]
    public async Task GetMyTask_BeforeSubmitting_DoesNotReturnTheCorrectAnswer()
    {
        var (teacher, student, classId, _) = await CreateClassWithStudentAsync();
        var taskId = await CreateTaskAsync(teacher, classId, 1);

        var text = await (await student.GetMyTaskAsync(taskId)).Content.ReadAsStringAsync();

        Assert.DoesNotContain("correctAnswer", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(CorrectAnswer, text);
    }

    [Fact]
    public async Task ListMyTasks_AfterSubmitting_ReturnsThePointsOfTheDoneTask()
    {
        var (teacher, student, classId, _) = await CreateClassWithStudentAsync();
        var doneTaskId = await CreateTaskAsync(teacher, classId, 1);
        var openTaskId = await CreateTaskAsync(teacher, classId, 1);
        (await student.SubmitAnswerAsync(doneTaskId, Body(CorrectAnswer))).EnsureSuccessStatusCode();

        var tasks = FindGroup(await ReadJsonAsync(await student.ListMyTasksAsync()), 1).GetProperty("tasks").EnumerateArray().ToList();

        Assert.Equal(10, tasks.Single(t => t.GetProperty("id").GetString() == doneTaskId).GetProperty("points").GetInt32());
        Assert.Equal(JsonValueKind.Null, tasks.Single(t => t.GetProperty("id").GetString() == openTaskId).GetProperty("points").ValueKind);
    }

    [Fact]
    public async Task ListMyTasks_ForTwoStudentsOfTheSameClass_KeepsTheirPointsSeparate()
    {
        var (teacher, student, classId, code) = await CreateClassWithStudentAsync();
        var firstGroupTaskIds = await CreateTasksAsync(teacher, classId, 1, 2);
        await CreateTaskAsync(teacher, classId, 2);
        var otherStudent = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);
        (await otherStudent.JoinAsync(code)).EnsureSuccessStatusCode();
        foreach (var taskId in firstGroupTaskIds)
        {
            (await student.SubmitAnswerAsync(taskId, Body(CorrectAnswer))).EnsureSuccessStatusCode();
        }

        var own = await ReadJsonAsync(await student.ListMyTasksAsync());
        var other = await ReadJsonAsync(await otherStudent.ListMyTasksAsync());

        Assert.Equal(20, FindGroup(own, 1).GetProperty("points").GetInt32());
        Assert.True(FindGroup(own, 2).GetProperty("isUnlocked").GetBoolean());
        Assert.Equal(0, FindGroup(other, 1).GetProperty("points").GetInt32());
        Assert.False(FindGroup(other, 2).GetProperty("isUnlocked").GetBoolean());
    }
}
