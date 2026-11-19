using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.Entities;
using Project.IntegrationTests.Classes;
using Project.IntegrationTests.Gbif;

namespace Project.IntegrationTests.TeacherTasks;

public class TeacherTaskEndpointTests : IClassFixture<GbifApiFixture>
{
    private static readonly object ValidBody = new
    {
        title = "Mokyklos kiemas",
        question = "Kokį paukštį matote ant stogo?",
        correctAnswer = "Žvirblis",
        difficultyGroup = 2,
        noOrganismDisturbanceConfirmed = true
    };

    private readonly GbifApiFixture _fixture;

    public TeacherTaskEndpointTests(GbifApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.Gbif.Reset();
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response) =>
        JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.Clone();

    private async Task<(ClassTestClient Teacher, string ClassId)> CreateTeacherWithClassAsync()
    {
        var teacher = await ClassTestClient.LoginAsync(_fixture, RoleName.Teacher);
        return (teacher, await teacher.CreateAndGetIdAsync("7a"));
    }

    private async Task<string> CreateTeacherTaskAsync(ClassTestClient teacher, string classId)
    {
        var response = await teacher.CreateTeacherTaskAsync(classId, ValidBody);
        response.EnsureSuccessStatusCode();
        return (await ReadJsonAsync(response)).GetProperty("id").GetString()!;
    }

