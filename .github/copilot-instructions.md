# Dice-Roll Project Memory

Use this as the first-read context for new agent sessions in this repository.

## Project Snapshot

- Solution: `DiceRoller.slnx`
- API: `Dice.Api` (F# / Saturn / Giraffe)
- Tests: `Dice.Api.Tests` (currently passing)
- Frontend: `Dice.Client` (SvelteKit static build)
- Hosting model: built client assets are copied into `Dice.Api/wwwroot` and served by the API.

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
- Client build/deploy script: `scripts/build-client.ps1`
- VS Code tasks: `.vscode/tasks.json`

## Build / Run Commands

- Build + deploy client to API static root:
  - `pwsh -NoProfile -ExecutionPolicy Bypass -File scripts/build-client.ps1`
- Run tests:
  - `dotnet test Dice.Api.Tests/Dice.Api.Tests.fsproj --logger "console;verbosity=normal"`
- Run API with HTTPS profile:
  - `dotnet run --project Dice.Api/Dice.Api.fsproj --launch-profile https`

## Operational Notes

- If Node/NPM is unavailable in shell PATH, use a fresh terminal session.
- Client build output is generated; source of truth is under `Dice.Client/`.
- Preferred workflow after UI edits: build-client script, then validate UI and API behavior.
