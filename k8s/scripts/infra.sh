#!/usr/bin/env bash
set -euo pipefail

echo "Deploying INFRASTRUCTURE layer..."

# namespaces
kubectl apply -f ../helm/infra/namespaces.yaml

cd ../helm

deploy_service () {
	SERVICE=$1
	NAMESPACE=$2
	APP_PATH=$3
	shift 3

	EXTRA_ARGS=("$@")

	echo "Deploying $SERVICE..."

	HELM_ARGS=(
		upgrade --install "$SERVICE" "$APP_PATH"
		--namespace "$NAMESPACE"
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

#databases
deploy_service inventory-db infra-databases ./infra/databases/inventory-db
deploy_service keycloak-db infra-databases ./infra/databases/keycloak-db
deploy_service reservation-db infra-databases ./infra/databases/reservation-db
deploy_service saga-orchestration-statemachine-db infra-databases ./infra/databases/saga-orchestration-statemachine-db
deploy_service user-db infra-databases ./infra/databases/user-db

#auth
deploy_service keycloak infra-auth ./infra/auth/keycloak --set-file configRealm=../../configs/keycloak/realm-rent-acme.json

#messaging
deploy_service rabbitmq infra-messaging ./infra/messaging/rabbitmq

#observability
deploy_service grafana infra-observability ./infra/observability/grafana
deploy_service loki infra-observability ./infra/observability/loki --set-file configLoki=../../configs/loki/loki-config.yaml
deploy_service otel-collector infra-observability ./infra/observability/otel-collector
deploy_service prometheus infra-observability ./infra/observability/prometheus
deploy_service tempo infra-observability ./infra/observability/tempo

echo "Infra services deployed"
cd ../scripts