    private async Task<List<ClassTask>> ReadStoredTasksAsync(string classId)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var id = Guid.Parse(classId);
        return await db.ClassTasks.AsNoTracking().Where(t => t.ClassId == id).ToListAsync();
    }

    [Fact]
    public async Task PostTasks_WithValidBody_Returns201AndSavesTeacherTask()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();

        var response = await teacher.CreateTeacherTaskAsync(classId, ValidBody);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal("Mokytojo užduotis", json.GetProperty("label").GetString());
        Assert.Equal("TeacherTask", json.GetProperty("type").GetString());
        var task = Assert.Single(await ReadStoredTasksAsync(classId));
        Assert.Equal(TaskSource.Teacher, task.Source);
        Assert.Equal("Mokyklos kiemas", task.Title);
        Assert.Equal("Žvirblis", task.CorrectAnswer);
        Assert.Equal(2, task.DifficultyGroup);
    }

    [Fact]
    public async Task PostTasks_WithSourceAndTypeInBody_IgnoresThemAndSavesTeacherTask()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        var body = new
        {
            title = "Kiemas",
            question = "Klausimas?",
            correctAnswer = "Atsakymas",
            difficultyGroup = 1,
            noOrganismDisturbanceConfirmed = true,
            source = "Gbif",
            type = "CommonSpecies",
            classId = Guid.NewGuid()
        };

        var response = await teacher.CreateTeacherTaskAsync(classId, body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var task = Assert.Single(await ReadStoredTasksAsync(classId));
        Assert.Equal(TaskSource.Teacher, task.Source);
        Assert.Equal(TaskType.TeacherTask, task.Type);
    }

    [Fact]
    public async Task GetTasks_AfterCreatingTeacherTask_ReturnsTaskWithTeacherLabel()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        await CreateTeacherTaskAsync(teacher, classId);

        var response = await teacher.ListTasksAsync(classId);

        var task = Assert.Single((await ReadJsonAsync(response)).EnumerateArray());
        Assert.Equal("Mokytojo užduotis", task.GetProperty("label").GetString());
        Assert.Equal(2, task.GetProperty("difficultyGroup").GetInt32());
    }

    [Fact]
    public async Task PostTasks_WithInvalidFields_Returns400WithErrorsPerFieldAndSavesNothing()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        var body = new
        {
            title = "",
            question = new string('q', 501),
            correctAnswer = new string('a', 101),
            difficultyGroup = 4,
            noOrganismDisturbanceConfirmed = true
        };

        var response = await teacher.CreateTeacherTaskAsync(classId, body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await ReadJsonAsync(response)).GetProperty("errors");
        Assert.Equal("Required", errors.GetProperty("title")[0].GetString());
        Assert.Equal("TooLong", errors.GetProperty("question")[0].GetString());
        Assert.Equal("TooLong", errors.GetProperty("correctAnswer")[0].GetString());
        Assert.Equal("Invalid", errors.GetProperty("difficultyGroup")[0].GetString());
        Assert.Empty(await ReadStoredTasksAsync(classId));
    }

    [Fact]
    public async Task PostTasks_WithoutConfirmation_Returns400AndSavesNothing()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        var body = new { title = "Kiemas", question = "Klausimas?", correctAnswer = "Atsakymas", difficultyGroup = 1, noOrganismDisturbanceConfirmed = false };

        var response = await teacher.CreateTeacherTaskAsync(classId, body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await ReadJsonAsync(response)).GetProperty("errors");
        Assert.Equal("NotConfirmed", errors.GetProperty("noOrganismDisturbanceConfirmed")[0].GetString());
        Assert.Empty(await ReadStoredTasksAsync(classId));
    }

    [Fact]
    public async Task PostTasks_WithoutCsrfToken_Returns400()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();

        var response = await teacher.SendWithoutCsrfTokenAsync(HttpMethod.Post, $"/api/classes/{classId}/tasks", ValidBody);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostGbifTasks_AfterTeacherTask_StillCreatesGbifTasks()
    {
        _fixture.Gbif.UseEnoughObservations();
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        await CreateTeacherTaskAsync(teacher, classId);

        var response = await teacher.CreateGbifTasksAsync(classId, new { latitude = 55.0, longitude = 24.0, season = "Summer" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Contains(await ReadStoredTasksAsync(classId), t => t.Source == TaskSource.Teacher);
    }

    [Fact]
    public async Task DeleteTask_ForOwnTeacherTask_Returns204AndRemovesItFromTheList()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        var taskId = await CreateTeacherTaskAsync(teacher, classId);

        var response = await teacher.DeleteTaskAsync(classId, taskId);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty((await ReadJsonAsync(await teacher.ListTasksAsync(classId))).EnumerateArray());
    }

    [Fact]
    public async Task DeleteTask_ForGbifTask_Returns409AndKeepsTheTask()
    {
        _fixture.Gbif.UseEnoughObservations();
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        await teacher.CreateGbifTasksAsync(classId, new { latitude = 55.0, longitude = 24.0, season = "Summer" });
        var before = await ReadStoredTasksAsync(classId);

        var response = await teacher.DeleteTaskAsync(classId, before[0].Id.ToString());

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(before.Count, (await ReadStoredTasksAsync(classId)).Count);
    }

    [Fact]
    public async Task DeleteTask_ForTaskOfAnotherClass_Returns404AndKeepsTheTask()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        var otherClassId = await teacher.CreateAndGetIdAsync("7b");
        var taskId = await CreateTeacherTaskAsync(teacher, otherClassId);

        var response = await teacher.DeleteTaskAsync(classId, taskId);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Single(await ReadStoredTasksAsync(otherClassId));
    }

    [Fact]
    public async Task DeleteTask_ForSameTaskTwice_Returns204ThenWith404()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        var taskId = await CreateTeacherTaskAsync(teacher, classId);

        var first = await teacher.DeleteTaskAsync(classId, taskId);
        var second = await teacher.DeleteTaskAsync(classId, taskId);

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, second.StatusCode);
    }

    [Fact]
    public async Task TaskEndpoints_ForStudent_Return403()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        var taskId = await CreateTeacherTaskAsync(teacher, classId);
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);

        Assert.Equal(HttpStatusCode.Forbidden, (await student.CreateTeacherTaskAsync(classId, ValidBody)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await student.DeleteTaskAsync(classId, taskId)).StatusCode);
        Assert.Single(await ReadStoredTasksAsync(classId));
    }

    [Fact]
    public async Task TaskEndpoints_ForAnotherTeacher_Return403AndChangeNothing()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        var taskId = await CreateTeacherTaskAsync(teacher, classId);
        var otherTeacher = await ClassTestClient.LoginAsync(_fixture, RoleName.Teacher);

        Assert.Equal(HttpStatusCode.Forbidden, (await otherTeacher.CreateTeacherTaskAsync(classId, ValidBody)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await otherTeacher.DeleteTaskAsync(classId, taskId)).StatusCode);
        Assert.Single(await ReadStoredTasksAsync(classId));
    }

    [Fact]
    public async Task TaskEndpoints_ForVisitor_Return401()
    {
        var visitor = ClassTestClient.CreateAnonymous(_fixture);
        var classId = Guid.NewGuid().ToString();

        Assert.Equal(HttpStatusCode.Unauthorized,
            (await visitor.SendWithoutCsrfTokenAsync(HttpMethod.Post, $"/api/classes/{classId}/tasks", ValidBody)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await visitor.SendWithoutCsrfTokenAsync(HttpMethod.Delete, $"/api/classes/{classId}/tasks/{Guid.NewGuid()}")).StatusCode);
    }

    [Fact]
    public async Task TaskEndpoints_ForMissingClassOrTask_Return404()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        var missingClassId = Guid.NewGuid().ToString();

        Assert.Equal(HttpStatusCode.NotFound, (await teacher.CreateTeacherTaskAsync(missingClassId, ValidBody)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await teacher.DeleteTaskAsync(missingClassId, Guid.NewGuid().ToString())).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await teacher.DeleteTaskAsync(classId, Guid.NewGuid().ToString())).StatusCode);
    }
}
