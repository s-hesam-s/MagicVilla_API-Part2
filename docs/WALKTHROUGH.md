# Technical walkthrough: read a versioned villa API

[Architecture](ARCHITECTURE.md) · [Read-only HTTP examples](examples/read-only.http) · [Setup](../README.md#requirements-and-configuration)

This guide separates isolated test evidence from optional requests against your own development host. Response fragments below are source-derived examples, not captured live output.

## 1. Run the existing isolated evidence

From the repository root:

```powershell
dotnet restore tests/Authorization.Tests/Authorization.Tests.csproj
dotnet test tests/Authorization.Tests/Authorization.Tests.csproj --configuration Release --no-restore --filter "FullyQualifiedName~RegistrationSecurityTests"
```

[RegistrationSecurityTests](../tests/Authorization.Tests/RegistrationSecurityTests.cs) replace Identity dependencies with mocks. They verify that requested elevated roles do not change the assigned `customer` role and that assignment/role-creation failures are not reported as successful registration. They do not start the API, SQL Server or its migration path. They do not test the complete token lifecycle.

## 2. Prepare a development host

Follow [Build and run](../README.md#build-and-run) only after reviewing the isolated development database and the startup ApplyMigration function in [Program](../MagicVilla_VillaAPI/Program.cs). No host, migration or account creation is executed by this documentation change.

API: `https://localhost:7001`; Swagger: `https://localhost:7001/swagger`; MVC: `https://localhost:7002`. Configure the API database/signing key using [SECURITY-SETUP.md](../SECURITY-SETUP.md). The MVC `ServiceUrls:VillaAPI` must match the API. Use a trusted development certificate. Mango and the other MagicVilla example reuse some default ports: run one environment at a time or change all relevant addresses consistently.

The model seed in [ApplicationDbContext](../MagicVilla_VillaAPI/Data/ApplicationDbContext.cs) includes villa ID 1, Royal Villa, rate 200, occupancy 4 and 550 sqft. These values only apply when the database still matches that seed. No new seed script or dataset is applied here.

## 3. Read an individual villa

```http
GET https://localhost:7001/api/v1/VillaAPI/1
Accept: application/json
```

The current V1 read action has no Authorize attribute. Expected success: HTTP 200 with an APIResponse envelope. A source-derived **response fragment** for unchanged seed data is:

```json
{
  "isSuccess": true,
  "result": {
    "id": 1,
    "name": "Royal Villa",
    "rate": 200,
    "occupancy": 4,
    "sqft": 550
  }
}
```

The complete response also has envelope and DTO fields omitted above. An ID of 0 produces HTTP 400; a positive ID that actually does not exist produces 404. Do not assume a chosen large ID is absent from a previously populated database.

## 4. Inspect version routing and pagination

```http
GET https://localhost:7001/api/v1/VillaAPI?filterOccupancy=4&pageSize=2&pageNumber=1
Accept: application/json
```

Expected: HTTP 200, `result` is an array and the response includes an `X-Pagination` header representing PageNumber 1 and PageSize 2. There are at most two results from the repository for that request; the query does not promise stable ordering.

Compare the equivalent `/api/v2/VillaAPI` URL in [read-only.http](examples/read-only.http). Both collection actions currently accept the same query names. To explore text search, use lowercase `search=pool`; search runs after repository pagination, so it is not a global count/search-before-page contract. Use pageSize 0 to disable repository pagination when examining all matching data.

These checks exercise API reads only and do not require a customer/admin token in the read actions shown.

## 5. Optional token-lifecycle review

This section describes operations that **write account/refresh-token state**. It is outside the read-only HTTP file and was not executed here. Use only your own approved development account/environment.

| Request | Body / expected outcome |
|---|---|
| `POST /api/v1/UsersAuth/register` | `{"userName":"demo@example.invalid","name":"Demo Customer","password":"<local-password>","role":"customer"}`. A unique valid account returns HTTP 200 / isSuccess true; it does not return a login token. The repository always assigns customer. Duplicate usernames return 400. |
| `POST /api/v1/UsersAuth/login` | `{"userName":"<local-user>","password":"<local-password>"}`. Success returns `result.accessToken` and `result.refreshToken`; invalid credentials return 400. |
| `POST /api/v1/UsersAuth/refresh` | `{"accessToken":"<current-local-access-token>","refreshToken":"<current-local-refresh-token>"}`. A valid stored token pair returns a replacement pair; use the replacement for subsequent requests. |
| `POST /api/v1/UsersAuth/revoke` | The same TokenDTO shape, with your current pair. A valid model follows the repository revocation path and returns a success envelope; this alone does not prove all previously issued access tokens are immediately invalidated. |

Send JSON bodies with Content-Type application/json. All credential/token strings above are placeholders. Keep real values out of committed files, screenshots and shared logs. Swagger is the appropriate UI for reviewing versioned operation schemas.

## Evidence boundaries

[CI](../.github/workflows/ci.yml) restores/builds the solution and runs registration regressions and the configuration check. No full SQL-backed API/MVC or token refresh/revoke scenario was executed for this guide. Architecture notes describe the implementation and its tradeoffs, not performance measurements or a production security certification.
