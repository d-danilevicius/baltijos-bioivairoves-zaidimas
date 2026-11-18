using System.Globalization;
using System.Text.Json;
using Project.Api.Entities;

namespace Project.Api.Integrations.Gbif;

public class GbifClient : IGbifClient
{
    private const double KilometresPerDegree = 111.195;
    private const int PageSize = 300;

    private readonly HttpClient _httpClient;
    private readonly ILogger<GbifClient> _logger;

    public GbifClient(HttpClient httpClient, ILogger<GbifClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<GbifOccurrence>> SearchAsync(
        double latitude,
        double longitude,
        double radiusKm,
        Season season,
        CancellationToken cancellationToken)
    {
        var url = "occurrence/search?" + BuildQuery(latitude, longitude, radiusKm, season);

        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return GbifOccurrenceParser.Parse(json);
        }
        catch (Exception exception) when (IsGbifFailure(exception, cancellationToken))
        {
            _logger.LogWarning(exception, "GBIF request failed for season {Season}", season);
            throw new GbifUnavailableException("GBIF is not available.", exception);
        }
    }

    // A cancelled request of the caller is not a GBIF failure; a timeout of the HttpClient is
    private static bool IsGbifFailure(Exception exception, CancellationToken cancellationToken) =>
        exception is HttpRequestException or JsonException or KeyNotFoundException or InvalidOperationException or FormatException
        || (exception is OperationCanceledException && !cancellationToken.IsCancellationRequested);

    private static string BuildQuery(double latitude, double longitude, double radiusKm, Season season)
    {
        var parameters = new List<string>
        {
            "country=LT",
            "hasCoordinate=true",
            "hasGeospatialIssue=false"
        };
        parameters.AddRange(season.Months().Select(month => $"month={month}"));
        parameters.Add("geometry=" + Uri.EscapeDataString(BuildSquare(latitude, longitude, radiusKm)));
        parameters.Add($"limit={PageSize}");
        return string.Join("&", parameters);
    }

    // A counter-clockwise square (WKT) that contains the circle of the radius around the point
    private static string BuildSquare(double latitude, double longitude, double radiusKm)
    {
        var latitudeDelta = radiusKm / KilometresPerDegree;
        var longitudeDelta = latitudeDelta / Math.Cos(latitude * Math.PI / 180);
        var west = Format(longitude - longitudeDelta);
        var east = Format(longitude + longitudeDelta);
        var south = Format(latitude - latitudeDelta);
        var north = Format(latitude + latitudeDelta);

        return $"POLYGON(({west} {south},{east} {south},{east} {north},{west} {north},{west} {south}))";
    }

    private static string Format(double value) => value.ToString("0.#####", CultureInfo.InvariantCulture);
}
