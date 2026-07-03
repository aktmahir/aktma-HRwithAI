Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not (Test-Path .env)) {
    Write-Host '.env file not found. Copy .env.example to .env and update it before starting the stack.' -ForegroundColor Yellow
    exit 1
}

Write-Host 'Starting Docker Compose services...'
docker compose up -d --build
