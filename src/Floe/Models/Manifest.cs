namespace Floe.Models;

public record Manifest(
    string Digest,
    string MediaType,
    long Size,
    string Content,
    DateTimeOffset CreatedAt
);
