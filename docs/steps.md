# JobyAI implementation roadmap

This checklist tracks the multi-project solution under `src/`. The original
root-level `JobyAI.csproj` is a separate existing application and is not counted
as a completed feature in this roadmap.


`[x]` means the scaffold or feature is present in the repository; it does not
mean that future production hardening or testing is complete.

## Milestone 1 — Repository and application foundation

- [x] Create the .NET solution with API, Contracts, UnitTests, and IntegrationTests projects
- [x] Add a Tauri 2 desktop shell with a Vite and TypeScript frontend
- [x] Add the README, architecture and security documentation, and `.gitignore`
- [x] Add a GitHub Actions workflow that runs the .NET solution tests
- [ ] Document and verify the Ollama setup required by the new API

## Milestone 2 — API, persistence, and local AI

- [x] Create the ASP.NET Core API and health endpoint
- [x] Add built-in OpenAPI generation and interactive Scalar documentation in Development
- [x] Add the EF Core SQLite provider, `AppDbContext`, and configurable database connection
- [x] Add initial persistence model class scaffolds
- [x] Register entities and configure conversation, message, tool-execution, and approval relationships
- [x] Create and apply an initial local EF Core migration (migration files are ignored by Git)
- [x] Implement `IAiProvider` and `OllamaProvider`
- [x] Implement conversation list, detail, and delete endpoints
- [ ] Create conversations and persist chat messages
- [ ] Handle missing models, timeouts, and unavailable Ollama

## Milestone 3 — Tool system and file access

- [x] Add initial tool, agent, and security source folders and interface scaffolds
- [ ] Define the complete assistant-tool contract and implement the tool registry
- [ ] Add strict tool input schemas and validation
- [ ] Implement approved-folder search and safe file reading
- [ ] Enforce permissions and persist pending approval records
- [ ] Record tool execution status and actual results

## Milestone 4 — Desktop interface

- [x] Add the initial Tauri and Vite desktop shell
- [ ] Build the chat interface and conversation list
- [ ] Add tool activity cards and clear execution statuses
- [ ] Build approval and rejection dialogs
- [ ] Add model settings, connection status, and error messages
- [ ] Integrate the desktop UI with the API using secure local communication

## Milestone 5 — Gmail

- [ ] Configure Google OAuth with the minimum required scopes
- [ ] Connect to Gmail, search messages, and retrieve message content
- [ ] Summarize emails using the local model
- [ ] Preview label and move operations
- [ ] Require approval, revalidate permissions, and execute modifications

## Milestone 6 — Quality and release

- [x] Add unit-test and integration-test project scaffolds
- [x] Verify the initial migration and cascade relationships with an in-memory SQLite integration test
- [ ] Write meaningful unit tests for tools, permissions, and approval expiry
- [ ] Write API integration tests using a temporary SQLite database
- [ ] Test Gmail integration with a dedicated test mailbox
- [ ] Test invalid inputs, duplicate approvals, and partial failures
- [ ] Package for Windows and document installation and limitations

## Current API setup

Run the API from the repository root:

```powershell
dotnet run --project src/JobyAI.Api
```

In Development, interactive API documentation is at `/scalar` and the OpenAPI
document is at `/openapi/v1.json`. The default SQLite file is
`src/JobyAI.Api/Data/jobyai.db` when the API project is used as the content root.
Conversation list, detail, and delete operations are available at
`/api/v1/conversations`.
The database file is local and ignored by Git. The initial migration is tracked
locally but is ignored by Git and is not available in a fresh clone. To create
a local initial migration or regenerate it after cloning, run from the
repository root:

```powershell
dotnet ef migrations add InitialCreate --project src/JobyAI.Api --startup-project src/JobyAI.Api --output-dir Data/Migrations
dotnet ef database update --project src/JobyAI.Api --startup-project src/JobyAI.Api
```
