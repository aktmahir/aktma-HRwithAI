# Release Checklist

## Before Release
- [ ] All tests pass.
- [ ] Security scan results are reviewed.
- [ ] Dependency update review is completed and any high-risk packages are assessed.
- [ ] Environment variables and secrets are verified.
- [ ] Migration plan is approved.
- [ ] Backup is available and the restore drill has been run or scheduled.

## During Release
- [ ] Deploy the new image.
- [ ] Run migrations.
- [ ] Verify health and readiness endpoints.
- [ ] Smoke-test key endpoints.

## After Release
- [ ] Confirm monitoring is healthy.
- [ ] Review logs for errors.
- [ ] Record the release version and any follow-up work.
