namespace Floe.Models;

public record Blob(
    string Digest,
    long Size,
    string ContentType,
    DateTimeOffset CreatedAt
);
