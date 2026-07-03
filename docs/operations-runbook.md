# Operations Runbook

## Purpose
This runbook provides the minimum operating procedure for deploying, monitoring, rolling back, and recovering the HR Management API.

## Pre-Deployment Checklist
- Confirm the target environment has the required secrets in the secret store.
- Confirm the latest image tag is available in the container registry.
- Confirm the database connection string and migration strategy are valid.
- Review the latest deployment diff and ensure the release notes are approved.
- Ensure backup snapshots or database dumps are available before production rollout.

## Deployment Steps
1. Build or pull the release image.
2. Apply or update the Kubernetes manifests or compose configuration.
3. Run the migration job before or alongside the new application rollout.
4. Verify health endpoints:
   - GET /health
   - GET /ready
   - GET /metrics
5. Smoke-test a core business route such as employee or department retrieval.

## Rollback Procedure
- If the deployment fails health checks, revert to the previous known-good image tag.
- For Kubernetes:
  - `kubectl rollout undo deployment/hr-management-api --namespace=<namespace>`
  - `kubectl rollout status deployment/hr-management-api --namespace=<namespace>`
- For Docker Compose, redeploy the previous image tag and restart the service.

## Migration Failure Handling
- If migrations fail, stop the rollout and do not proceed with the new application version.
- Check logs for database connectivity, schema drift, and blocking locks.
- Re-run migrations manually if the issue is resolved.
- Preserve a copy of failed migration output for incident review.

## Backup and Restore Verification
- Run the backup validation helper monthly or before significant releases.
- Use `bash ./scripts/verify-backup-restore.sh` to confirm the latest backup archive is readable.
- Execute the actual restore procedure in a non-production environment to verify the recovery path.

## Monitoring and Alerting
- Watch application logs for authentication, database, and LLM failures.
- Alert on sustained health check failures, elevated 5xx responses, and database connection errors.
- Review Prometheus-style metrics and application logs daily during the first week after release.

## Incident Response
1. Identify the issue and confirm affected users or services.
2. Contain impact by disabling risky routes or scaling back traffic if needed.
3. Apply rollback or mitigation steps immediately.
4. Capture evidence from logs, metrics, and release history.
5. Document the root cause and follow-up actions.
