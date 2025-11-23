# CLAUDE.md - Floe Container Registry

## Project Overview

Floe is a distributed, self-clustering container image registry designed for air-gapped Kubernetes environments. It provides high availability without requiring external databases or shared storage.

**Current Phase:** Phase 1 - Standalone OCI-Compliant Registry

## Tech Stack

- **.NET 10** with Minimal APIs
- **SQLite** via Microsoft.Data.Sqlite for metadata storage
- **File system** for blob/manifest content storage
- **Apache 2.0** license

## Project Structure

```
src/
└── Floe/
    ├── Program.cs                 # Entry point, minimal API setup
    ├── Floe.csproj
    ├── Endpoints/                 # API endpoint handlers
    │   ├── BaseEndpoints.cs       # GET /v2/ 
    │   ├── BlobEndpoints.cs       # Blob operations
    │   ├── ManifestEndpoints.cs   # Manifest operations
    │   └── CatalogEndpoints.cs    # Catalog & tag listing
    ├── Services/                  # Business logic
    │   ├── IBlobService.cs
    │   ├── BlobService.cs
    │   ├── IManifestService.cs
    │   ├── ManifestService.cs
    │   └── DigestService.cs       # SHA256 computation
    ├── Storage/                   # Storage implementations
    │   ├── IBlobStore.cs
    │   ├── IManifestStore.cs
    │   ├── FileSystemBlobStore.cs
    │   └── FileSystemManifestStore.cs
    ├── Data/                      # Database
    │   ├── FloeDbContext.cs       # EF Core not used; raw ADO.NET
    │   ├── SqliteMetadataStore.cs
    │   └── Migrations/            # Schema setup scripts
    ├── Models/                    # Domain models
    │   ├── Blob.cs
    │   ├── Manifest.cs
    │   ├── Repository.cs
    │   ├── Tag.cs
    │   ├── BlobUpload.cs          # In-progress uploads
    │   └── Oci/                   # OCI spec types
    │       ├── OciDescriptor.cs
    │       ├── OciManifest.cs
    │       └── OciImageIndex.cs
    ├── Middleware/
    │   └── OciErrorMiddleware.cs  # Standard OCI error responses
    └── Configuration/
        └── FloeOptions.cs
tests/
└── Floe.Tests/
    ├── Endpoints/
    ├── Services/
    └── Integration/
docs/
    └── oci-spec-reference.md
```

## Build & Run Commands

```bash
# Build
dotnet build

# Run (development)
dotnet run --project src/Floe

# Run tests
dotnet test

# Publish (linux-x64)
dotnet publish src/Floe -c Release -r linux-x64 --self-contained

# Docker build
docker build -t floe:dev .
```

## Coding Conventions

### C# Style
- Use file-scoped namespaces
- Use primary constructors where appropriate
- Prefer `record` for immutable data types
- Use nullable reference types (`#nullable enable`)
- Async methods should be named with `Async` suffix
- Use `CancellationToken` on all async methods

### API Design
- Follow OCI Distribution Spec exactly for paths and responses
- Return proper OCI error format for all errors
- Use kebab-case for JSON properties (match OCI spec)
- Include `Docker-Distribution-API-Version: registry/2.0` header on all responses
- Include `Docker-Content-Digest` header where required by spec

### Storage Paths
```
/data
├── blobs/sha256/{first2chars}/{digest}/data
├── manifests/sha256/{first2chars}/{digest}/data
└── floe.db
```

### Error Response Format (OCI Standard)
```json
{
  "errors": [
    {
      "code": "BLOB_UNKNOWN",
      "message": "blob unknown to registry",
      "detail": { "digest": "sha256:..." }
    }
  ]
}
```

## Key Dependencies

```xml
<PackageReference Include="Microsoft.Data.Sqlite" Version="10.*" />
<PackageReference Include="System.IO.Pipelines" Version="10.*" />
```

## Testing Strategy

- Unit tests for services and storage
- Integration tests using `WebApplicationFactory`
- Verify with real tools: `docker pull/push`, `crane`, `skopeo`
- OCI conformance tests (future)

## Phase 1 Scope

### In Scope
- OCI Distribution Spec v1.1 compliance (Pull, Push, Content Discovery, Content Management)
- OCI Image Manifest and OCI Image Index support
- Monolithic and chunked blob uploads
- Cross-repository blob mounting
- Cursor-based pagination for catalog and tags
- Manifest deletion (blob GC deferred)
- SQLite for metadata, filesystem for content

### Out of Scope (Future Phases)
- Authentication/authorization
- Pull-through proxy cache
- Clustering/replication
- Garbage collection
- Referrers API (optional in spec)

## Important OCI Spec Details

See `docs/oci-spec-reference.md` for full endpoint reference.

### Critical Behaviors
1. Manifest push MUST validate all referenced blobs exist
2. Blob uploads return `Location` header with upload URL
3. Digest format: `sha256:hex` (lowercase)
4. Content-Type for blobs: `application/octet-stream`
5. Accept header negotiation for manifest media types
6. Tags are mutable pointers to manifest digests
