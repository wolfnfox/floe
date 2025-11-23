using System.Text.Json.Serialization;

namespace Floe.Models.Oci;

public record OciManifest(
    [property: JsonPropertyName("schemaVersion")] int SchemaVersion,
    [property: JsonPropertyName("mediaType")] string? MediaType,
    [property: JsonPropertyName("config")] OciDescriptor Config,
    [property: JsonPropertyName("layers")] OciDescriptor[] Layers,
    [property: JsonPropertyName("annotations")] Dictionary<string, string>? Annotations = null,
    [property: JsonPropertyName("subject")] OciDescriptor? Subject = null
);
