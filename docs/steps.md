Milestone 1 — Repository and environment
[x] Create the .NET solution and React + TypeScript + Vite app
[x] Install and verify Ollama on Windows
[x] Create the initial README, .gitignore and configuration structure
[x] Add a basic GitHub Actions workflow

Milestone 2 — API and local AI
[x] Create the ASP.NET Core API and health endpoint
[ ] Add SQLite, EF Core and the first migration
[ ] Implement IAiProvider and OllamaProvider
[ ] Create conversations and persist chat messages
[ ] Handle missing models, timeouts and unavailable Ollama

Milestone 3 — Tool system and file access
[ ] Define IAssistantTool and the tool registry
[ ] Add strict tool input schemas and validation
[ ] Implement approved-folder search and safe file reading
[ ] Add permission enforcement and pending approval records
[ ] Record execution status and actual tool results

Milestone 4 — React desktop interface
[ ] Build the chat interface and conversation list
[ ] Add tool activity cards and clear execution statuses
[ ] Build the approval and rejection dialogs
[ ] Add model settings, connection status and error messages
[ ] Integrate Tauri and secure local API communication

Milestone 5 — Gmail
[ ] Configure Google OAuth and read-only Gmail scopes
[ ] Connect, search and retrieve messages
[ ] Summarize emails using the local model
[ ] Preview label and move operations
[ ] Require approval, revalidate and execute each modification

Milestone 6 — Quality and release
[ ] Write unit tests for tools, permissions and approval expiry
[ ] Write API integration tests using a temporary test database
[ ] Test Gmail integration with a dedicated test mailbox
[ ] Test invalid inputs, duplicate approvals and partial failures
[ ] Package for Windows and document installation and limitations