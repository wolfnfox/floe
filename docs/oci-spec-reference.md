# OCI Distribution Specification Reference

Quick reference for implementing OCI Distribution Spec v1.1.

## API Endpoints Summary

| Method | Path | Description | Category |
|--------|------|-------------|----------|
| GET | `/v2/` | API version check | Base |
| HEAD | `/v2/<name>/blobs/<digest>` | Check blob exists | Pull |
| GET | `/v2/<name>/blobs/<digest>` | Fetch blob | Pull |
| HEAD | `/v2/<name>/manifests/<reference>` | Check manifest exists | Pull |
| GET | `/v2/<name>/manifests/<reference>` | Fetch manifest | Pull |
| POST | `/v2/<name>/blobs/uploads/` | Initiate blob upload | Push |
| GET | `/v2/<name>/blobs/uploads/<uuid>` | Get upload status | Push |
| PATCH | `/v2/<name>/blobs/uploads/<uuid>` | Upload blob chunk | Push |
| PUT | `/v2/<name>/blobs/uploads/<uuid>` | Complete blob upload | Push |
| DELETE | `/v2/<name>/blobs/uploads/<uuid>` | Cancel blob upload | Push |
| PUT | `/v2/<name>/manifests/<reference>` | Push manifest | Push |
| GET | `/v2/_catalog` | List repositories | Discovery |
| GET | `/v2/<name>/tags/list` | List tags | Discovery |
| DELETE | `/v2/<name>/manifests/<reference>` | Delete manifest | Management |
| DELETE | `/v2/<name>/blobs/<digest>` | Delete blob | Management |

## Endpoint Details

### GET /v2/
Check API version support.

**Response:** `200 OK`
```
Docker-Distribution-API-Version: registry/2.0
{}
```

**On auth required:** `401 Unauthorized` with `WWW-Authenticate` header

---

### HEAD /v2/<name>/blobs/<digest>
Check if blob exists.

**Response Headers (200 OK):**
- `Content-Length: <size>`
- `Docker-Content-Digest: <digest>`
- `Content-Type: application/octet-stream`

**Errors:**
- `404 Not Found` → `BLOB_UNKNOWN`

---

### GET /v2/<name>/blobs/<digest>
Fetch blob content.

**Response:** `200 OK` with blob content

**Headers:**
- `Content-Length: <size>`
- `Docker-Content-Digest: <digest>`
- `Content-Type: application/octet-stream`

**Range Requests:** SHOULD support `Range` header, respond with `206 Partial Content`

---

### HEAD/GET /v2/<name>/manifests/<reference>
Fetch manifest. `<reference>` can be tag or digest.

**Request Headers:**
```
Accept: application/vnd.oci.image.manifest.v1+json,
        application/vnd.oci.image.index.v1+json,
        application/vnd.docker.distribution.manifest.v2+json
```

**Response Headers (200 OK):**
- `Content-Type: <manifest media type>`
- `Content-Length: <size>`
- `Docker-Content-Digest: <digest>`

**Errors:**
- `404 Not Found` → `MANIFEST_UNKNOWN`

---

### POST /v2/<name>/blobs/uploads/
Initiate blob upload.

**Query Parameters:**
- `mount=<digest>` - Mount blob from another repo
- `from=<other_name>` - Source repo for mount
- `digest=<digest>` - For single POST monolithic upload

**Responses:**

*Initiate chunked upload:* `202 Accepted`
```
Location: /v2/<name>/blobs/uploads/<uuid>
Range: 0-0
Docker-Upload-UUID: <uuid>
```

*Successful mount:* `201 Created`
```
Location: /v2/<name>/blobs/<digest>
Docker-Content-Digest: <digest>
```

