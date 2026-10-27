# MagicVilla API — Part 2

An educational ASP.NET Core Web API and MVC client for managing villas and villa numbers, extending the earlier learning repository with additional API and token-lifecycle work.

## Implementation highlights

- Versioned API controllers, DTO mapping, validation, filtering and data access through EF Core repositories.
- ASP.NET Core Identity and JWT-based account flows; public registration assigns only customer.
- An MVC client that calls the API through service abstractions.
- Refresh/revoke token endpoints and custom API error-handling components.

## Repository map

| Path | Purpose |
|---|---|
| [MagicVilla_VillaAPI](MagicVilla_VillaAPI) | API host, versioned controllers, EF context and repositories |
| [MagicVilla_Web](MagicVilla_Web) | MVC client and API service adapters |
| [MagicVilla_Utility](MagicVilla_Utility) | Shared constants |
| [tests/Authorization.Tests](tests/Authorization.Tests) | Customer-role and failed-assignment regressions |

## Requirements and configuration

Use an SDK capable of targeting .NET 8 and an isolated SQL Server development database. Package restore requires access to the configured NuGet feeds. These are existing .NET 8 examples; dependency and runtime modernization are separate work.

Read [SECURITY-SETUP.md](SECURITY-SETUP.md) before configuring credentials. Tracked secret values are intentionally empty. Use environment variables for your own development resources; do not restore old values from Git history.

The API reads `ConnectionStrings:DefaultSQLConnection` and `ApiSettings:Secret`: set `ConnectionStrings__DefaultSQLConnection` and `ApiSettings__Secret` locally. Review the MVC client's `ServiceUrls:VillaAPI` setting so it points to your local API address.

**API startup applies pending migrations:** inspect the ApplyMigration function in [Program.cs](MagicVilla_VillaAPI/Program.cs) before starting it. Only use an approved isolated development database.

## Build and run

```powershell
git clone https://github.com/s-hesam-s/MagicVilla_API-Part2.git
cd MagicVilla_API-Part2
```

```powershell
dotnet restore MagicVilla.sln
dotnet build MagicVilla.sln --no-restore
```

After configuration and database review, start the API:

```powershell
dotnet run --project MagicVilla_VillaAPI/MagicVilla_VillaAPI.csproj --launch-profile https
```

The checked-in HTTPS profile uses `https://localhost:7001`; inspect the versioned operations at `https://localhost:7001/swagger`.

In another terminal, from the same repository root:

```powershell
dotnet run --project MagicVilla_Web/MagicVilla_Web.csproj --launch-profile https
```

Open `https://localhost:7002`. Start the API before the MVC client and use a local development certificate. The two MagicVilla repositories use the same default ports, so run one version at a time or change its launch/client settings consistently.

## Verification

```powershell
pwsh -File scripts/Test-PublicConfiguration.ps1
dotnet test tests/Authorization.Tests/Authorization.Tests.csproj
```

The authorization tests use mocked dependencies, with in-memory HTTP checks where applicable. They do not start the application or apply database migrations. Build and test results do not establish that a full deployment or external integration is configured.

## Related learning repository

[MagicVilla API — earlier version](https://github.com/s-hesam-s/MagicVilla_API)

## Project scope and reuse

This is an educational repository. The implementation notes describe the code in this repository; they do not claim a deployed commercial service or measured production results. Third-party libraries and assets retain their own terms. No project-wide license is currently declared.

## Continuous integration

GitHub Actions restores and builds `MagicVilla.sln` in Release mode on Windows using the existing .NET 8 target. It also runs `scripts/Test-PublicConfiguration.ps1` without printing configured values. The existing authorization regressions run with mocked dependencies and in-memory HTTP hosts where applicable. The application hosts, database migrations/seeds, cloud integrations and payments are not started by CI.

The [CI workflow](.github/workflows/ci.yml) runs for pull requests and pushes to `master`, and can be started manually. It uses read-only repository permissions and does not deploy the project.
