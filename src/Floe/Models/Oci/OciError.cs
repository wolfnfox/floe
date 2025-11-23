using System.Text.Json.Serialization;

namespace Floe.Models.Oci;

public record OciError(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("detail")] object? Detail = null
);

public record OciErrorResponse(
    [property: JsonPropertyName("errors")] OciError[] Errors
);
