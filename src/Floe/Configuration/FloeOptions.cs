namespace Floe.Configuration;

public class FloeOptions
{
    public const string SectionName = "Floe";

    public string DataPath { get; init; } = "/data";
    public int MaxManifestSize { get; init; } = 4 * 1024 * 1024; // 4MB
    public int DefaultPageSize { get; init; } = 100;
    public int MaxPageSize { get; init; } = 1000;
}
