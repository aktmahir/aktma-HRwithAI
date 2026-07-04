# HR Management API

.NET 8 Clean Architecture scaffold for an HR Management Web API with PostgreSQL persistence and a local LLM adapter.

## Folder Structure

```text
HrManagement.slnx
src/
  HrManagement.Domain/
    Common/
    Employees/
    Leave/
  HrManagement.Application/
    Abstractions/
      Ai/
      Persistence/
    CvScreening/
    LeaveRequests/
    PerformanceReviews/
  HrManagement.Infrastructure/
    Ai/
    Persistence/
      Configurations/
  HrManagement.Api/
    Controllers/
    Dockerfile
    appsettings.Test.json
docker-compose.yml
docs/
  architecture-notes.md
  change-log.md
  onboarding-guide.md
  operations-runbook.md
  security-checklist.md
  release-checklist.md
tests/
  HrManagement.Domain.Tests/
  HrManagement.Application.Tests/
  HrManagement.Infrastructure.Tests/
  HrManagement.Api.Tests/
scripts/
  preflight-check.ps1
  run-tests.ps1
  start-dev.ps1
  verify-backup-restore.sh
k8s/
  deployment.yaml
  namespace-prod.yaml
  namespace-staging.yaml
  service.yaml
```

## Implemented Endpoints

- `POST /api/ai/cv-screening`
- `POST /api/ai/performance-review-analysis`
- `GET|POST /api/employees`, `GET|PUT|DELETE /api/employees/{id}`
- `GET|POST /api/departments`, `GET|PUT|DELETE /api/departments/{id}`
- `GET|POST /api/roles`, `GET|PUT|DELETE /api/roles/{id}`
- `GET|POST /api/leave-requests`, `POST /api/leave-requests/{id}/approve`, `POST /api/leave-requests/{id}/reject`

## Testing

Run the full test suite across all four test projects:

```bash
dotnet test HrManagement.slnx --configuration Release
```

Or run individual projects:

```bash
dotnet test tests/HrManagement.Domain.Tests/HrManagement.Domain.Tests.csproj
dotnet test tests/HrManagement.Application.Tests/HrManagement.Application.Tests.csproj
dotnet test tests/HrManagement.Infrastructure.Tests/HrManagement.Infrastructure.Tests.csproj
dotnet test tests/HrManagement.Api.Tests/HrManagement.Api.Tests.csproj
```

The solution includes xUnit + Moq tests protecting:
- Domain entity behavior and state transitions
- Application MediatR command handlers
- Infrastructure repository and EF Core mappings
- API controller behavior, authorization, and middleware

Convenience scripts:

```powershell
./scripts/preflight-check.ps1
./scripts/run-tests.ps1
./scripts/start-dev.ps1
```

## Local LLM

The Infrastructure adapter targets an Ollama-compatible endpoint by default:

```text
POST http://localhost:11434/api/generate
```

Use `Llm:BaseUrl`, `Llm:Model`, and `Llm:GeneratePath` to swap LM Studio, Ollama, or another local provider without changing Application code.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker & Docker Compose (for containerized run)
- PostgreSQL (if running without Docker)

### Local Development (with Docker Compose)

1. Copy the example environment file and fill in your secrets:
   ```bash
   cp .env.example .env
   # edit .env with strong passwords
   ```
2. Start the stack:
   ```bash
   docker-compose up -d --build
   ```
3. The API will be available at `http://localhost:8080`.
   Swagger UI (Development only): `http://localhost:8080/swagger`
   Health check: `http://localhost:8080/health`

### Local Development (without Docker)

1. Ensure PostgreSQL is running and create a database `hr_management`.
2. Set environment variables (or use `dotnet user-secrets`):
   ```bash
   export ConnectionStrings__HrDatabase="Host=localhost;Port=5432;Database=hr_management;Username=hr_user;Password=your_password"
   export Llm__BaseUrl=http://localhost:11434
   export Llm__Model=llama3
   export Llm__GeneratePath=/api/generate
   export ASPNETCORE_ENVIRONMENT=Development
   ```
3. Run the API:
   ```bash
   dotnet run --project src/HrManagement.Api/HrManagement.Api.csproj
   ```

## Configuration

All settings are driven by environment variables (or `appsettings.json`/`appsettings.{Environment}.json`). The following are the key variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name (Development, Staging, Production) | Development |
| `ASPNETCORE_URLS` | URLs Kestrel listens on | `http://+:8080` |
| `ConnectionStrings__HrDatabase` | PostgreSQL connection string | *required* |
| `Database__ApplyMigrationsOnStartup` | Run EF Core migrations on startup | `false` |
| `Llm__BaseUrl` | Base URL of the LLM provider (Ollama) | `http://ollama:11434` |
| `Llm__Model` | Model name | `llama3` |
| `Llm__GeneratePath` | Generate endpoint path | `/api/generate` |
| `AllowedOrigins` | Comma-separated CORS origins | `http://localhost:3000` |
| `IpRateLimiting__*` | Rate limiting rules (see `appsettings.json`) | *configured* |

**Never commit `.env` or any file containing real secrets.** Use a secret manager (GitHub Secrets, Azure Key Vault, AWS Secrets Manager, HashiCorp Vault) in production.

## Production Deployment

### Docker Image

The `Dockerfile` uses a multi-stage build, runs as non-root user (`appuser`), and includes a health check. Build and push:

