namespace Floe.Models;

public record Tag(
    string RepositoryName,
    string Name,
    string ManifestDigest,
    DateTimeOffset UpdatedAt
);
