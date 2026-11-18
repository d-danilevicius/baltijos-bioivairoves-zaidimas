namespace Project.Api.Services;

public static class GeoDistance
{
    private const double EarthRadiusKm = 6371.0088;

    // Haversine distance between two points given in degrees
    public static double InKilometres(double latitudeA, double longitudeA, double latitudeB, double longitudeB)
    {
        var deltaLatitude = ToRadians(latitudeB - latitudeA);
        var deltaLongitude = ToRadians(longitudeB - longitudeA);
        var a = Math.Pow(Math.Sin(deltaLatitude / 2), 2)
            + Math.Cos(ToRadians(latitudeA)) * Math.Cos(ToRadians(latitudeB)) * Math.Pow(Math.Sin(deltaLongitude / 2), 2);

        return 2 * EarthRadiusKm * Math.Asin(Math.Sqrt(a));
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
