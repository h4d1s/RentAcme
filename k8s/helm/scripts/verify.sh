#!/usr/bin/env bash
set -euo pipefail

echo "Verifying system status..."

kubectl get pods -A

echo ""
echo "Infra pods:"
kubectl get pods -n infra

echo ""
echo "App pods:"
kubectl get pods -n apps

echo ""
echo "Services:"
kubectl get svc -A

echo ""
echo "Verification complete"