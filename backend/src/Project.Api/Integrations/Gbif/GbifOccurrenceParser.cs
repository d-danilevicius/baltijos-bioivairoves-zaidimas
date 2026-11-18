using System.Globalization;
using System.Text.Json;

namespace Project.Api.Integrations.Gbif;

public static class GbifOccurrenceParser
{
    public static IReadOnlyList<GbifOccurrence> Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("results", out var results) || results.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException("The GBIF response has no results array.");
        }

        return results.EnumerateArray().Select(ToOccurrence).ToList();
    }

    private static GbifOccurrence ToOccurrence(JsonElement item) => new(
        item.GetProperty("key").GetInt64(),
        ReadString(item, "species"),
        ReadString(item, "class"),
        ReadString(item, "family"),
        ReadNumber(item, "decimalLatitude"),
        ReadNumber(item, "decimalLongitude"),
        ReadString(item, "locality"),
        ReadDate(item, "eventDate"),
        ReadString(item, "datasetName"),
        ReadString(item, "license"),
        ReadString(item, "iucnRedListCategory"));

    private static string? ReadString(JsonElement item, string name) =>
        item.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    private static double? ReadNumber(JsonElement item, string name) =>
        item.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetDouble() : null;

    // GBIF gives a date, a date with time or a range; the first ten characters are the date
    private static DateOnly? ReadDate(JsonElement item, string name)
    {
        var text = ReadString(item, name);
        if (text is null || text.Length < 10)
        {
            return null;
        }

        return DateOnly.TryParseExact(text[..10], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : null;
    }
}
