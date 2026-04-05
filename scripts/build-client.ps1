param(
    [switch]$RunUnitTests
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$clientPath = Join-Path $repoRoot 'Dice.Client'
$apiWwwroot = Join-Path $repoRoot 'Dice.Api\wwwroot'

$npm = Get-Command npm -ErrorAction SilentlyContinue
if (-not $npm) {
    $defaultNodePath = 'C:\Program Files\nodejs'
    $defaultNpm = Join-Path $defaultNodePath 'npm.cmd'

    if (Test-Path $defaultNpm) {
        # Node was installed but this shell has a stale PATH; patch PATH for this run.
        $env:Path = "$defaultNodePath;$env:Path"
        $npm = Get-Command npm -ErrorAction SilentlyContinue
    }
}

if (-not $npm) {
    throw 'npm was not found. Install Node.js (which includes npm) to build Dice.Client.'
}

Push-Location $clientPath
try {
    & $npm.Source install
    if ($LASTEXITCODE -ne 0) {
        throw 'npm install failed for Dice.Client.'
    }

    if ($RunUnitTests) {
        Write-Host 'Running Dice.Client unit tests...'
        & $npm.Source run test:unit
        if ($LASTEXITCODE -ne 0) {
            throw 'npm run test:unit failed for Dice.Client.'
        }
    }

    & $npm.Source run build
    if ($LASTEXITCODE -ne 0) {
        throw 'npm run build failed for Dice.Client.'
    }
}
finally {
    Pop-Location
}

$clientBuildPath = Join-Path $clientPath 'build'
if (-not (Test-Path $clientBuildPath)) {
    throw "Client build output was not found at $clientBuildPath"
}

if (Test-Path $apiWwwroot) {
    Remove-Item -Path (Join-Path $apiWwwroot '*') -Recurse -Force -ErrorAction SilentlyContinue
} else {
    New-Item -ItemType Directory -Path $apiWwwroot | Out-Null
}

Copy-Item -Path (Join-Path $clientBuildPath '*') -Destination $apiWwwroot -Recurse -Force
Write-Host "Copied Dice.Client build output to $apiWwwroot"
