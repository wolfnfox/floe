namespace Floe.Configuration;

public class FloeOptions
{
    public const string SectionName = "Floe";

    public string DataPath { get; set; } = "/data";
    public int MaxManifestSize { get; set; } = 4 * 1024 * 1024; // 4MB
    public int DefaultPageSize { get; set; } = 100;
    public int MaxPageSize { get; set; } = 1000;
}
