#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
BACKUP_DIR="${BACKUP_DIR:-$SCRIPT_DIR/backups}"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="${BACKUP_DIR}/hr_management_${TIMESTAMP}.sql.gz"
ENCRYPTED_BACKUP_FILE="${BACKUP_FILE}.gpg"

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

# Optional: encrypt backup if GPG key is provided
if [ -n "${BACKUP_GPG_KEY:-}" ]; then
    echo "Encrypting backup with GPG..."
    gpg --batch --yes --recipient "$BACKUP_GPG_KEY" --output "$ENCRYPTED_BACKUP_FILE" --encrypt "$BACKUP_FILE"
    rm -f "$BACKUP_FILE"
    echo "Encrypted backup created: $ENCRYPTED_BACKUP_FILE"
    BACKUP_FILE="$ENCRYPTED_BACKUP_FILE"
fi

# Test restore to temporary database (optional validation)
if [ -n "$VALIDATE_RESTORE" ]; then
    echo "Testing restore to temporary database..."
    TEST_DB="hr_management_restore_test_$$"
    createdb -h "${POSTGRES_HOST:-db}" -U "${POSTGRES_USER}" "$TEST_DB" 2>/dev/null || true
    
    if [ -f "${BACKUP_FILE}" ]; then
        if [[ "$BACKUP_FILE" == *.gpg ]]; then
            gpg --batch --yes --decrypt "$BACKUP_FILE" | psql -h "${POSTGRES_HOST:-db}" -U "${POSTGRES_USER}" -d "$TEST_DB" 2>/dev/null || {
                echo "ERROR: Backup restore test failed"
                dropdb -h "${POSTGRES_HOST:-db}" -U "${POSTGRES_USER}" "$TEST_DB" 2>/dev/null || true
                rm -f "$BACKUP_FILE"
                exit 1
            }
        else
            zcat "$BACKUP_FILE" | psql -h "${POSTGRES_HOST:-db}" -U "${POSTGRES_USER}" -d "$TEST_DB" 2>/dev/null || {
                echo "ERROR: Backup restore test failed"
                dropdb -h "${POSTGRES_HOST:-db}" -U "${POSTGRES_USER}" "$TEST_DB" 2>/dev/null || true
                rm -f "$BACKUP_FILE"
                exit 1
            }
        fi
    fi
    
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
find "$BACKUP_DIR" -name "*.sql.gz.gpg" -mtime +7 -delete

echo "Backup process completed successfully"
