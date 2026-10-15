# JobyAI implementation roadmap

This checklist tracks the multi-project solution under `src/`. The original
root-level `JobyAI.csproj` is a separate existing application and is not counted
as a completed feature in this roadmap.

[steps.md](../STEPS.md).



## Current API setup

Run the API from the repository root:

```powershell
dotnet run --project src/JobyAI.Api
```

In Development, interactive API documentation is at `/scalar` and the OpenAPI
document is at `/openapi/v1.json`. The default SQLite file is
`src/JobyAI.Api/Data/jobyai.db` when the API project is used as the content root.
The database file is local and ignored by Git. The initial migration is tracked
locally but is ignored by Git and is not available in a fresh clone. To create
a local initial migration or regenerate it after cloning, run from the
repository root:

```powershell
dotnet ef migrations add InitialCreate --project src/JobyAI.Api --startup-project src/JobyAI.Api --output-dir Data/Migrations
dotnet ef database update --project src/JobyAI.Api --startup-project src/JobyAI.Api
```