*Mount failed (blob doesn't exist):* `202 Accepted` (fall back to upload)

*Monolithic POST (with digest param):* Can complete in one request

---

### PATCH /v2/<name>/blobs/uploads/<uuid>
Upload blob chunk.

**Request:**
```
Content-Type: application/octet-stream
Content-Length: <chunk size>
Content-Range: <start>-<end>
```

**Response:** `202 Accepted`
```
Location: /v2/<name>/blobs/uploads/<uuid>
Range: 0-<offset>
Docker-Upload-UUID: <uuid>
```

---

### PUT /v2/<name>/blobs/uploads/<uuid>?digest=<digest>
Complete blob upload.

**Request:** Final chunk (or empty if all chunks sent via PATCH)
```
Content-Type: application/octet-stream
Content-Length: <size>
```

**Response:** `201 Created`
```
Location: /v2/<name>/blobs/<digest>
Docker-Content-Digest: <digest>
```

**Errors:**
- `400 Bad Request` → `DIGEST_INVALID` (if computed digest doesn't match)

---

### GET /v2/<name>/blobs/uploads/<uuid>
Get upload progress.

**Response:** `204 No Content`
```
Location: /v2/<name>/blobs/uploads/<uuid>
Range: 0-<offset>
Docker-Upload-UUID: <uuid>
```

---

### DELETE /v2/<name>/blobs/uploads/<uuid>
Cancel upload.

**Response:** `204 No Content`

---

### PUT /v2/<name>/manifests/<reference>
Push manifest.

**Request:**
```
Content-Type: application/vnd.oci.image.manifest.v1+json
```
```json
{
  "schemaVersion": 2,
  "mediaType": "application/vnd.oci.image.manifest.v1+json",
  "config": { ... },
  "layers": [ ... ]
}
```

**Response:** `201 Created`
```
Location: /v2/<name>/manifests/<digest>
Docker-Content-Digest: <digest>
```

**Validation:**
- All referenced blobs MUST exist
- Return `MANIFEST_BLOB_UNKNOWN` if missing

**Errors:**
- `400 Bad Request` → `MANIFEST_INVALID`
- `400 Bad Request` → `MANIFEST_BLOB_UNKNOWN`
- `413 Payload Too Large` (recommend 4MB limit)

---

### GET /v2/_catalog
List repositories.

**Query Parameters:**
- `n=<integer>` - Maximum entries
- `last=<string>` - Cursor (last repo name from previous page)

**Response:** `200 OK`
```json
{
  "repositories": ["library/alpine", "library/nginx"]
}
```

**Pagination:** Include `Link` header for next page:
```
Link: </v2/_catalog?n=10&last=library/nginx>; rel="next"
```

---

### GET /v2/<name>/tags/list
List tags for repository.

**Query Parameters:**
- `n=<integer>` - Maximum entries
- `last=<string>` - Cursor (last tag from previous page)

**Response:** `200 OK`
```json
{
  "name": "library/alpine",
  "tags": ["3.18", "3.19", "latest"]
}
```

---

### DELETE /v2/<name>/manifests/<reference>
Delete manifest. `<reference>` MUST be a digest.

**Response:** `202 Accepted`

**Errors:**
- `404 Not Found` → `MANIFEST_UNKNOWN`
- `400 Bad Request` / `405 Method Not Allowed` if deletion disabled

---

### DELETE /v2/<name>/blobs/<digest>
Delete blob.

**Response:** `202 Accepted`

**Errors:**
- `404 Not Found` → `BLOB_UNKNOWN`
- `405 Method Not Allowed` if deletion disabled

---

## Error Codes

| Code | Description |
|------|-------------|
| `BLOB_UNKNOWN` | Blob unknown to registry |
| `BLOB_UPLOAD_INVALID` | Blob upload invalid |
| `BLOB_UPLOAD_UNKNOWN` | Blob upload unknown to registry |
| `DIGEST_INVALID` | Provided digest did not match uploaded content |
| `MANIFEST_BLOB_UNKNOWN` | Manifest references blob unknown to registry |
| `MANIFEST_INVALID` | Manifest invalid |
| `MANIFEST_UNKNOWN` | Manifest unknown to registry |
| `NAME_INVALID` | Invalid repository name |
| `NAME_UNKNOWN` | Repository name not known to registry |
| `SIZE_INVALID` | Provided length did not match content length |
| `UNAUTHORIZED` | Authentication required |
| `DENIED` | Access denied |
| `UNSUPPORTED` | Operation not supported |

**Error Response Format:**
```json
{
  "errors": [
    {
      "code": "MANIFEST_UNKNOWN",
      "message": "manifest unknown",
      "detail": { "tag": "latest" }
    }
  ]
}
```

---

## Media Types

### Manifests
- `application/vnd.oci.image.manifest.v1+json` - OCI Image Manifest
- `application/vnd.oci.image.index.v1+json` - OCI Image Index
- `application/vnd.docker.distribution.manifest.v2+json` - Docker v2 (optional)
- `application/vnd.docker.distribution.manifest.list.v2+json` - Docker list (optional)

### Blobs
- `application/octet-stream` - Default for uploads
- `application/vnd.oci.image.config.v1+json` - Config blob
- `application/vnd.oci.image.layer.v1.tar+gzip` - Compressed layer
- `application/vnd.oci.image.layer.v1.tar` - Uncompressed layer

---

## OCI Image Manifest Structure

```json
{
  "schemaVersion": 2,
  "mediaType": "application/vnd.oci.image.manifest.v1+json",
  "config": {
    "mediaType": "application/vnd.oci.image.config.v1+json",
    "digest": "sha256:...",
    "size": 7023
  },
  "layers": [
    {
      "mediaType": "application/vnd.oci.image.layer.v1.tar+gzip",
      "digest": "sha256:...",
      "size": 32654
    }
  ],
  "annotations": {
    "org.opencontainers.image.created": "2024-01-15T10:00:00Z"
  }
}
```

---

## OCI Image Index Structure

```json
{
  "schemaVersion": 2,
  "mediaType": "application/vnd.oci.image.index.v1+json",
  "manifests": [
    {
      "mediaType": "application/vnd.oci.image.manifest.v1+json",
      "digest": "sha256:...",
      "size": 7143,
      "platform": {
        "architecture": "amd64",
        "os": "linux"
      }
    },
    {
      "mediaType": "application/vnd.oci.image.manifest.v1+json",
      "digest": "sha256:...",
      "size": 7682,
      "platform": {
        "architecture": "arm64",
        "os": "linux"
      }
    }
  ]
}
```

---

## Digest Format

```
<algorithm>:<encoded>
sha256:6c3c624b58dbbcd3c0dd82b4c53f04194d1247c6eebdaab7c610cf7d66709b3b
```

- Algorithm: `sha256` (required), `sha512` (optional)
- Encoded: lowercase hex
- Full regex: `[a-z0-9]+:[a-f0-9]+`

---

## Repository Name Rules

- Must match: `[a-z0-9]+([._-][a-z0-9]+)*(/[a-z0-9]+([._-][a-z0-9]+)*)*`
- Max 256 characters
- Cannot start with `.` or `_`
- Examples: `library/nginx`, `myorg/myrepo`, `single-name`
