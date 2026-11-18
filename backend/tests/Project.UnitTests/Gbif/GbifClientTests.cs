using System.Net;
using System.Web;
using Microsoft.Extensions.Logging.Abstractions;
using Project.Api.Entities;
using Project.Api.Integrations.Gbif;
using Xunit;

namespace Project.UnitTests.Gbif;

public class GbifClientTests
{
    private const string TwoOccurrences = """
        {
          "offset": 0, "limit": 300, "endOfRecords": true, "count": 2,
          "results": [
            {
              "key": 1001, "species": "Parus major", "class": "Aves", "family": "Paridae",
              "decimalLatitude": 55.7, "decimalLongitude": 21.1, "locality": "Palanga",
              "eventDate": "2024-06-15T10:30:00", "datasetName": "iNaturalist", "license": "CC0_1_0",
              "iucnRedListCategory": "LC"
            },
            { "key": 1002 }
          ]
        }
        """;

    [Fact]
    public async Task SearchAsync_SendsCountryCoordinatesMonthsSquareAndLimit()
    {
        var handler = FakeHttpMessageHandler.Returning(TwoOccurrences);
        var client = CreateClient(handler);

        await client.SearchAsync(55.7, 21.1, 10, Season.Summer, CancellationToken.None);

        var request = Assert.Single(handler.Requests);
        Assert.Equal("https", request.RequestUri!.Scheme);
        Assert.EndsWith("/occurrence/search", request.RequestUri.AbsolutePath);
        var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
        Assert.Equal("LT", query["country"]);
        Assert.Equal("true", query["hasCoordinate"]);
        Assert.Equal(new[] { "6", "7", "8" }, query.GetValues("month"));
        Assert.Equal("300", query["limit"]);
        Assert.StartsWith("POLYGON((", query["geometry"]);
    }

    [Fact]
    public async Task SearchAsync_ForWinter_SendsDecemberJanuaryAndFebruary()
    {
        var handler = FakeHttpMessageHandler.Returning(TwoOccurrences);
        var client = CreateClient(handler);

        await client.SearchAsync(55.7, 21.1, 10, Season.Winter, CancellationToken.None);

        var query = HttpUtility.ParseQueryString(handler.Requests[0].RequestUri!.Query);
        Assert.Equal(new[] { "12", "1", "2" }, query.GetValues("month"));
    }

    [Fact]
    public async Task SearchAsync_MapsTheFieldsOfAnOccurrence()
    {
        var client = CreateClient(FakeHttpMessageHandler.Returning(TwoOccurrences));

        var occurrences = await client.SearchAsync(55.7, 21.1, 10, Season.Summer, CancellationToken.None);

        var first = occurrences[0];
        Assert.Equal(1001, first.Key);
        Assert.Equal("Parus major", first.Species);
        Assert.Equal("Aves", first.GbifClass);
        Assert.Equal("Paridae", first.Family);
        Assert.Equal(55.7, first.Latitude);
        Assert.Equal(21.1, first.Longitude);
        Assert.Equal("Palanga", first.Locality);
        Assert.Equal(new DateOnly(2024, 6, 15), first.EventDate);
        Assert.Equal("iNaturalist", first.DatasetName);
        Assert.Equal("CC0_1_0", first.License);
        Assert.Equal("LC", first.IucnCategory);
    }

    [Fact]
    public async Task SearchAsync_WhenFieldsAreMissing_ReturnsEmptyValues()
    {
        var client = CreateClient(FakeHttpMessageHandler.Returning(TwoOccurrences));

        var occurrences = await client.SearchAsync(55.7, 21.1, 10, Season.Summer, CancellationToken.None);

        var second = occurrences[1];
        Assert.Equal(1002, second.Key);
        Assert.Null(second.Species);
        Assert.Null(second.Latitude);
        Assert.Null(second.EventDate);
        Assert.Null(second.IucnCategory);
    }

    [Fact]
    public async Task SearchAsync_WhenGbifReturnsServerError_ThrowsGbifUnavailable()
    {
        var client = CreateClient(FakeHttpMessageHandler.Returning("{}", HttpStatusCode.InternalServerError));

        await Assert.ThrowsAsync<GbifUnavailableException>(
            () => client.SearchAsync(55.7, 21.1, 10, Season.Summer, CancellationToken.None));
    }

    [Fact]
    public async Task SearchAsync_WhenNetworkFails_ThrowsGbifUnavailable()
    {
        var handler = new FakeHttpMessageHandler((_, _) => throw new HttpRequestException("network down"));
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<GbifUnavailableException>(
            () => client.SearchAsync(55.7, 21.1, 10, Season.Summer, CancellationToken.None));
    }

    [Fact]
    public async Task SearchAsync_WhenGbifDoesNotAnswerInTime_ThrowsGbifUnavailable()
    {
        var handler = new FakeHttpMessageHandler((_, _) => throw new TaskCanceledException("timeout"));
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<GbifUnavailableException>(
            () => client.SearchAsync(55.7, 21.1, 10, Season.Summer, CancellationToken.None));
    }

    [Fact]
    public async Task SearchAsync_WhenResponseIsNotValidJson_ThrowsGbifUnavailable()
    {
        var client = CreateClient(FakeHttpMessageHandler.Returning("not json"));

        await Assert.ThrowsAsync<GbifUnavailableException>(
            () => client.SearchAsync(55.7, 21.1, 10, Season.Summer, CancellationToken.None));
    }

    [Fact]
    public async Task SearchAsync_WhenCallerCancels_ThrowsOperationCanceledNotGbifUnavailable()
    {
        using var cancellation = new CancellationTokenSource();
        var handler = new FakeHttpMessageHandler((_, token) =>
        {
            cancellation.Cancel();
            token.ThrowIfCancellationRequested();
            return Task.FromResult(new HttpResponseMessage());
        });
        var client = CreateClient(handler);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.SearchAsync(55.7, 21.1, 10, Season.Summer, cancellation.Token));
    }

    private static GbifClient CreateClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.gbif.org/v1/") };
        return new GbifClient(httpClient, NullLogger<GbifClient>.Instance);
    }
}
