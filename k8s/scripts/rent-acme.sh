#!/usr/bin/env bash
set -euo pipefail

echo "Starting FULL Rent-Acme platform..."

bash ingress-ngix.sh
bash infra.sh
bash apps.sh

echo "ALL SYSTEMS DEPLOYED SUCCESSFULLY"