# Final Report

## Files Changed

### API runtime and deployment behavior

- `NZWalks.API/Program.cs`
  - cleaned SQL Server setup for deployment
  - added single-database fallback for auth DB
  - added separate migration history tables
  - made Swagger configurable outside Development
  - disabled fragile file logging by config
  - cleaned duplicate image static-file mapping
  - added `/` and `/health`
  - added deployment initialization hook

- `NZWalks.API/Infrastructure/DeploymentInitializationExtensions.cs`
  - new deployment startup helper
  - optional startup migrations
  - optional bootstrap Writer user creation
  - role ensuring for `Reader` and `Writer`

- `NZWalks.API/Repositories/LocalImageRepository.cs`
  - switched image-folder lookup to configuration

### Configuration cleanup

- `NZWalks.API/appsettings.json`
  - changed to production-safe placeholders

- `NZWalks.API/appsettings.Development.json`
  - moved local development SQL/JWT settings here

- `NZWalks.API/appsettings.Production.json`
  - new production deployment config file

- `NZWalks.UI/appsettings.json`
  - replaced localhost default with placeholder base URL

- `NZWalks.UI/appsettings.Development.json`
  - moved localhost API URL to development config

- `NZWalks.UI/appsettings.Production.json`
  - added production placeholder for future UI deployment

### Repo hygiene

- `NZWalks.API/NZWalks.API.csproj`
  - removed invalid publish/build artifact includes from `bin\Debug`

- deleted `NZWalks.API/Controllers/StudentsController.cs`
  - sample/demo endpoint removed from production API surface

- deleted `NZWalks.API/Controllers/WeatherForecastController.cs`
  - template endpoint removed from production API surface

- deleted `NZWalks.API/WeatherForecast.cs`
  - no longer needed after removing template controller

### Deployment documentation

- `DEPLOY_SOMEE.md`
- `DEPLOYMENT_SUMMARY.md`
- `FINAL_REPORT.md`

## Why Each Change Was Necessary

- The API was locked to localhost and LocalDB assumptions, which would fail on Somee.
- The repository had bad API project-file hygiene that could create confusing publish behavior.
- The free-hosting path strongly favors deploying the API first, not both apps at once.
- The project needed a practical way to create one initial Writer user without manual DB hacking.
- Somee Free is much more realistic for this repository if both DbContexts share one SQL Server database.

## Manual Actions Still Required

- create a Somee free website
- create a Somee MSSQL database
- copy the real Somee SQL connection string into `appsettings.Production.json`
- set the real public API URL in `Jwt:Issuer` and `Jwt:Audience`
- temporarily enable startup migrations and bootstrap admin for the first deployment, or apply migrations manually
- publish locally and upload the publish output to Somee

## Exact Next Steps

1. Open `DEPLOY_SOMEE.md`.
2. Edit `NZWalks.API/appsettings.Production.json` with your real Somee values.
3. Decide whether you want:
   - startup migrations
   - or manual SQL script application
4. If using startup bootstrap, set:
   - `Deployment:ApplyMigrationsOnStartup = true`
   - `BootstrapAdmin:Enabled = true`
   - `BootstrapAdmin:Email` and `BootstrapAdmin:Password`
5. Publish the API:

```powershell
dotnet publish .\NZWalks.API\NZWalks.API.csproj -c Release -o .\publish\NZWalks.API
```

6. Edit the published `.\publish\NZWalks.API\appsettings.Production.json` if needed one last time.
7. Upload the **contents** of `.\publish\NZWalks.API` to Somee.
8. Browse to:

```text
https://your-api.somee.com/health
```

9. Open Swagger:

```text
https://your-api.somee.com/swagger
```

10. Log in with the bootstrap Writer user:
    - `POST /api/Auth/Login`

11. After the first successful deployment:
    - set `Deployment:ApplyMigrationsOnStartup = false`
    - set `BootstrapAdmin:Enabled = false`
    - upload the updated `appsettings.Production.json`

## Practical Deployment Decision

For this repository, the fastest realistic free public deployment is:

- `NZWalks.API` on Somee
- one Somee MSSQL database
- no UI deployment in phase 1

That is the least painful route that still produces a real online URL.
