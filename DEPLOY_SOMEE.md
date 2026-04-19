# Deploy NZWalks.API To Somee Free

## What Changed And Why

This repository was prepared for the lowest-risk free deployment path for this codebase:

- `NZWalks.API` is the **phase 1 deployment target**.
- `NZWalks.UI` is **not** the primary deployment target for phase 1 because:
  - Somee Free is a poor fit for two separate public web apps.
  - the UI currently calls the API over HTTP but does not send JWTs for protected write operations.
  - the current UI only covers Region screens and is incomplete compared to the API.

Deployment-focused changes made in this repository:

- moved production-sensitive values out of localhost/localdb defaults
- added `appsettings.Production.json` placeholders
- made the API able to reuse **one SQL Server database** for both DbContexts
- separated EF migration history tables so both DbContexts can safely share one database
- added optional startup migrations for first deployment
- added optional bootstrap creation of one initial Writer user
- reduced local file logging risk for shared hosting
- removed sample/demo-only API controllers from production surface
- cleaned `NZWalks.API.csproj` so publish output is no longer polluted by `bin\Debug` includes
- added root `/` and `/health` endpoints for faster post-deploy verification

## Phase 1 Scope

Deploy this project first:

- `NZWalks.API`

Do **not** prioritize `NZWalks.UI` on Somee Free for the first public deployment.

## Exact Project To Publish

Publish this project:

```powershell
dotnet publish .\NZWalks.API\NZWalks.API.csproj -c Release -o .\publish\NZWalks.API
```

Expected publish output folder:

```text
.\publish\NZWalks.API
```

The publish output should include a generated `web.config`.

## Optional Fallback Publish Command

Use this only if Somee runtime support changes unexpectedly and the framework-dependent build does not start:

```powershell
dotnet publish .\NZWalks.API\NZWalks.API.csproj -c Release -r win-x64 --self-contained true -o .\publish\NZWalks.API-selfcontained
```

Do not use the self-contained publish first unless you need it. It is larger and less friendly to a free disk quota.

## Production Config You Must Set Before Upload

Edit this file before uploading:

```text
.\publish\NZWalks.API\appsettings.Production.json
```

Required keys:

```json
{
  "ConnectionStrings": {
    "NZWalksConnectionString": "REPLACE-WITH-YOUR-SOMEE-SQL-CONNECTION-STRING",
    "NZWalksAuthConnectionString": ""
  },
  "Jwt": {
    "Key": "REPLACE-WITH-A-LONG-RANDOM-JWT-KEY",
    "Issuer": "https://your-api.somee.com",
    "Audience": "https://your-api.somee.com"
  },
  "Swagger": {
    "Enabled": true
  },
  "LoggingTargets": {
    "EnableFile": false
  },
  "Storage": {
    "ImagesFolder": "Images"
  },
  "Deployment": {
    "ApplyMigrationsOnStartup": false
  },
  "BootstrapAdmin": {
    "Enabled": false,
    "Email": "writer@your-domain.example",
    "Password": "CHANGE-ME-WRITER-PASSWORD"
  }
}
```

### Important Notes

- `NZWalksAuthConnectionString` may stay blank.
  - If blank, the API will reuse `NZWalksConnectionString`.
  - This is the recommended Somee Free path because the free plan is best treated as a **single-database deployment**.
- `Jwt:Issuer` and `Jwt:Audience` must match the **real public API URL**.
- Keep `LoggingTargets:EnableFile` as `false` on Somee unless you have a strong reason to turn it on.

## Somee Database Steps

### Recommended database layout

Use **one Somee MSSQL database** for both:

- `NZWalksDbContext`
- `NZWalksAuthDbContext`

This repository now supports that layout safely by:

- reusing the main connection string when the auth connection string is blank
- using separate EF migration history tables for each DbContext

### Create the database

1. Create your Somee free website.
2. Create your Somee free MSSQL database from the Somee control panel.
3. Copy the SQL Server connection string from Somee.
4. Put that value into:

```json
"ConnectionStrings": {
  "NZWalksConnectionString": "YOUR-SOMEE-CONNECTION-STRING",
  "NZWalksAuthConnectionString": ""
}
```

## How To Apply Migrations

There are two valid paths. Use **Path A** for the lowest-friction first deployment.

### Path A: Apply migrations automatically on first startup

Before upload, temporarily set:

```json
"Deployment": {
  "ApplyMigrationsOnStartup": true
}
```

This tells the deployed API to apply pending migrations on startup.

This is the fastest path if:

- the Somee database already exists
- the connection string is correct
- the SQL user has rights to create tables and indexes

After the first successful startup, set it back to `false` and re-upload `appsettings.Production.json`.

### Path B: Generate SQL scripts locally and apply them manually

Generate idempotent SQL scripts locally:

```powershell
dotnet ef migrations script --idempotent --project .\NZWalks.API\NZWalks.API.csproj --startup-project .\NZWalks.API\NZWalks.API.csproj --context NZWalksDbContext -o .\publish\db\NZWalksDb.sql
dotnet ef migrations script --idempotent --project .\NZWalks.API\NZWalks.API.csproj --startup-project .\NZWalks.API\NZWalks.API.csproj --context NZWalksAuthDbContext -o .\publish\db\NZWalksAuthDb.sql
```

