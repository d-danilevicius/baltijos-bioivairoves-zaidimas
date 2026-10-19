using System.Net;
using System.Text.Json;
using Project.Api.Entities;

namespace Project.IntegrationTests.Classes;

public class ClassCodeCollisionTests : IClassFixture<ClassCodeApiFixture>
{
    private readonly ClassCodeApiFixture _fixture;

    public ClassCodeCollisionTests(ClassCodeApiFixture fixture)
    {
        _fixture = fixture;
    }

    private static async Task<string> ReadCodeAsync(HttpResponseMessage response)
    {
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("code").GetString()!;
    }

    [Fact]
    public async Task PostClasses_WhenGeneratedCodeAlreadyExists_Returns201WithAnotherCode()
    {
        var teacher = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        _fixture.Codes.Enqueue("SEQAAA");
        await teacher.CreateAsync("Pirma klasė");
        _fixture.Codes.Enqueue("SEQAAA", "SEQBBB");

        var response = await teacher.CreateAsync("Antra klasė");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("SEQBBB", await ReadCodeAsync(response));
    }

    [Fact]
    public async Task PostClasses_WhenTwoTeachersCreateAtTheSameTimeWithTheSameCode_BothGetDifferentCodes()
    {
        var first = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        var second = await ClassesTestClient.LoginAsync(_fixture, RoleNames.Teacher);
        _fixture.Codes.Enqueue("RACEAA", "RACEAA", "RACEBB");

        var responses = await Task.WhenAll(first.CreateAsync("Pirma"), second.CreateAsync("Antra"));

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.Created, r.StatusCode));
        var codes = new[] { await ReadCodeAsync(responses[0]), await ReadCodeAsync(responses[1]) };
        Assert.Equal(new[] { "RACEAA", "RACEBB" }, codes.OrderBy(c => c).ToArray());
    }
}
