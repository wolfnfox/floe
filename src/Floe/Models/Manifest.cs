namespace Floe.Models;

public record Manifest(
    string Digest,
    string MediaType,
    long Size,
    byte[] Content,
    DateTimeOffset CreatedAt
);
