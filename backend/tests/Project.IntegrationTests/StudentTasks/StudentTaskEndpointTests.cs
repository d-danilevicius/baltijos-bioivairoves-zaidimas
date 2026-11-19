using System.Net;
using System.Text.Json;
using Project.Api.Entities;
using Project.IntegrationTests.Classes;
using Project.IntegrationTests.Gbif;

namespace Project.IntegrationTests.StudentTasks;

public class StudentTaskEndpointTests : IClassFixture<GbifApiFixture>
{
    private const string Title = "Mokyklos kiemas";
    private const string Question = "Kokį paukštį matote ant stogo?";
    private const string CorrectAnswer = "Žvirblis";

    private readonly GbifApiFixture _fixture;

    public StudentTaskEndpointTests(GbifApiFixture fixture)
    {
        _fixture = fixture;
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response) =>
        JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.Clone();

    private static async Task<string> CreateTeacherTaskAsync(ClassTestClient teacher, string classId, int group)
    {
        var response = await teacher.CreateTeacherTaskAsync(classId, new
        {
            title = Title,
            question = Question,
            correctAnswer = CorrectAnswer,
            difficultyGroup = group,
            noOrganismDisturbanceConfirmed = true
        });
        response.EnsureSuccessStatusCode();
        return (await ReadJsonAsync(response)).GetProperty("id").GetString()!;
    }

    private async Task<(ClassTestClient Teacher, ClassTestClient Student, string ClassId)> CreateClassWithStudentAsync()
    {
        var teacher = await ClassTestClient.LoginAsync(_fixture, RoleName.Teacher);
        var created = await ReadJsonAsync(await teacher.CreateAsync("7a"));
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);
        (await student.JoinAsync(created.GetProperty("code").GetString()!)).EnsureSuccessStatusCode();
        return (teacher, student, created.GetProperty("id").GetString()!);
    }

    [Fact]
    public async Task GetMyTasks_ForStudentOfClass_Returns200AndTheGroupsWithTheTaskToContinueFrom()
    {
        var (teacher, student, classId) = await CreateClassWithStudentAsync();
        var firstTaskId = await CreateTeacherTaskAsync(teacher, classId, 1);
        await CreateTeacherTaskAsync(teacher, classId, 2);

        var response = await student.ListMyTasksAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.True(json.GetProperty("hasClass").GetBoolean());
        Assert.Equal(firstTaskId, json.GetProperty("continueTaskId").GetString());
        var groups = json.GetProperty("groups").EnumerateArray().ToList();
        Assert.Equal(new[] { 1, 2 }, groups.Select(g => g.GetProperty("group").GetInt32()));
        Assert.True(groups[0].GetProperty("isUnlocked").GetBoolean());
        Assert.False(groups[1].GetProperty("isUnlocked").GetBoolean());
        var lockedTask = groups[1].GetProperty("tasks").EnumerateArray().Single();
        Assert.Equal("Locked", lockedTask.GetProperty("status").GetString());
        Assert.Equal("Mokytojo užduotis", lockedTask.GetProperty("label").GetString());
    }

    [Fact]
    public async Task GetMyTasks_ForStudentOfClass_DoesNotReturnTitleQuestionCorrectAnswerOrOptions()
    {
        var (teacher, student, classId) = await CreateClassWithStudentAsync();
        await CreateTeacherTaskAsync(teacher, classId, 1);

        var text = await (await student.ListMyTasksAsync()).Content.ReadAsStringAsync();

        Assert.DoesNotContain(Title, text);
        Assert.DoesNotContain(Question, text);
        Assert.DoesNotContain(CorrectAnswer, text);
        Assert.DoesNotContain("options", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("title", text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetMyTasks_ForStudentWithoutClass_Returns200WithNoClassAndNoGroups()
    {
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);

        var response = await student.ListMyTasksAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.False(json.GetProperty("hasClass").GetBoolean());
        Assert.Empty(json.GetProperty("groups").EnumerateArray());
    }

    [Fact]
    public async Task GetMyTasks_AfterTeacherDeletesTheTask_DoesNotReturnTheTask()
    {
        var (teacher, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await CreateTeacherTaskAsync(teacher, classId, 1);
        (await teacher.DeleteTaskAsync(classId, taskId)).EnsureSuccessStatusCode();

        var json = await ReadJsonAsync(await student.ListMyTasksAsync());

        Assert.Empty(json.GetProperty("groups").EnumerateArray());
        Assert.Equal(JsonValueKind.Null, json.GetProperty("continueTaskId").ValueKind);
    }

    [Theory]
    [InlineData(RoleName.Teacher)]
    [InlineData(RoleName.Administrator)]
    public async Task GetMyTasks_ForTeacherOrAdministrator_Returns403(string role)
    {
        var client = await ClassTestClient.LoginAsync(_fixture, role);

        var response = await client.ListMyTasksAsync();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetMyTasks_ForVisitor_Returns401()
    {
        var visitor = ClassTestClient.CreateAnonymous(_fixture);

        var response = await visitor.ListMyTasksAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
