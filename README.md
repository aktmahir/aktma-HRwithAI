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
  HrManagement.Infrastructure/
    Ai/
    Persistence/
      Configurations/
  HrManagement.Api/
    Controllers/
    Dockerfile
docker-compose.yml
```

## Implemented Endpoints

- `POST /api/ai/cv-screening`
- `POST /api/ai/performance-review-analysis`
- `GET|POST /api/employees`, `GET|PUT|DELETE /api/employees/{id}`
- `GET|POST /api/departments`, `GET|PUT|DELETE /api/departments/{id}`
- `GET|POST /api/roles`, `GET|PUT|DELETE /api/roles/{id}`
- `GET|POST /api/leave-requests`, `POST /api/leave-requests/{id}/approve`, `POST /api/leave-requests/{id}/reject`

## Local LLM

The Infrastructure adapter targets an Ollama-compatible endpoint by default:

```text
POST http://localhost:11434/api/generate
```

Use `Llm:BaseUrl`, `Llm:Model`, and `Llm:GeneratePath` to swap LM Studio, Ollama, or another local provider without changing Application code.
