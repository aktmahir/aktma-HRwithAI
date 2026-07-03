# Security and Compliance Checklist

## Authentication and Authorization
- Enforce JWT authentication for all business APIs.
- Validate issuer, audience, expiration, and signing key in every environment.
- Apply role-based access control for HR-sensitive endpoints.
- Review admin and privileged access regularly.

## Data Protection
- Keep secrets in a managed secret store, not in source control.
- Restrict database access to the application service account only.
- Avoid logging sensitive personal data and tokens.
- Define data retention and deletion rules for employee and leave records.

## Infrastructure and Runtime
- Keep container images up to date.
- Scan images and dependencies for vulnerabilities before release.
- Use TLS termination at the edge and enforce HTTPS in production.
- Review CORS origins and allowed methods regularly.

## Operational Governance
- Require review and approval for production changes.
- Maintain change logs and release records.
- Run periodic restore drills for backups.
- Document incident response and escalation paths.
