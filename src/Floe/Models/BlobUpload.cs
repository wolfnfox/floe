namespace Floe.Models;

public record BlobUpload(
    string Uuid,
    string RepositoryName,
    long BytesReceived,
    DateTimeOffset StartedAt,
    DateTimeOffset LastUpdated
);
