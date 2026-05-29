#!/usr/bin/env bash
set -euo pipefail

echo "Deploying APPLICATION services..."

#namespaces
kubectl apply -f ../helm/apps/namespaces.yaml

cd ../helm

deploy_service () {
	SERVICE=$1
	APP_PATH=$2

	echo "Deploying $SERVICE..."

	HELM_ARGS=(
		upgrade --install "$SERVICE" "$APP_PATH"
		--namespace "apps"
		--wait
		--timeout 5m
		--atomic
		--debug
		"${EXTRA_ARGS[@]}"
	)

	if [ -f "$APP_PATH/secrets.yaml" ]; then
		HELM_ARGS+=(-f "$APP_PATH/secrets.yaml")
	fi

	helm "${HELM_ARGS[@]}"

	echo "$SERVICE deployed"
	echo ""
}

deploy_service api-gateway apps/api-gateway
deploy_service inventory-api apps/inventory-api
deploy_service notification-api apps/notification-api
deploy_service payment-api apps/payment-api
deploy_service reservation-api apps/reservation-api
deploy_service saga-orchestration-statemachine apps/saga-orchestration-statemachine
deploy_service user-api apps/user-api

echo "App services deployed"
cd ../scripts