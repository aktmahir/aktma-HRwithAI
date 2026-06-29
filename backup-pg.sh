#!/bin/bash
set -e

BACKUP_DIR="${BACKUP_DIR:-/backups}"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="${BACKUP_DIR}/hr_management_${TIMESTAMP}.sql.gz"

mkdir -p "$BACKUP_DIR"

echo "Starting PostgreSQL backup..."
pg_dump -h "${POSTGRES_HOST:-db}" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" | gzip > "$BACKUP_FILE"

echo "Backup created: $BACKUP_FILE"

# Optional: upload to cloud storage if BACKUP_S3_BUCKET is set
if [ -n "$BACKUP_S3_BUCKET" ]; then
    echo "Uploading to S3..."
    aws s3 cp "$BACKUP_FILE" "s3://${BACKUP_S3_BUCKET}/"
fi

# Keep only last 7 days of backups
find "$BACKUP_DIR" -name "*.sql.gz" -mtime +7 -delete
