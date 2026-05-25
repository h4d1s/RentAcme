#!/usr/bin/env bash
set -euo pipefail

echo "Starting FULL Rent-Acme platform..."

bash infra.sh
bash apps.sh
bash verify.sh

echo "ALL SYSTEMS DEPLOYED SUCCESSFULLY"