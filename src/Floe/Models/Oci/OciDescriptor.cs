using System.Text.Json.Serialization;

namespace Floe.Models.Oci;

public record OciDescriptor(
    [property: JsonPropertyName("mediaType")] string MediaType,
    [property: JsonPropertyName("digest")] string Digest,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("annotations")] Dictionary<string, string>? Annotations = null,
    [property: JsonPropertyName("platform")] OciPlatform? Platform = null
);

public record OciPlatform(
    [property: JsonPropertyName("architecture")] string Architecture,
    [property: JsonPropertyName("os")] string Os,
    [property: JsonPropertyName("os.version")] string? OsVersion = null,
    [property: JsonPropertyName("os.features")] string[]? OsFeatures = null,
    [property: JsonPropertyName("variant")] string? Variant = null
);
