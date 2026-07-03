#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
BACKUP_DIR="${BACKUP_DIR:-$SCRIPT_DIR/../backups}"
LATEST_BACKUP=$(find "$BACKUP_DIR" -maxdepth 1 -type f -name "*.sql.gz" | sort | tail -n 1 || true)

if [ -z "$LATEST_BACKUP" ]; then
  echo "No backup archive found in $BACKUP_DIR"
  exit 1
fi

echo "Validating backup archive: $LATEST_BACKUP"
gzip -t "$LATEST_BACKUP"

echo "Backup archive is readable."

echo "Restore verification is a placeholder for your environment-specific restore process."
echo "Run the appropriate restore command in a non-production environment before release."
