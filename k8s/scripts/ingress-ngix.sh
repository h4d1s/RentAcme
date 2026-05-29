#!/usr/bin/env bash
set -euo pipefail

echo "Installing Ingress-Nginx..."

kubectl apply -f ../ingress-nginx/namespaces.yaml

helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update

helm upgrade --install ingress-nginx ingress-nginx/ingress-nginx \
  --namespace ingress-nginx

kubectl apply -f ../ingress-nginx/mandatory.yaml
kubectl apply -f ../ingress-nginx/service-nodeport.yaml