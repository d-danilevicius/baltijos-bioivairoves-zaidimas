using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.Entities;
using Project.IntegrationTests.Classes;
using Project.IntegrationTests.Gbif;

namespace Project.IntegrationTests.ObservationCards;

public class StudentTaskDetailsEndpointTests : IClassFixture<GbifApiFixture>
{
    private const string TeacherQuestion = "Kokį paukštį matote ant stogo?";
    private const string CorrectAnswer = "Žvirblis";
    private const string Species = "Parus major";
    private const string Latitude = "55.123456";
    private const string Longitude = "24.654321";

    private readonly GbifApiFixture _fixture;

    public StudentTaskDetailsEndpointTests(GbifApiFixture fixture)
    {
        _fixture = fixture;
    }

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

    private static async Task<string> CreateTeacherTaskAsync(ClassTestClient teacher, string classId, int group)
    {
        var response = await teacher.CreateTeacherTaskAsync(classId, new
        {
            title = "Mokyklos kiemas",
            question = TeacherQuestion,
            correctAnswer = CorrectAnswer,
            difficultyGroup = group,
            noOrganismDisturbanceConfirmed = true
        });
        response.EnsureSuccessStatusCode();
        return (await ReadJsonAsync(response)).GetProperty("id").GetString()!;
    }

    // The GBIF client is replaced by a fake in the tests, so the observation and the task are stored directly
    private async Task<string> AddGbifTaskAsync(string classId, string? iucnCategory)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var observation = new Observation
        {
            Id = Guid.NewGuid(),
            ClassId = Guid.Parse(classId),
            GbifKey = 4242,
            Species = Species,
            Latitude = double.Parse(Latitude, System.Globalization.CultureInfo.InvariantCulture),
            Longitude = double.Parse(Longitude, System.Globalization.CultureInfo.InvariantCulture),
            Locality = "Verkių regioninis parkas",
            ObservedOn = new DateOnly(2026, 5, 12),
            DatasetName = "iNaturalist",
            License = "CC_BY_4_0",
            IucnCategory = iucnCategory
        };
        var task = new ClassTask
        {
            Id = Guid.NewGuid(),
            ClassId = Guid.Parse(classId),
            Type = TaskType.CommonSpecies,
            Source = TaskSource.Gbif,
            DifficultyGroup = 1,
            Question = "Kuri rūšis tarp gautų stebėjimų pastebėta dažniausiai?",
            CorrectAnswer = CorrectAnswer,
            Options = new[] { "A", "B" },
            ObservationId = observation.Id
        };
        db.Observations.Add(observation);
        db.ClassTasks.Add(task);
        await db.SaveChangesAsync();
        return task.Id.ToString();
    }

    [Fact]
    public async Task GetMyTask_ForGbifTaskOfSpeciesThatIsNotProtected_Returns200WithCardAndCoordinates()
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await AddGbifTaskAsync(classId, iucnCategory: null);

        var response = await student.GetMyTaskAsync(taskId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var card = (await ReadJsonAsync(response)).GetProperty("observation");
        Assert.Equal("Patvirtintas GBIF stebėjimas", card.GetProperty("label").GetString());
        Assert.Equal("iNaturalist", card.GetProperty("datasetName").GetString());
        Assert.Equal("CC_BY_4_0", card.GetProperty("license").GetString());
        Assert.Equal("https://www.gbif.org/occurrence/4242", card.GetProperty("gbifUrl").GetString());
        Assert.Equal("Verkių regioninis parkas", card.GetProperty("locality").GetString());
        Assert.Equal(55.123456, card.GetProperty("latitude").GetDouble());
        Assert.False(card.GetProperty("coordinatesHidden").GetBoolean());
    }

    [Theory]
    [InlineData("VU")]
    [InlineData("EN")]
    [InlineData("CR")]
    public async Task GetMyTask_ForProtectedSpecies_DoesNotReturnTheCoordinates(string iucnCategory)
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await AddGbifTaskAsync(classId, iucnCategory);

        var response = await student.GetMyTaskAsync(taskId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var text = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain(Latitude, text);
        Assert.DoesNotContain(Longitude, text);
        var card = JsonDocument.Parse(text).RootElement.GetProperty("observation");
        Assert.False(card.TryGetProperty("latitude", out _));
        Assert.False(card.TryGetProperty("longitude", out _));
        Assert.Equal("Verkių regioninis parkas", card.GetProperty("locality").GetString());
        Assert.True(card.GetProperty("coordinatesHidden").GetBoolean());
    }

    [Fact]
    public async Task GetMyTask_ForGbifTask_DoesNotReturnTheCorrectAnswerTheSpeciesNameOrTheObservationDate()
    {
        var (_, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await AddGbifTaskAsync(classId, iucnCategory: null);

        var text = await (await student.GetMyTaskAsync(taskId)).Content.ReadAsStringAsync();

        Assert.DoesNotContain(CorrectAnswer, text);
        Assert.DoesNotContain(Species, text);
        Assert.DoesNotContain("2026-05-12", text);
        Assert.DoesNotContain("correctAnswer", text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetMyTask_ForTeacherTask_Returns200WithTitleAndQuestionAndNoCard()
    {
        var (teacher, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await CreateTeacherTaskAsync(teacher, classId, 1);

        var response = await student.GetMyTaskAsync(taskId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal("Mokytojo užduotis", json.GetProperty("label").GetString());
        Assert.Equal("Mokyklos kiemas", json.GetProperty("title").GetString());
        Assert.Equal(TeacherQuestion, json.GetProperty("question").GetString());
        Assert.False(json.TryGetProperty("observation", out var card) && card.ValueKind != JsonValueKind.Null);
        Assert.DoesNotContain(CorrectAnswer, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetMyTask_ForTaskOfLockedGroup_Returns403()
    {
        var (teacher, student, classId) = await CreateClassWithStudentAsync();
        await CreateTeacherTaskAsync(teacher, classId, 1);
        var lockedTaskId = await CreateTeacherTaskAsync(teacher, classId, 2);

        var response = await student.GetMyTaskAsync(lockedTaskId);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetMyTask_ForTaskOfAnotherClass_Returns404()
    {
        var (_, student, _) = await CreateClassWithStudentAsync();
        var (otherTeacher, _, otherClassId) = await CreateClassWithStudentAsync();
        var otherTaskId = await CreateTeacherTaskAsync(otherTeacher, otherClassId, 1);

        var response = await student.GetMyTaskAsync(otherTaskId);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMyTask_ForMissingTask_Returns404()
    {
        var (_, student, _) = await CreateClassWithStudentAsync();

        var response = await student.GetMyTaskAsync(Guid.NewGuid().ToString());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMyTask_AfterTeacherDeletesTheTask_Returns404()
    {
        var (teacher, student, classId) = await CreateClassWithStudentAsync();
        var taskId = await CreateTeacherTaskAsync(teacher, classId, 1);
        (await teacher.DeleteTaskAsync(classId, taskId)).EnsureSuccessStatusCode();

        var response = await student.GetMyTaskAsync(taskId);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(RoleName.Teacher)]
    [InlineData(RoleName.Administrator)]
    public async Task GetMyTask_ForTeacherOrAdministrator_Returns403(string role)
    {
        var client = await ClassTestClient.LoginAsync(_fixture, role);

        var response = await client.GetMyTaskAsync(Guid.NewGuid().ToString());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetMyTask_ForVisitor_Returns401()
    {
        var visitor = ClassTestClient.CreateAnonymous(_fixture);

        var response = await visitor.GetMyTaskAsync(Guid.NewGuid().ToString());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
