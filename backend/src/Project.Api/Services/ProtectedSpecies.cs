namespace Project.Api.Services;

// Decides whether a species is protected (VT-020): GBIF gives it an IUCN Red List category of VU, EN or CR
public static class ProtectedSpecies
{
    private static readonly string[] ProtectedCategories = ["VU", "EN", "CR"];

    public static bool IsProtected(string? iucnCategory)
    {
        return iucnCategory is not null && ProtectedCategories.Contains(iucnCategory);
    }
}
