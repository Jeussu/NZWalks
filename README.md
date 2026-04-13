# NZWalks

NZWalks is an ASP.NET Core 8 solution with:

- `NZWalks.API`: a Web API for regions, walks, authentication, and image upload
- `NZWalks.UI`: a server-rendered MVC UI that currently manages regions through the API

This repository is currently a small CRUD-oriented .NET solution. It is not a full trail platform yet.

## Current Implemented Scope

Confirmed features in the repository:

- Region CRUD API
- Walk CRUD API with basic filtering, sorting, and paging
- JWT-based register/login endpoints
- Local image upload API
- MVC UI for listing, adding, editing, and deleting regions
- Swagger UI in Development for the API

## Current Limitations

This repository does **not** currently implement the broader product claims that previously appeared in the README, including:

- user reviews
- favorites
- trip planning
- interactive maps
- trail condition management
- a walk-management UI
- a login/register UI
- automated tests
- CI/CD workflows

Some sample/template endpoints still exist in the API project and are not part of the core NZWalks feature set.

## Technology Stack

- .NET 8
- ASP.NET Core Web API
- ASP.NET Core MVC / Razor Views
- Entity Framework Core with SQL Server
- ASP.NET Core Identity
- JWT bearer authentication
- AutoMapper
- Serilog
- Swagger / Swashbuckle
- Bootstrap and jQuery static assets in the UI project

## Prerequisites

- .NET 8 SDK
- SQL Server LocalDB, or another SQL Server instance if you update the connection strings

Default API connection strings are stored in `NZWalks.API/appsettings.json` and currently target:

- `NZWalksDB`
- `NZWalksAuthDB`

If your local SQL Server setup is different, update the connection strings before running the app.

## Getting Started

1. Restore dependencies:

```bash
dotnet restore NZWalks.sln
```

2. Build the solution:

```bash
dotnet build NZWalks.sln
```

3. Apply database migrations for both DbContexts.

Example commands:

```bash
dotnet ef database update --project NZWalks.API --startup-project NZWalks.API --context NZWalksDbContext
dotnet ef database update --project NZWalks.API --startup-project NZWalks.API --context NZWalksAuthDbContext
```

If `dotnet ef` is not installed globally, install it first or use your preferred EF tooling workflow.

4. Run the API:

```bash
dotnet run --project NZWalks.API --launch-profile https
```

5. Run the UI:

```bash
dotnet run --project NZWalks.UI --launch-profile https
```

6. Open the applications:

- API Swagger: `https://localhost:7032/swagger`
- UI: `https://localhost:7102`

## Authentication Notes

- `POST /api/Auth/Register` creates a user for self-registration
- self-registered users are assigned the `Reader` role when that role exists
- region and walk create/update/delete endpoints require the `Writer` role
- image upload requires an authenticated user

There is currently no dedicated login/register UI in `NZWalks.UI`; authentication is exposed through the API only.

## UI Configuration

`NZWalks.UI` reads the API base URL from:

- `NZWalks.UI/appsettings.json`
- key: `ApiSettings:BaseUrl`

Update that value if the API runs on a different host or port.

## Operational Notes

- Swagger is enabled in Development only
- uploaded files are stored locally in the API project's `Images` folder
- local-development secrets and connection strings are still stored in config files in this repository; move them to environment-specific secret storage before any shared or production deployment

## Contributing

If you plan to extend this repository, recommended next steps are:

- add automated tests
- add CI
- expand the UI beyond region management
- harden environment-specific configuration and secrets
- decide whether to keep this as a demo/sample app or evolve it into a fuller product