Then run both SQL scripts against the same Somee MSSQL database using your preferred SQL client.

Run order:

1. `NZWalksDb.sql`
2. `NZWalksAuthDb.sql`

Use Path B if:

- automatic startup migration fails
- you prefer explicit SQL control
- you want to verify schema changes before the app boots

## How To Create The Initial Writer User

The repository now supports a config-controlled bootstrap Writer user.

For the first deployment, temporarily set:

```json
"BootstrapAdmin": {
  "Enabled": true,
  "Email": "your-writer-login@example.com",
  "Password": "YourStrongPassword123!"
}
```

Recommended first-deploy combination:

```json
"Deployment": {
  "ApplyMigrationsOnStartup": true
},
"BootstrapAdmin": {
  "Enabled": true,
  "Email": "your-writer-login@example.com",
  "Password": "YourStrongPassword123!"
}
```

What happens on startup:

- roles `Reader` and `Writer` are ensured
- the configured user is created if missing
- that user is added to both `Reader` and `Writer`

After the user has been created successfully:

1. set `BootstrapAdmin:Enabled` back to `false`
2. re-upload `appsettings.Production.json`

## How To Upload To Somee

1. Publish locally:

```powershell
dotnet publish .\NZWalks.API\NZWalks.API.csproj -c Release -o .\publish\NZWalks.API
```

2. Edit `.\publish\NZWalks.API\appsettings.Production.json`.
3. Upload the **contents** of `.\publish\NZWalks.API` to your Somee site root using FTP or Somee File Manager.
4. Make sure `web.config` is uploaded with the rest of the publish output.
5. Browse to:

```text
https://your-api.somee.com/health
```

or:

```text
https://your-api.somee.com/swagger
```

If startup migrations are enabled, the first request will trigger app startup and schema initialization.

## How To Verify The Deployment

### Basic checks

1. Root endpoint:

```text
GET https://your-api.somee.com/
```

Expected result: JSON showing service status and links.

2. Health endpoint:

```text
GET https://your-api.somee.com/health
```

Expected result: JSON with `Healthy`.

3. Swagger:

```text
https://your-api.somee.com/swagger
```

### Verify database-backed API

Use Swagger or any REST client:

1. `POST /api/Auth/Login`
   - use the bootstrap Writer credentials
2. Copy the JWT token
3. Authorize in Swagger with:

```text
Bearer YOUR_TOKEN_HERE
```

4. Test:
   - `GET /api/Regions`
   - `POST /api/Regions`
   - `GET /api/Walks`

### Verify the Writer bootstrap

If login succeeds and a protected `POST /api/Regions` call works, the Writer bootstrap is functioning.

## Common Failure Points On Somee

### 1. `500 Internal Server Error` immediately after upload

Most likely causes:

- bad connection string
- placeholder JWT values still left in `appsettings.Production.json`
- startup migrations enabled but database access is invalid
- bootstrap admin enabled with placeholder password

What to check:

- verify `appsettings.Production.json` in the uploaded site
- verify the Somee SQL connection string
- temporarily set:
  - `Deployment:ApplyMigrationsOnStartup` to `false`
  - `BootstrapAdmin:Enabled` to `false`
  - then test `/health` again

### 2. `Login works but POST/PUT/DELETE fails with 401/403`

Causes:

- missing bearer token
- bootstrap user was not assigned `Writer`
- JWT issuer/audience do not match the real public URL

Fix:

- confirm `Jwt:Issuer` and `Jwt:Audience` use the real public Somee URL
- confirm the bootstrap section was enabled once and used valid credentials

### 3. Schema/tables do not exist

Causes:

- migrations were not run
- `ApplyMigrationsOnStartup` was left `false`

Fix:

- use Path A or Path B from the migration section above

### 4. Image upload works once, then files disappear later

Cause:

- this phase still uses local disk storage

Reality:

- local image storage is acceptable for a small demo, but it is not durable cloud storage
- redeployments or host-side cleanup may affect stored files

### 5. Swagger does not open

Cause:

- `Swagger:Enabled` is `false`

Fix:

- set it to `true` in `appsettings.Production.json` and re-upload that file

## Local Commands Summary

Publish:

```powershell
dotnet publish .\NZWalks.API\NZWalks.API.csproj -c Release -o .\publish\NZWalks.API
```

Build:

```powershell
dotnet build NZWalks.sln
```

Optional migration script generation:

```powershell
dotnet ef migrations script --idempotent --project .\NZWalks.API\NZWalks.API.csproj --startup-project .\NZWalks.API\NZWalks.API.csproj --context NZWalksDbContext -o .\publish\db\NZWalksDb.sql
dotnet ef migrations script --idempotent --project .\NZWalks.API\NZWalks.API.csproj --startup-project .\NZWalks.API\NZWalks.API.csproj --context NZWalksAuthDbContext -o .\publish\db\NZWalksAuthDb.sql
```
