# Deployment Summary

## Chosen Strategy

The deployment strategy chosen for this repository is:

- **Somee Free**
- **API-first**
- **minimum-change**
- **single SQL Server database**
- **UI deferred to phase 2**

## Why Somee Was Chosen

Somee is the most realistic free target for this repository because the current solution already uses:

- ASP.NET Core
- SQL Server
- IIS-style hosting assumptions

This makes Somee a much better minimum-change fit than Render, Koyeb, or Railway.

## Why Somee Was Chosen Over Render / Koyeb / Railway

### Somee

Best match for the current code because:

- no forced database engine migration
- no forced Docker/containerization
- Windows + IIS hosting fits the current ASP.NET Core + SQL Server solution
- lowest-risk route to get a real public URL online quickly

### Render

Not chosen for phase 1 because:

- current project is SQL Server based, not PostgreSQL based
- free hosting is more comfortable with one service than two
- local file storage is a bad fit there
- would require more refactor than Somee

### Koyeb

Not chosen for phase 1 because:

- free tier is still a poor fit for this two-app architecture
- SQL Server is not the natural path there
- would need more deployment packaging and likely a DB migration

### Railway

Not chosen for phase 1 because:

- it is not the cleanest long-term free path
- it still pushes this repository toward a cloud/container/Postgres style evolution
- that is not the lowest-change route for this codebase

## What Was Improved For Somee

- production config is now placeholder-driven instead of localhost-driven
- the API can now use a single SQL Server database for both DbContexts
- migrations can be auto-applied on first startup if desired
- a bootstrap Writer user can be created from configuration
- file logging is disabled by default for safer shared-host deployment
- sample controllers were removed from the production API surface
- API publish hygiene was fixed
- a root endpoint and health endpoint were added for fast post-deploy checks

## What Still Remains Imperfect

- `NZWalks.UI` is still not a good phase 1 deployment target
- image upload still uses local disk storage
- AutoMapper still has a package vulnerability warning in the current version
- nullable-reference warnings still exist across the codebase
- the app still depends on manual production config editing before upload

## Recommended Phase 1 Deployment Shape

- Deploy `NZWalks.API` only
- Keep `Swagger:Enabled = true`
- Use one Somee MSSQL database
- Bootstrap one Writer account from configuration
- Verify with `/health`, `/swagger`, and `POST /api/Auth/Login`

## Phase 2 Recommendation

If you later want a stronger cloud-ready deployment:

1. decide whether the UI should stay separate or be merged into one app
2. fix the UI auth flow so it can send JWT tokens properly
3. replace local image storage with object storage
4. upgrade vulnerable/outdated packages
5. add CI/CD and deployment automation
6. only then consider multi-service hosting or more cloud-native platforms

For this repository, phase 2 should focus on:

- **finishing the API/UI integration**
- **making file storage durable**
- **deciding whether the free-hosting architecture should stay split or be consolidated**
