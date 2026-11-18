namespace Project.Api.Services;

public static class SpeciesGroupName
{
    private static readonly Dictionary<string, string> Names = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Aves"] = "Paukščiai",
        ["Mammalia"] = "Žinduoliai",
        ["Insecta"] = "Vabzdžiai",
        ["Reptilia"] = "Ropliai",
        ["Amphibia"] = "Varliagyviai",
        ["Arachnida"] = "Voragyviai",
        ["Actinopterygii"] = "Žuvys",
        ["Magnoliopsida"] = "Žydintys augalai",
        ["Liliopsida"] = "Žydintys augalai"
    };

    // The Lithuanian name of a GBIF class; an unknown class keeps its GBIF name
    public static string For(string gbifClass) => Names.TryGetValue(gbifClass, out var name) ? name : gbifClass;
}
