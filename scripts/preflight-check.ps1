Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$requiredFiles = @('.env.example', 'docker-compose.yml', 'src/HrManagement.Api/HrManagement.Api.csproj')
foreach ($file in $requiredFiles) {
    if (-not (Test-Path $file)) {
        throw "Required file not found: $file"
    }
}

$requiredEnvVars = @('Jwt__Secret')
if ([Environment]::GetEnvironmentVariable('ASPNETCORE_ENVIRONMENT') -eq 'Production') {
    $requiredEnvVars += 'ConnectionStrings__HrDatabase'
}

foreach ($name in $requiredEnvVars) {
    $value = [Environment]::GetEnvironmentVariable($name)
    if ([string]::IsNullOrWhiteSpace($value)) {
        throw "Required environment variable is missing: $name"
    }
}

if (-not (Test-Path '.env')) {
    Write-Warning '.env is missing; the script will still continue, but local runtime configuration may be incomplete.'
}

Write-Host 'Preflight checks passed.' -ForegroundColor Green
