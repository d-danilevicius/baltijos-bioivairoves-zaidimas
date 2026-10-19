using System.Net;
using System.Text.Json;
using Project.Api.Entities;
using Project.IntegrationTests.Login;

namespace Project.IntegrationTests.Classes;

public class ClassEndpointTests : IClassFixture<LoginApiFixture>
{
    private readonly LoginApiFixture _fixture;

    public ClassEndpointTests(LoginApiFixture fixture)
    {
        _fixture = fixture;
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.Clone();
    }

    [Fact]
    public async Task PostClasses_WithValidName_Returns201WithSixCharacterCode()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);

        var response = await teacher.CreateAsync("7a klasė");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal("7a klasė", json.GetProperty("name").GetString());
        Assert.Matches("^[A-Z0-9]{6}$", json.GetProperty("code").GetString()!);
    }

    [Fact]
    public async Task PostClasses_WithFiftyCharactersAndSpacesAround_Returns201WithTrimmedName()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        var name = new string('a', 50);

        var response = await teacher.CreateAsync($"  {name}  ");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(name, (await ReadJsonAsync(response)).GetProperty("name").GetString());
    }

    [Fact]
    public async Task PostClasses_WithEmptyName_Returns400WithRequiredErrorForName()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);

        var response = await teacher.CreateAsync("   ");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await ReadJsonAsync(response)).GetProperty("errors");
        Assert.Contains("Required", errors.GetProperty("name").EnumerateArray().Select(x => x.GetString()));
    }

    [Fact]
    public async Task PostClasses_WithFiftyOneCharacterName_Returns400WithTooLongErrorForName()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);

        var response = await teacher.CreateAsync(new string('a', 51));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await ReadJsonAsync(response)).GetProperty("errors");
        Assert.Contains("TooLong", errors.GetProperty("name").EnumerateArray().Select(x => x.GetString()));
    }

    [Fact]
    public async Task PostClasses_WithoutCsrfToken_Returns400()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);

        var response = await teacher.SendWithoutCsrfTokenAsync(HttpMethod.Post, "/api/classes", new { name = "7a" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetClasses_AfterCreatingClass_ListsOnlyClassesOfLoggedInTeacher()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        var otherTeacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        await teacher.CreateAsync("Mano klasė");
        await otherTeacher.CreateAsync("Kito mokytojo klasė");

        var response = await teacher.ListAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var names = (await ReadJsonAsync(response)).EnumerateArray().Select(c => c.GetProperty("name").GetString()).ToList();
        Assert.Contains("Mano klasė", names);
        Assert.DoesNotContain("Kito mokytojo klasė", names);
    }

    [Fact]
    public async Task GetClassById_ForOwnClass_Returns200WithCode()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        var id = await teacher.CreateAndGetIdAsync("7a");

        var response = await teacher.GetAsync(id);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal(id, json.GetProperty("id").GetString());
        Assert.Matches("^[A-Z0-9]{6}$", json.GetProperty("code").GetString()!);
    }

    [Fact]
    public async Task GetClassById_ForClassOfAnotherTeacher_Returns403()
    {
        var owner = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        var otherTeacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        var id = await owner.CreateAndGetIdAsync("7a");

        var response = await otherTeacher.GetAsync(id);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetClassById_WhenClassDoesNotExist_Returns404()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);

        var response = await teacher.GetAsync(Guid.NewGuid().ToString());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteClass_ForOwnClass_Returns204AndClassIsNoLongerListed()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        var id = await teacher.CreateAndGetIdAsync("Ištrinama klasė");

        var response = await teacher.DeleteAsync(id);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var ids = (await ReadJsonAsync(await teacher.ListAsync())).EnumerateArray().Select(c => c.GetProperty("id").GetString());
        Assert.DoesNotContain(id, ids);
    }

    [Fact]
    public async Task DeleteClass_ForClassOfAnotherTeacher_Returns403AndKeepsClass()
    {
        var owner = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        var otherTeacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        var id = await owner.CreateAndGetIdAsync("7a");

        var response = await otherTeacher.DeleteAsync(id);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await owner.GetAsync(id)).StatusCode);
    }

    [Fact]
    public async Task DeleteClass_WhenClassDoesNotExist_Returns404()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);

        var response = await teacher.DeleteAsync(Guid.NewGuid().ToString());

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteClass_WithoutCsrfToken_Returns400AndKeepsClass()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        var id = await teacher.CreateAndGetIdAsync("7a");

        var response = await teacher.SendWithoutCsrfTokenAsync(HttpMethod.Delete, $"/api/classes/{id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await teacher.GetAsync(id)).StatusCode);
    }

    [Fact]
    public async Task ClassEndpoints_AsStudent_Return403WithAccessDenied()
    {
        var student = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Student);
        var id = Guid.NewGuid().ToString();

        var responses = new[]
        {
            await student.ListAsync(),
            await student.CreateAsync("7a"),
            await student.GetAsync(id),
            await student.DeleteAsync(id)
        };

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode));
        var json = await ReadJsonAsync(responses[0]);
        Assert.Equal("Prieiga uždrausta", json.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task ClassEndpoints_WithoutSession_Return401()
    {
        var visitor = ClassesTestClient.CreateAnonymous(_fixture);
        var id = Guid.NewGuid().ToString();

        var responses = new[]
        {
            await visitor.ListAsync(),
            await visitor.GetAsync(id),
            await visitor.SendWithoutCsrfTokenAsync(HttpMethod.Post, "/api/classes", new { name = "7a" }),
            await visitor.SendWithoutCsrfTokenAsync(HttpMethod.Delete, $"/api/classes/{id}")
        };

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode));
    }
}
