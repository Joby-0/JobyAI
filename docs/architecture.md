# JobyAI architecture

## Projects

- `src/JobyAI.Api` is the ASP.NET Core API and orchestration host.
- `src/JobyAI.Contracts` contains types shared across the API and clients.
- `src/JobyAI.Desktop` is the Tauri 2 desktop shell with a Vite/TypeScript frontend.
- `tests/JobyAI.UnitTests` and `tests/JobyAI.IntegrationTests` hold automated tests.

The API is organized into controllers, agent orchestration, AI providers, tools,
security, data access, and services. Contracts are grouped by chat, tools, and
approvals. These are initial scaffolds; feature behavior can be added without
mixing responsibilities between projects.

The repository's existing root-level `JobyAI.csproj` remains in place and is
separate from this new solution.

## Build and test

```powershell
dotnet test JobyAI.sln
```

When running the API in Development, browse to `/scalar` for interactive API
documentation or `/openapi/v1.json` for the OpenAPI document.

For the desktop frontend, install dependencies from `src/JobyAI.Desktop` and run
`npm run build`. Tauri desktop builds additionally require the Rust toolchain and
platform prerequisites described in the Tauri documentation.
