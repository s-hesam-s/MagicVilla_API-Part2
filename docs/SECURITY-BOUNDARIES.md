# Token, mutation and browser security boundaries

This repository is an educational API/MVC sample. Source controls and isolated tests are distinguished from deployed integration evidence.

## Token lifecycle

- Public registration continues to assign only customer. Administrative mutations require the admin role.
- Refresh/revoke verify the access token signature, HS256 algorithm, issuer and audience, then match subject and JTI to the persisted refresh record. Malformed, unsigned, wrong-key and mismatched pairs are rejected.
- An expired access token can be presented for refresh; the refresh token itself must still be valid and unexpired. This is intentional and does not make the expired access token valid for protected API calls.
- An invalid access-token pair does not invalidate someone else's refresh record. A signed matching replay invalidates the chain.
- Refresh/revoke use a Serializable SQL Server transaction and parameterized UPDLOCK/HOLDLOCK lookup. Rotation consumes the old record conditionally before inserting a replacement in the same transaction; failures roll back the transaction. These paths require the configured SQL provider.
- Cryptographic pair validation is exercised with locally generated test keys. SQL-backed rotation, replay races, transaction rollback and the live login/refresh/revoke flow were not executed against a database. Revoking refresh tokens does not immediately revoke a previously issued access JWT.

## Villa mutations and images

- V1/V2 PATCH accepts at most 32 replace operations on business fields only. Identity, local image paths, image URLs and upload objects cannot be patched. The resulting model is validated before save.
- PUT preserves stored image paths instead of trusting hidden storage fields. V2 uploads use a new generated filename, extension/signature checks for PNG/JPEG/WebP, a 2 MiB file limit and a 3 MiB multipart limit. The image URL retains the API host so the separate MVC host can display it.
- Deletion is restricted to the managed ProductImage folder; previous images are cleaned up after a successful save. Failed cleanup may leave orphan files. Signature checks are not full image decoding or malware scanning. Static responses include nosniff.
- CRUD failure responses do not return exception dumps to the caller.

## MVC boundary

- MVC registers automatic antiforgery validation; Logout is POST with a form token.
- Access/refresh cookies are HttpOnly, Secure, SameSite=Lax and scoped to `/`. They expire after 30 days, matching the refresh lifetime. An expired access token is retained for the refresh-pair protocol; protected API calls still validate its lifetime.
- Missing token pairs on an unauthorized API response lead to the login flow, rather than a null dereference. Malformed/empty API responses cannot report success and do not expose exception details.
- The frontend trusts token responses from its configured API. HTTPS, host configuration and real browser cookie behavior still need deployment verification.

## Verification limits

Tests use strict repository mocks, real MVC binding/validation and authorization policies, in-process HTTP antiforgery checks with ephemeral keys, and signed/malformed JWT fixtures. They do not start either original host, migrate SQL, contact a live API or certify deployed security. The completed source migration and package changes are documented in [RUNTIME.md](RUNTIME.md); installed and deployed runtime patches must still be checked independently.

Use HTTPS for local MVC sign-in: Secure token cookies are intentionally not sent over plain HTTP.
