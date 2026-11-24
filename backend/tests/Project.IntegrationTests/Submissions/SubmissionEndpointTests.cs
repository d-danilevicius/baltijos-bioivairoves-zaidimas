using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.Entities;
using Project.IntegrationTests.Classes;
using Project.IntegrationTests.Gbif;

namespace Project.IntegrationTests.Submissions;

public class SubmissionEndpointTests : IClassFixture<GbifApiFixture>
{
    private const string Evidence = "Mačiau prie ežero.";

    private readonly GbifApiFixture _fixture;

    public SubmissionEndpointTests(GbifApiFixture fixture)
    {
        _fixture = fixture;
    }

    private static object Body(string answer = "A", string evidence = Evidence) => new { answer, evidence };

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response) =>
        JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.Clone();

    private async Task<(ClassTestClient Teacher, ClassTestClient Student, string ClassId)> CreateClassWithStudentAsync()
    {
        var teacher = await ClassTestClient.LoginAsync(_fixture, RoleName.Teacher);
        var created = await ReadJsonAsync(await teacher.CreateAsync("7a"));
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);
        (await student.JoinAsync(created.GetProperty("code").GetString()!)).EnsureSuccessStatusCode();
        return (teacher, student, created.GetProperty("id").GetString()!);
    }

    // A teacher task has no answer options, so the answer is typed
    private static async Task<string> CreateTeacherTaskAsync(ClassTestClient teacher, string classId, int group = 1)
    {
        var response = await teacher.CreateTeacherTaskAsync(classId, new
        {
            title = "Mokyklos kiemas",
            question = "Kokį paukštį matote ant stogo?",
            correctAnswer = "Žvirblis",
            difficultyGroup = group,
            noOrganismDisturbanceConfirmed = true
        });
        response.EnsureSuccessStatusCode();
        return (await ReadJsonAsync(response)).GetProperty("id").GetString()!;
    }

    // The GBIF client is replaced by a fake in the tests, so a task with the options A and B is stored directly
    private async Task<string> AddTaskWithOptionsAsync(string classId, int group = 1, string question = "Question")
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var task = new ClassTask
        {
            Id = Guid.NewGuid(),
            ClassId = Guid.Parse(classId),
            Type = TaskType.CommonSpecies,
            Source = TaskSource.Gbif,
            DifficultyGroup = group,
            Question = question,
            CorrectAnswer = "A",
            Options = new[] { "A", "B" }
        };
        db.ClassTasks.Add(task);
        await db.SaveChangesAsync();
        return task.Id.ToString();
    }

    private async Task<List<Submission>> ReadStoredSubmissionsAsync(string taskId)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await db.Submissions.AsNoTracking().Where(s => s.ClassTaskId == Guid.Parse(taskId)).ToListAsync();
    }

    [Fact]
    public async Task SubmitAnswer_WithChosenOption_Returns201AndSavesTheSubmission()
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await AddTaskWithOptionsAsync(classId);

        var response = await student.SubmitAnswerAsync(taskId, Body("A"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal("Mokinio pateikti duomenys", json.GetProperty("label").GetString());
        Assert.Equal("A", json.GetProperty("answer").GetString());
        Assert.Equal(Evidence, json.GetProperty("evidence").GetString());
        Assert.Equal(taskId, json.GetProperty("taskId").GetString());
        var stored = Assert.Single(await ReadStoredSubmissionsAsync(taskId));
        Assert.Equal("A", stored.Answer);
        Assert.Equal(Evidence, stored.Evidence);
    }

    [Fact]
    public async Task SubmitAnswer_ForTeacherTask_AcceptsTheTypedAnswer()
    {
        var (teacher, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await CreateTeacherTaskAsync(teacher, classId);

        var response = await student.SubmitAnswerAsync(taskId, Body("Žvirblis"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Žvirblis", Assert.Single(await ReadStoredSubmissionsAsync(taskId)).Answer);
    }

    [Fact]
    public async Task SubmitAnswer_ForTheSecondTime_Returns409AndKeepsTheFirstAnswer()
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await AddTaskWithOptionsAsync(classId);
        (await student.SubmitAnswerAsync(taskId, Body("A", "Pirmas įrodymas"))).EnsureSuccessStatusCode();

        var response = await student.SubmitAnswerAsync(taskId, Body("B", "Antras įrodymas"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var stored = Assert.Single(await ReadStoredSubmissionsAsync(taskId));
        Assert.Equal("A", stored.Answer);
        Assert.Equal("Pirmas įrodymas", stored.Evidence);
    }

    [Fact]
    public async Task SubmitAnswer_AtTheSameTime_SavesOneSubmissionAndReturns409ForTheRest()
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await AddTaskWithOptionsAsync(classId);

        var responses = await student.SubmitAnswerAtTheSameTimeAsync(taskId, Body("A"), 5);

        Assert.Single(responses, r => r.StatusCode == HttpStatusCode.Created);
        Assert.Equal(4, responses.Count(r => r.StatusCode == HttpStatusCode.Conflict));
        Assert.Single(await ReadStoredSubmissionsAsync(taskId));
    }

    [Fact]
    public async Task SubmitAnswer_WithEmptyAnswerAndEvidence_Returns400WithErrorsPerField()
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await AddTaskWithOptionsAsync(classId);

        var response = await student.SubmitAnswerAsync(taskId, Body("", "   "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await ReadJsonAsync(response)).GetProperty("errors");
        Assert.Equal("Required", errors.GetProperty("answer")[0].GetString());
        Assert.Equal("Required", errors.GetProperty("evidence")[0].GetString());
        Assert.Empty(await ReadStoredSubmissionsAsync(taskId));
    }

    [Fact]
    public async Task SubmitAnswer_WithTooLongAnswerAndEvidence_Returns400WithTooLong()
    {
        var (teacher, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await CreateTeacherTaskAsync(teacher, classId);

        var response = await student.SubmitAnswerAsync(taskId, Body(new string('a', 101), new string('b', 501)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await ReadJsonAsync(response)).GetProperty("errors");
        Assert.Equal("TooLong", errors.GetProperty("answer")[0].GetString());
        Assert.Equal("TooLong", errors.GetProperty("evidence")[0].GetString());
        Assert.Empty(await ReadStoredSubmissionsAsync(taskId));
    }

    [Fact]
    public async Task SubmitAnswer_WithAnswerThatIsNotAnOption_Returns400AndSavesNothing()
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await AddTaskWithOptionsAsync(classId);

        var response = await student.SubmitAnswerAsync(taskId, Body("C"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Invalid", (await ReadJsonAsync(response)).GetProperty("errors").GetProperty("answer")[0].GetString());
        Assert.Empty(await ReadStoredSubmissionsAsync(taskId));
    }

    [Fact]
    public async Task SubmitAnswer_ForTaskOfLockedGroup_Returns403AndSavesNothing()
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        await AddTaskWithOptionsAsync(classId, group: 1);
        var lockedTaskId = await AddTaskWithOptionsAsync(classId, group: 2);

        var response = await student.SubmitAnswerAsync(lockedTaskId, Body("A"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Empty(await ReadStoredSubmissionsAsync(lockedTaskId));
    }

    [Fact]
    public async Task SubmitAnswer_ForTaskOfAnotherClass_Returns404AndSavesNothing()
    {
        var (_, student, _) = await CreateClassWithStudentAsync();
        var (_, _, otherClassId) = await CreateClassWithStudentAsync();
        var otherTaskId = await AddTaskWithOptionsAsync(otherClassId);

        var response = await student.SubmitAnswerAsync(otherTaskId, Body("A"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Empty(await ReadStoredSubmissionsAsync(otherTaskId));
    }

    [Fact]
    public async Task SubmitAnswer_ForMissingTask_Returns404()
    {
        var (_, student, _) = await CreateClassWithStudentAsync();

        var response = await student.SubmitAnswerAsync(Guid.NewGuid().ToString(), Body("A"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SubmitAnswer_AfterTeacherDeletesTheTask_Returns404()
    {
        var (teacher, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await CreateTeacherTaskAsync(teacher, classId);
        (await teacher.DeleteTaskAsync(classId, taskId)).EnsureSuccessStatusCode();

        var response = await student.SubmitAnswerAsync(taskId, Body("Žvirblis"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTask_AfterStudentSubmitted_KeepsTheSubmissionWithoutTheTask()
    {
        var (teacher, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await CreateTeacherTaskAsync(teacher, classId);
        (await student.SubmitAnswerAsync(taskId, Body("Žvirblis"))).EnsureSuccessStatusCode();

        var response = await teacher.DeleteTaskAsync(classId, taskId);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Contains(await db.Submissions.AsNoTracking().ToListAsync(), s => s.ClassTaskId == null && s.Answer == "Žvirblis");
    }

    [Theory]
    [InlineData(RoleName.Teacher)]
    [InlineData(RoleName.Administrator)]
    public async Task SubmitAnswer_ForTeacherOrAdministrator_Returns403(string role)
    {
        var client = await ClassTestClient.LoginAsync(_fixture, role);

        var response = await client.SubmitAnswerAsync(Guid.NewGuid().ToString(), Body("A"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SubmitAnswer_ForVisitor_Returns401()
    {
        var visitor = ClassTestClient.CreateAnonymous(_fixture);

        var response = await visitor.SendWithoutCsrfTokenAsync(HttpMethod.Post, $"/api/my/tasks/{Guid.NewGuid()}/submissions", Body("A"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SubmitAnswer_WithoutCsrfToken_Returns400AndSavesNothing()
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await AddTaskWithOptionsAsync(classId);

        var response = await student.SendWithoutCsrfTokenAsync(HttpMethod.Post, $"/api/my/tasks/{taskId}/submissions", Body("A"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(await ReadStoredSubmissionsAsync(taskId));
    }

    [Fact]
    public async Task ListMyTasks_AfterSubmitting_ShowsDoneAndContinuesFromTheNextTask()
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        var firstTaskId = await AddTaskWithOptionsAsync(classId, question: "A first");
        var secondTaskId = await AddTaskWithOptionsAsync(classId, question: "B second");
        (await student.SubmitAnswerAsync(firstTaskId, Body("A"))).EnsureSuccessStatusCode();

        var json = await ReadJsonAsync(await student.ListMyTasksAsync());

        var tasks = json.GetProperty("groups")[0].GetProperty("tasks").EnumerateArray().ToList();
        Assert.Equal("Done", tasks.Single(t => t.GetProperty("id").GetString() == firstTaskId).GetProperty("status").GetString());
        Assert.Equal("NotDone", tasks.Single(t => t.GetProperty("id").GetString() == secondTaskId).GetProperty("status").GetString());
        Assert.Equal(secondTaskId, json.GetProperty("continueTaskId").GetString());
    }

    [Fact]
    public async Task GetMyTask_AfterSubmitting_ReturnsTheSubmissionWithPointsAndTheCorrectAnswer()
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await AddTaskWithOptionsAsync(classId);
        (await student.SubmitAnswerAsync(taskId, Body("B"))).EnsureSuccessStatusCode();

        var response = await student.GetMyTaskAsync(taskId);

        var json = await ReadJsonAsync(response);
        var submission = json.GetProperty("submission");
        Assert.Equal("B", submission.GetProperty("answer").GetString());
        Assert.Equal("Mokinio pateikti duomenys", submission.GetProperty("label").GetString());
        Assert.Equal(0, submission.GetProperty("points").GetInt32());
        Assert.Equal("A", submission.GetProperty("correctAnswer").GetString());
    }
}
