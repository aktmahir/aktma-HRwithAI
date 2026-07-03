Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host 'Running domain tests...'
dotnet test tests/HrManagement.Domain.Tests/HrManagement.Domain.Tests.csproj --verbosity minimal
