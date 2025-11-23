using System.Text.Json.Serialization;

namespace Floe.Models.Oci;

public record OciImageIndex(
    [property: JsonPropertyName("schemaVersion")] int SchemaVersion,
    [property: JsonPropertyName("mediaType")] string? MediaType,
    [property: JsonPropertyName("manifests")] OciDescriptor[] Manifests,
    [property: JsonPropertyName("annotations")] Dictionary<string, string>? Annotations = null,
    [property: JsonPropertyName("subject")] OciDescriptor? Subject = null
);
