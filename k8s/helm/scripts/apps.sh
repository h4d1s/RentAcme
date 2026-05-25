#!/usr/bin/env bash
set -euo pipefail

echo "Deploying APPLICATION services..."

cd ../

# namespaces
kubectl apply -f apps/namespaces.yaml

deploy_service () {
	SERVICE=$1
	APP_PATH=$2

	echo "Deploying $SERVICE..."

	helm upgrade --install "$SERVICE" "$APP_PATH" \
	  --namespace "apps" \
	  --create-namespace \
	  --wait \
	  --timeout 5m \
	  --atomic

	echo "$SERVICE deployed"
	echo ""
}

#deploy_service api-gateway apps/api-gateway
deploy_service inventory-api apps/inventory-api
deploy_service notification-api apps/notification-api
deploy_service payment-api apps/payment-api
deploy_service reservation-api apps/reservation-api
deploy_service saga-orchestration-statemachine apps/saga-orchestration-statemachine
deploy_service user-api apps/user-api

echo "App services deployed"
cd ./scripts