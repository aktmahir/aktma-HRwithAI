# Onboarding Guide

## Who this project is for
This project is intended for developers, operators, and HR product stakeholders who need a secure, containerized API foundation for HR workflows.

## First steps for a new developer
1. Install the .NET 8 SDK and Docker Desktop.
2. Copy [.env.example](../.env.example) to .env and fill in the required values.
3. Start the local stack with Docker Compose or run the API directly against a local PostgreSQL instance.
4. Run the API and domain test suites with `dotnet test tests/HrManagement.Domain.Tests/HrManagement.Domain.Tests.csproj` and `dotnet test tests/HrManagement.Api.Tests/HrManagement.Api.Tests.csproj`.
5. Review the API endpoints and the operational documents in the docs folder.
6. Verify local health and readiness endpoints with `/health` and `/ready` once the API is running.
7. Run the preflight check with `./scripts/preflight-check.ps1` before starting the stack.
8. Exercise the backup validation helper with `bash ./scripts/verify-backup-restore.sh` before release-related work.

## Common support paths
- Use the runbook for deployment, rollback, and incident handling.
- Use the security checklist before production changes.
- Use the release checklist before every deployment.
- Open a bug report or feature request through the GitHub issue templates when needed.
