using System.Net;
using System.Text.Json;
using Project.Api.Entities;
using Project.IntegrationTests.Login;

namespace Project.IntegrationTests.Classes;

public class JoinClassEndpointTests : IClassFixture<LoginApiFixture>
{
    private readonly LoginApiFixture _fixture;

    public JoinClassEndpointTests(LoginApiFixture fixture)
    {
        _fixture = fixture;
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.Clone();
    }

    private async Task<string> CreateClassCodeAsync(string name)
    {
        var teacher = await ClassTestClient.LoginAsync(_fixture, RoleName.Teacher);
        return await teacher.CreateAndGetCodeAsync(name);
    }

    [Fact]
    public async Task PostJoin_WithValidCode_Returns200WithClassName()
    {
        var code = await CreateClassCodeAsync("7a");
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);

        var response = await student.JoinAsync(code);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal("7a", json.GetProperty("className").GetString());
        Assert.False(json.GetProperty("alreadyMember").GetBoolean());
    }

    [Fact]
    public async Task PostJoin_WithLowercaseCodeAndSpaces_Returns200()
    {
        var code = await CreateClassCodeAsync("7a");
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);

        var response = await student.JoinAsync($" {code[..3].ToLowerInvariant()} {code[3..].ToLowerInvariant()} ");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostJoin_WithUnknownCode_Returns400WithNotFoundErrorForCode()
    {
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);

        var response = await student.JoinAsync("ZZZZZ9");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await ReadJsonAsync(response)).GetProperty("errors");
        Assert.Contains("NotFound", errors.GetProperty("code").EnumerateArray().Select(x => x.GetString()));
    }

    [Theory]
    [InlineData("", "Required")]
    [InlineData("ABC12", "InvalidFormat")]
    [InlineData("ABC1234", "InvalidFormat")]
    [InlineData("ABC-12", "InvalidFormat")]
    public async Task PostJoin_WithInvalidCode_Returns400WithErrorForCode(string code, string expectedError)
    {
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);

        var response = await student.JoinAsync(code);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await ReadJsonAsync(response)).GetProperty("errors");
        Assert.Contains(expectedError, errors.GetProperty("code").EnumerateArray().Select(x => x.GetString()));
    }

    [Fact]
    public async Task PostJoin_WhenClassWasDeleted_Returns400WithNotFoundErrorForCode()
    {
        var teacher = await ClassTestClient.LoginAsync(_fixture, RoleName.Teacher);
        var id = await teacher.CreateAndGetIdAsync("7a");
        var code = JsonDocument.Parse(await (await teacher.GetAsync(id)).Content.ReadAsStringAsync()).RootElement.GetProperty("code").GetString()!;
        await teacher.DeleteAsync(id);
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);

        var response = await student.JoinAsync(code);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostJoin_WhenStudentIsInAnotherClassWithoutConfirmation_Returns409WithBothClassNames()
    {
        var oldCode = await CreateClassCodeAsync("7b");
        var newCode = await CreateClassCodeAsync("7a");
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);
        await student.JoinAsync(oldCode);

        var response = await student.JoinAsync(newCode);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal("7b", json.GetProperty("currentClassName").GetString());
        Assert.Equal("7a", json.GetProperty("newClassName").GetString());
    }

    [Fact]
    public async Task PostJoin_WhenStudentIsInAnotherClassWithConfirmation_Returns200AndChangesTheClass()
    {
        var oldCode = await CreateClassCodeAsync("7b");
        var newCode = await CreateClassCodeAsync("7a");
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);
        await student.JoinAsync(oldCode);

        var response = await student.JoinAsync(newCode, confirmChange: true);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("7a", (await ReadJsonAsync(response)).GetProperty("className").GetString());
        var again = await student.JoinAsync(newCode);
        Assert.True((await ReadJsonAsync(again)).GetProperty("alreadyMember").GetBoolean());
        var backToOld = await student.JoinAsync(oldCode);
        Assert.Equal(HttpStatusCode.Conflict, backToOld.StatusCode);
    }

    [Fact]
    public async Task PostJoin_WhenStudentIsAlreadyInThisClass_Returns200WithAlreadyMember()
    {
        var code = await CreateClassCodeAsync("7a");
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);
        await student.JoinAsync(code);

        var response = await student.JoinAsync(code);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True((await ReadJsonAsync(response)).GetProperty("alreadyMember").GetBoolean());
    }

    [Fact]
    public async Task PostJoin_WhenSameStudentJoinsTheSameClassTwiceAtTheSameTime_BothRequestsReturn200()
    {
        var code = await CreateClassCodeAsync("7a");
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);

        var responses = await student.JoinAtTheSameTimeAsync(code, 2);

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
    }

    [Theory]
    [InlineData(RoleName.Teacher)]
    [InlineData(RoleName.Administrator)]
    public async Task PostJoin_WhenUserIsNotStudent_Returns403(string role)
    {
        var code = await CreateClassCodeAsync("7a");
        var user = await ClassTestClient.LoginAsync(_fixture, role);

        var response = await user.JoinAsync(code);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PostJoin_WhenVisitorIsNotLoggedIn_Returns401()
    {
        var visitor = ClassTestClient.CreateAnonymous(_fixture);

        var response = await visitor.SendWithoutCsrfTokenAsync(HttpMethod.Post, "/api/classes/join", new { code = "AAAAAA" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostJoin_WithoutCsrfToken_Returns400()
    {
        var code = await CreateClassCodeAsync("7a");
        var student = await ClassTestClient.LoginAsync(_fixture, RoleName.Student);

        var response = await student.SendWithoutCsrfTokenAsync(HttpMethod.Post, "/api/classes/join", new { code });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
