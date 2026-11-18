namespace Project.Api.Entities;

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter
}

public static class SeasonExtensions
{
    // VT-012: seasons are defined by months
    public static IReadOnlyList<int> Months(this Season season) => season switch
    {
        Season.Spring => new[] { 3, 4, 5 },
        Season.Summer => new[] { 6, 7, 8 },
        Season.Autumn => new[] { 9, 10, 11 },
        Season.Winter => new[] { 12, 1, 2 },
        _ => throw new ArgumentOutOfRangeException(nameof(season), season, "Unknown season")
    };

    public static Season Opposite(this Season season) => season switch
    {
        Season.Spring => Season.Autumn,
        Season.Summer => Season.Winter,
        Season.Autumn => Season.Spring,
        Season.Winter => Season.Summer,
        _ => throw new ArgumentOutOfRangeException(nameof(season), season, "Unknown season")
    };
}
