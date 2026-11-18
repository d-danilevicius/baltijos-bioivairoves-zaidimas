using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.Entities;
using Project.IntegrationTests.Classes;

namespace Project.IntegrationTests.Gbif;

public class GbifTaskEndpointTests : IClassFixture<GbifApiFixture>
{
    private static readonly object ValidBody = new { latitude = 55.0, longitude = 24.0, season = "Summer" };

    private readonly GbifApiFixture _fixture;

    public GbifTaskEndpointTests(GbifApiFixture fixture)
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

    private async Task<(int Observations, int Tasks, Season? Season)> ReadStoredAsync(string classId)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var id = Guid.Parse(classId);
        var season = (await db.Classes.AsNoTracking().SingleAsync(c => c.Id == id)).Season;
        return (await db.Observations.CountAsync(o => o.ClassId == id), await db.ClassTasks.CountAsync(t => t.ClassId == id), season);
    }

    [Fact]
    public async Task PostGbifTasks_WithEnoughObservations_Returns201AndSavesTasksObservationsAndLocation()
    {
        _fixture.Gbif.UseEnoughObservations();
        var (teacher, classId) = await CreateTeacherWithClassAsync();

        var response = await teacher.CreateGbifTasksAsync(classId, ValidBody);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = (await ReadJsonAsync(response)).GetProperty("createdCount").GetInt32();
        var stored = await ReadStoredAsync(classId);
        Assert.Equal(created, stored.Tasks);
        Assert.Equal(22, stored.Observations);
        Assert.Equal(Season.Summer, stored.Season);
        var classJson = await ReadJsonAsync(await teacher.GetAsync(classId));
        Assert.Equal("Summer", classJson.GetProperty("season").GetString());
        Assert.Equal(55.0, classJson.GetProperty("latitude").GetDouble());
    }

    [Fact]
    public async Task GetTasks_AfterCreatingGbifTasks_ReturnsTasksOfAllFourTypesWithGbifLabel()
    {
        _fixture.Gbif.UseEnoughObservations();
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        await teacher.CreateGbifTasksAsync(classId, ValidBody);

        var response = await teacher.ListTasksAsync(classId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tasks = (await ReadJsonAsync(response)).EnumerateArray().ToList();
        Assert.Equal(4, tasks.Select(t => t.GetProperty("type").GetString()).Distinct().Count());
        Assert.All(tasks, t => Assert.Equal("GBIF", t.GetProperty("label").GetString()));
    }

    [Fact]
    public async Task GetTasks_ForClassWithoutTasks_ReturnsEmptyList()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();

        var response = await teacher.ListTasksAsync(classId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty((await ReadJsonAsync(response)).EnumerateArray());
    }

    [Fact]
    public async Task PostGbifTasks_WithFewerThanTenObservations_Returns422AndSavesNothing()
    {
        _fixture.Gbif.UseObservations(Season.Summer, 9);
        var (teacher, classId) = await CreateTeacherWithClassAsync();

        var response = await teacher.CreateGbifTasksAsync(classId, ValidBody);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var stored = await ReadStoredAsync(classId);
        Assert.Equal((0, 0, (Season?)null), stored);
    }

    [Fact]
    public async Task PostGbifTasks_WhenGbifFails_Returns502AndSavesNothing()
    {
        _fixture.Gbif.UseEnoughObservations();
        _fixture.Gbif.Fails = true;
        var (teacher, classId) = await CreateTeacherWithClassAsync();

        var response = await teacher.CreateGbifTasksAsync(classId, ValidBody);

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        Assert.Equal((0, 0, (Season?)null), await ReadStoredAsync(classId));
    }

    [Fact]
    public async Task PostGbifTasks_ForClassThatAlreadyHasGbifTasks_Returns409AndKeepsTheFirstTasks()
    {
        _fixture.Gbif.UseEnoughObservations();
        var (teacher, classId) = await CreateTeacherWithClassAsync();
        await teacher.CreateGbifTasksAsync(classId, ValidBody);
        var before = await ReadStoredAsync(classId);

        var response = await teacher.CreateGbifTasksAsync(classId, new { latitude = 55.0, longitude = 24.0, season = "Winter" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(before, await ReadStoredAsync(classId));
    }

    [Fact]
    public async Task PostGbifTasks_WhenTwoRequestsRunAtTheSameTime_CreatesOneSetOfTasks()
    {
        _fixture.Gbif.UseEnoughObservations();
        _fixture.Gbif.Delay = TimeSpan.FromMilliseconds(300);
        var (teacher, classId) = await CreateTeacherWithClassAsync();

        var responses = await teacher.CreateGbifTasksAtTheSameTimeAsync(classId, ValidBody, 2);

        var statuses = responses.Select(r => r.StatusCode).OrderBy(s => s).ToList();
        Assert.Equal(new[] { HttpStatusCode.Created, HttpStatusCode.Conflict }, statuses);
        var created = (await ReadJsonAsync(responses.Single(r => r.StatusCode == HttpStatusCode.Created))).GetProperty("createdCount").GetInt32();
        var stored = await ReadStoredAsync(classId);
        Assert.Equal(22, stored.Observations);
        Assert.Equal(created, stored.Tasks);
    }

    [Fact]
    public async Task PostGbifTasks_WithPointOutsideLithuania_Returns400WithErrorForLatitude()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();

        var response = await teacher.CreateGbifTasksAsync(classId, new { latitude = 50.0, longitude = 24.0, season = "Summer" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await ReadJsonAsync(response)).GetProperty("errors");
        Assert.Equal("OutsideLithuania", errors.GetProperty("latitude")[0].GetString());
    }

    [Fact]
    public async Task PostGbifTasks_WithoutSeason_Returns400WithRequiredErrorForSeason()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();

        var response = await teacher.CreateGbifTasksAsync(classId, new { latitude = 55.0, longitude = 24.0 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await ReadJsonAsync(response)).GetProperty("errors");
        Assert.Equal("Required", errors.GetProperty("season")[0].GetString());
    }

    [Fact]
    public async Task PostGbifTasks_WithUnknownSeason_Returns400()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();

        var response = await teacher.CreateGbifTasksAsync(classId, new { latitude = 55.0, longitude = 24.0, season = "Monsoon" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostGbifTasks_WithoutCsrfToken_Returns400()
    {
        var (teacher, classId) = await CreateTeacherWithClassAsync();

        var response = await teacher.SendWithoutCsrfTokenAsync(HttpMethod.Post, $"/api/classes/{classId}/gbif-tasks", ValidBody);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GbifEndpoints_ForStudent_Return403()
    {
        _fixture.Gbif.UseEnoughObservations();
        var (_, classId) = await CreateTeacherWithClassAsync();
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);

        Assert.Equal(HttpStatusCode.Forbidden, (await student.CreateGbifTasksAsync(classId, ValidBody)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await student.ListTasksAsync(classId)).StatusCode);
    }

    [Fact]
    public async Task GbifEndpoints_ForAnotherTeacher_Return403AndSaveNothing()
    {
        _fixture.Gbif.UseEnoughObservations();
        var (_, classId) = await CreateTeacherWithClassAsync();
        var otherTeacher = await ClassTestClient.LoginAsync(_fixture, RoleName.Teacher);

        Assert.Equal(HttpStatusCode.Forbidden, (await otherTeacher.CreateGbifTasksAsync(classId, ValidBody)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await otherTeacher.ListTasksAsync(classId)).StatusCode);
        Assert.Equal((0, 0, (Season?)null), await ReadStoredAsync(classId));
    }

    [Fact]
    public async Task GbifEndpoints_ForVisitor_Return401()
    {
        var visitor = ClassTestClient.CreateAnonymous(_fixture);
        var classId = Guid.NewGuid().ToString();

        Assert.Equal(HttpStatusCode.Unauthorized, (await visitor.ListTasksAsync(classId)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized,
            (await visitor.SendWithoutCsrfTokenAsync(HttpMethod.Post, $"/api/classes/{classId}/gbif-tasks", ValidBody)).StatusCode);
    }

    [Fact]
    public async Task GbifEndpoints_ForMissingClass_Return404()
    {
        _fixture.Gbif.UseEnoughObservations();
        var teacher = await ClassTestClient.LoginAsync(_fixture, RoleName.Teacher);
        var classId = Guid.NewGuid().ToString();

        Assert.Equal(HttpStatusCode.NotFound, (await teacher.CreateGbifTasksAsync(classId, ValidBody)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await teacher.ListTasksAsync(classId)).StatusCode);
    }
}
