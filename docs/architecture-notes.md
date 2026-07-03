# Architecture Notes

## Current structure
The solution follows a clean architecture style with clear separation between domain, application, infrastructure, and API layers.

- Domain: business entities and invariants.
- Application: use cases, commands, and orchestration.
- Infrastructure: persistence, external services, and implementation details.
- API: controllers, request handling, and HTTP concerns.

## Design principles
- Keep domain logic inside the domain layer.
- Keep controllers thin and focused on HTTP concerns.
- Use abstractions in the application layer for infrastructure dependencies.
- Prefer explicit configuration for runtime behavior and secrets.

## Current hardening and operational capabilities
- The API now uses JWT-based authentication with role-based authorization policies for HR administration endpoints.
- Request validation is enforced for admin payloads and AI endpoints so malformed input fails fast with 400 responses.
- Structured audit logging and correlation IDs now improve traceability for sensitive administrative actions and error handling.
- Health checks, readiness probes, and basic metrics endpoints are exposed for deployment and monitoring workflows.
- Integration tests cover controller behavior, authorization, tracing, health probes, and basic API flows.

## Recommended evolution
- Add more persistence-focused integration tests as the domain model grows.
- Introduce contract testing and versioned API compatibility checks as the external surface expands.
- Extend observability with OpenTelemetry traces and richer service metrics over time.
