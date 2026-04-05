# Dice-Roll Project Memory

Use this as the first-read context for new agent sessions in this repository.

## Project Snapshot

- Solution: `DiceRoller.slnx`
- API: `Dice.Api` (F# / Saturn / Giraffe)
- Tests: `Dice.Api.Tests` (currently passing)
- Frontend: `Dice.Client` (SvelteKit static build)
- Client unit tests: Vitest tests co-located under `Dice.Client/src/**`
- Hosting model: built client assets are copied into `Dice.Api/wwwroot` and served by the API.
- Infrastructure: Azure App Service Bicep files under `infrastructure/`
- CI/CD: Azure Pipelines via `azure-pipelines.yml`

## Architecture Decisions

- SvelteKit uses static adapter output for deployable static assets.
- `Dice.Api/Program.fs` serves static files and keeps `/roll` API routes intact.
- SPA fallback is enabled for non-file, non-`/roll` routes.
- Client and API are same-origin in this setup.

## Client Behavior (Current UX)

- Dice controls: d4, d6, d8, d10, d12, d20, and d% (d100).
- d4-d20 controls use split UI:
  - Main button label is fixed to the die sides value (4/6/8/10/12/20).
  - Main button rolls with currently selected count.
  - Dropdown selects count (1-10) and triggers roll on change.
  - After a successful roll, the selected count resets to 1 (without triggering another roll).
- d% is a single action button (rolls 1d100).
- Loading guard prevents overlapping requests.

## Results Header Rules

- For normal dice rolls: show `{count}d{sides}`.
- If `count > 1`, append ` (Total: {sum})`.
- For d100: header shows only `d%`.
- Roll values are rendered in a compact table (no header), with near-square row/column layout and max 10 columns.
- Empty trailing cells in the final row use invisible borders.
- A persistent live log appears after first roll and prepends new entries at the top.
- Live log entry format: `hh:mm:ss {count}d{sides} [v1, v2, ...]` (or `hh:mm:ss d% [value]`).

## Layout / Styling Notes

- Dark theme with green accent buttons.
- Dice icon assets were removed from the UI.
- Small viewport behavior (`max-width: 760px`): 3-column control grid (3/3/1 rows for 7 controls).

## Key Files

- API entry: `Dice.Api/Program.fs`
- Dice route handlers: `Dice.Api/DiceRoutes/*.fs`
- Client page: `Dice.Client/src/routes/+page.svelte`
- Client API wrapper: `Dice.Client/src/lib/api/dice.ts`
- Client testable helpers: `Dice.Client/src/lib/roll-utils.ts`
- Client unit tests: `Dice.Client/src/**/*.test.ts`
- Client build/deploy script: `scripts/build-client.ps1`
- Azure pipeline: `azure-pipelines.yml`
- Azure Bicep template: `infrastructure/main.bicep`
- Azure Bicep parameters: `infrastructure/dev.bicepparam`
- VS Code tasks: `.vscode/tasks.json`

## Build / Run Commands

- Build + deploy client to API static root:
  - `pwsh -NoProfile -ExecutionPolicy Bypass -File scripts/build-client.ps1`
- Build client, run client unit tests, and deploy client assets to API static root:
  - `pwsh -NoProfile -ExecutionPolicy Bypass -File scripts/build-client.ps1 -RunUnitTests`
- Run tests:
  - `dotnet test Dice.Api.Tests/Dice.Api.Tests.fsproj --logger "console;verbosity=normal"`
- Run client unit tests:
  - `cd Dice.Client && npm run test:unit`
- Run client unit tests in watch mode:
  - `cd Dice.Client && npm run test:unit:watch`
- Run API with HTTPS profile:
  - `dotnet run --project Dice.Api/Dice.Api.fsproj --launch-profile https`

## Azure Delivery Notes

- `azure-pipelines.yml` has `Build` and `Deploy` stages.
- Build stage restores the solution, runs API tests, runs client unit tests via `scripts/build-client.ps1 -RunUnitTests`, publishes the API, archives it, and publishes a `dice-api` pipeline artifact.
- Deploy stage targets an existing Azure Web App through `AzureWebApp@1` and is gated to `main`.
- Before real pipeline deployment, update `azureServiceConnection` and `webAppName` in `azure-pipelines.yml`.

## Infrastructure Notes

- `infrastructure/main.bicep` is resource-group scoped and declares an App Service plan plus a Web App.
- Current defaults are for Windows App Service on the Free tier: `F1` / `Free` with `alwaysOn = false`.
- Web App names must be globally unique in Azure App Service; plan names do not have the same global uniqueness requirement.
- Bicep deployments are declarative: create if missing, update if present in the target scope and the deploying identity has permission.

## Operational Notes

- If Node/NPM is unavailable in shell PATH, use a fresh terminal session.
- `scripts/build-client.ps1` also patches PATH with `C:\Program Files\nodejs` if the shell PATH is stale.
- Client build output is generated; source of truth is under `Dice.Client/`.
- Preferred workflow after UI edits: build-client script, then validate UI and API behavior.
- Preferred workflow after client logic edits: `npm run test:unit`, then `scripts/build-client.ps1 -RunUnitTests` if preparing a deployable build.
