# Dice Roll

This repository now contains:

- Dice.Api: F# API for dice roll endpoints
- Dice.Client: SvelteKit client project

## Current Hosting Model

Dice.Client is built as static assets and copied into Dice.Api/wwwroot.
Dice.Api then serves the client at the root URL and continues to serve roll endpoints under /roll.

## Prerequisites

- .NET SDK 10
- Node.js LTS (includes npm)

## Local Development

1. Build and copy client assets:

   pwsh -NoProfile -ExecutionPolicy Bypass -File scripts/build-client.ps1

2. Run API:

   dotnet run --project Dice.Api/Dice.Api.fsproj --launch-profile https

Or run the VS Code task:

- Run Dice.Api (this now depends on Build Dice.Client)

## Client Project

Client source lives in Dice.Client.

Key scripts:

- npm run dev
- npm run build
- npm run preview

## API Endpoints

Examples:

- /roll/1/d/6
- /roll/10/d/20

## Notes

- Generated client assets are not committed.
- Dice.Api/wwwroot/index.html is a placeholder until the SvelteKit build is copied in.
