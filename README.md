# JobyAI

JobyAI is a personal assistant project. This repository now includes a
multi-project application scaffold alongside the pre-existing root-level
application.

## Layout

- `src/JobyAI.Api` — ASP.NET Core API
- `src/JobyAI.Contracts` — shared API contracts
- `src/JobyAI.Desktop` — Tauri desktop application and Vite frontend
- `tests/JobyAI.UnitTests` — unit tests
- `tests/JobyAI.IntegrationTests` — integration tests
- `docs` — architecture and security notes

Build and run the .NET solution tests with:

```powershell
dotnet test JobyAI.sln
```
