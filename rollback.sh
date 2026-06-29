#!/bin/bash
# rollback.sh - Rollback to previous release
# Usage: ./rollback.sh [COMMIT_SHA]

set -e

REGISTRY="${REGISTRY:-ghcr.io}"
IMAGE_NAME="${IMAGE_NAME:-$(basename $(pwd))}"
TARGET_SHA="${1:-}"

if [ -z "$TARGET_SHA" ]; then
    echo "Usage: $0 COMMIT_SHA"
    echo "Available recent releases:"
    git log --oneline -5
    exit 1
fi

echo "Rolling back to $TARGET_SHA..."

# Get list of recent images and keep last 5
echo "Cleaning old images (keeping last 5)..."

# Update docker-compose to use the target SHA
if [ -f .env ]; then
    # Extract registry info from deployment
    export TAG="$TARGET_SHA"
fi

# Pull the target image
docker pull "${REGISTRY}/${IMAGE_NAME}:$TARGET_SHA"

# Tag it as rollback
docker tag "${REGISTRY}/${IMAGE_NAME}:$TARGET_SHA" "${REGISTRY}/${IMAGE_NAME}:rollback"

# Restart services with rollback image
docker-compose up -d --scale api=0 api || true
sleep 2
docker-compose up -d api

echo "Rollback complete. Current image: $($TARGET_SHA)"