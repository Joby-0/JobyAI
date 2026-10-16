# JobyAI

JobyAI is a personal assistant project. This repository includes a new
multi-project application alongside the pre-existing root-level application.

## Layout

- `src/JobyAI.Api` — ASP.NET Core API
- `src/JobyAI.Contracts` — shared API contracts
- `src/JobyAI.Desktop` — Tauri desktop application and Vite frontend
- `tests/JobyAI.UnitTests` — unit tests
- `tests/JobyAI.IntegrationTests` — integration tests
- `docs/steps.md` — implementation roadmap and current setup instructions
- `docs/architecture.md` — project structure and architecture
- `SECURITY.md` — GitHub security policy
- `docs/security.md` — security design notes

## Project status

The API includes a health endpoint, development OpenAPI/Scalar documentation,
EF Core SQLite registration, and conversation list/detail/delete endpoints.
The context and database connection are configured, with conversation/message,
conversation/tool execution, and tool execution/approval relationships. Migration files are
ignored by Git and are not included in GitHub; recreate them locally before
using EF Core migrations after cloning. The desktop project is an initial
Tauri/Vite/TypeScript shell; chat creation, approval, Gmail, and tool workflows
remain on the roadmap.

See the [implementation roadmap](docs/steps.md) for milestone-by-milestone
progress and API run instructions.

## Run the API

```powershell
dotnet run --project src/JobyAI.Api
```

In Development, open `/scalar` for interactive API documentation or
`/openapi/v1.json` for the OpenAPI document.

Build and run the .NET solution tests with:

```powershell
dotnet test JobyAI.sln
```

For security concerns, follow the reporting instructions in [SECURITY.md](SECURITY.md).
