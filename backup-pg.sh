#!/bin/bash
set -e

BACKUP_DIR="${BACKUP_DIR:-/backups}"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="${BACKUP_DIR}/hr_management_${TIMESTAMP}.sql.gz"

mkdir -p "$BACKUP_DIR"

echo "Starting PostgreSQL backup..."
pg_dump -h "${POSTGRES_HOST:-db}" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" | gzip > "$BACKUP_FILE"

echo "Backup created: $BACKUP_FILE"

# Validate backup integrity
echo "Validating backup integrity..."
gzip -t "$BACKUP_FILE" || {
    echo "ERROR: Backup validation failed - corrupted backup file"
    rm -f "$BACKUP_FILE"
    exit 1
}

# Test restore to temporary database (optional validation)
if [ -n "$VALIDATE_RESTORE" ]; then
    echo "Testing restore to temporary database..."
    TEST_DB="hr_management_restore_test_$$"
    createdb -h "${POSTGRES_HOST:-db}" -U "${POSTGRES_USER}" "$TEST_DB" 2>/dev/null || true
    zcat "$BACKUP_FILE" | psql -h "${POSTGRES_HOST:-db}" -U "${POSTGRES_USER}" -d "$TEST_DB" 2>/dev/null || {
        echo "ERROR: Backup restore test failed"
        dropdb -h "${POSTGRES_HOST:-db}" -U "${POSTGRES_USER}" "$TEST_DB" 2>/dev/null || true
        rm -f "$BACKUP_FILE"
        exit 1
    }
    dropdb -h "${POSTGRES_HOST:-db}" -U "${POSTGRES_USER}" "$TEST_DB" 2>/dev/null || true
    echo "Restore validation passed"
fi

# Optional: upload to cloud storage if BACKUP_S3_BUCKET is set
if [ -n "$BACKUP_S3_BUCKET" ]; then
    echo "Uploading to S3..."
    aws s3 cp "$BACKUP_FILE" "s3://${BACKUP_S3_BUCKET}/"
fi

# Keep only last 7 days of backups
find "$BACKUP_DIR" -name "*.sql.gz" -mtime +7 -delete