```bash
docker build -t your-registry/hr-management-api:latest -f src/HrManagement.Api/Dockerfile .
docker push your-registry/hr-management-api:latest
```

### Kubernetes / Container Orchestration

- Use the provided `docker-compose.yml` as a reference for service definitions.
- Set `ASPNETCORE_ENVIRONMENT=Production`.
- Provide secrets via your platform's secret store (Kubernetes Secrets, Docker Swarm secrets, etc.).
- Enable TLS termination at ingress/controller (NGINX, Traefik, Azure Application Gateway, etc.).
- Configure resource limits/requests (see `deploy.resources` in compose file).
- Run database migrations as an init container or a separate job (`dotnet ef database update`).

### Health Checks & Monitoring

- Liveness: `GET /health` (checks DB and LLM connectivity).
- Readiness: `GET /ready` (app is ready to accept traffic).
- LLM health: `GET /health/llm` (non-blocking LLM provider check).
- Metrics: `GET /metrics` exposes Prometheus metrics via `prometheus-net.AspNetCore` (process memory, CPU, and ASP.NET Core request metrics).
- Logging: Serilog JSON format to `Logs/log-.json` (compact JSON) and console, 14-day rotation. Forward to ELK/Loki/Seq via log forwarder.
- Graceful shutdown: The host flushes Serilog buffers on SIGTERM and drains in-flight requests before stopping.

### Database Backup & Restore

- **Automated backup**: Enable the backup profile: `docker-compose --profile backup up -d` runs a one-off backup to `/backups`.
- **Manual backup**: 
  ```bash
  docker exec $(docker ps -q -f name=hr_db) pg_dump -U hr_user hr_management | gzip > backup_$(date +%Y%m%d).sql.gz
  ```
- **Restore**:
  ```bash
  zcat backup_20240101.sql.gz | docker exec -i $(docker ps -q -f name=hr_db) psql -U hr_user hr_management
  ```
- **Cloud storage**: Set `BACKUP_S3_BUCKET` in `.env` to auto-upload backups.
- **Quarterly restore test**: Run restore procedure monthly (documented above).
- **Operational backup drill**: Run `bash ./scripts/verify-backup-restore.sh` to validate the backup archive and restore path in a non-production environment.

## CI/CD Pipeline

A GitHub Actions workflow (`.github/workflows/ci.yml`) runs on every push/PR:

1. **Build & Test** – Restores, builds, runs unit tests, and checks code formatting.
2. **Security Scan** – Trivy vulnerability scanner (filesystem + Docker image) with GitHub Security tab integration.
3. **Docker** – Builds and pushes multi-arch image to GHCR with SHA tag (registry retention keeps last 5).
4. **Deploy Staging** – Placeholder for staging deployment.
5. **Deploy Production** – Placeholder for production deployment (requires manual approval).

### Rollback

Images are tagged by commit SHA for easy rollback:

```bash
# View recent commits
git log --oneline -5

# Rollback to specific version
./rollback.sh abc123def456

# Or manually
docker pull ghcr.io/ORG/hr-management-api:abc123def456
docker-compose up -d api=ghcr.io/ORG/hr-management-api:abc123def456
```

Configure the following repository secrets for the workflow:
- `GITHUB_TOKEN` (automatically provided) for pushing to GHCR.
- Any additional secrets needed for your deployment steps (kubeconfig, cloud credentials, etc.).

## Security Hardening (Already Applied)

- Non-root container user (`appuser`).
- Health checks in Dockerfile and Compose (`/ready` and `/health` endpoints).
- Resource limits in Compose.
- Strict CORS (whitelisted origins, specific methods/headers).
- Rate limiting (6 req/10s per IP) via `AspNetCoreRateLimit`.
- Security headers: HSTS, CSP, X-Frame-Options, Referrer-Policy.
- Response compression (Brotli/Gzip).
- TLS termination via Traefik reverse proxy with Let's Encrypt.
- Secrets out of source control (`.env` ignored).
- Automated dependency scanning (Dependabot / GitHub code scanning) – enable in repo settings.

## Operational Guidance

- Architecture overview: [docs/architecture-notes.md](docs/architecture-notes.md)
- Change log: [docs/change-log.md](docs/change-log.md)
- New contributor onboarding: [docs/onboarding-guide.md](docs/onboarding-guide.md)
- Deployment, rollback, and incident steps: [docs/operations-runbook.md](docs/operations-runbook.md)
- Security and compliance checklist: [docs/security-checklist.md](docs/security-checklist.md)
- Release checklist: [docs/release-checklist.md](docs/release-checklist.md)

## Extending the API

- Add new endpoints in `src/HrManagement.Api/Controllers/`.
- Business logic goes in `src/HrManagement.Application/`.
- Domain entities in `src/HrManagement.Domain/`.
- Infrastructure implementations (EF Core, LLM client) in `src/HrManagement.Infrastructure/`.

## API Behavior

- List endpoints support pagination via `?page=1&pageSize=50` (max 200).
- `GET /api/leave-requests` supports filtering via `?employeeId=`, `?startDate=`, `?endDate=`, `?status=Pending|Approved|Rejected`.
- Audit logs redact email addresses before writing to Serilog sinks.

## Data Retention

- `LeaveRequest` records are immutable once reviewed.
- `Employee` and `Department` records support soft updates via audit trail.
- Deleted records are removed from the database immediately.
- Configure automated archival or purging via a background hosted service as needed for compliance.

## License

MIT