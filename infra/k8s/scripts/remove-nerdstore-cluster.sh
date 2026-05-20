#!/bin/bash

set -e

#############################
# Variáveis
#############################

PROFILE="nerdstore-enterprise"
NAMESPACE_APP="nerdstore"
NAMESPACE_INFRA="infra"

#############################
# Funções
#############################

log() {
  echo ""
  echo "======================================"
  echo "🧹 $1"
  echo "======================================"
}

#############################
# Removendo aplicações WEB
#############################

log "Removendo aplicações WEB"

helm uninstall web-mvc -n $NAMESPACE_APP || true
helm uninstall web-status -n $NAMESPACE_APP || true

#############################
# Removendo microsserviços
#############################

log "Removendo microsserviços"

helm uninstall api-bff-checkout -n $NAMESPACE_APP || true
helm uninstall api-orders -n $NAMESPACE_APP || true
helm uninstall api-payment -n $NAMESPACE_APP || true
helm uninstall api-cart -n $NAMESPACE_APP || true
helm uninstall api-customer -n $NAMESPACE_APP || true
helm uninstall api-catalog -n $NAMESPACE_APP || true
helm uninstall api-identity -n $NAMESPACE_APP || true

#############################
# Removendo infraestrutura
#############################

log "Removendo PostgreSQL"

helm uninstall postgresql -n $NAMESPACE_INFRA || true

log "Removendo RabbitMQ"

kubectl delete -f ./helm-charts/rabbitmq -R --ignore-not-found=true || true

log "Removendo SQL Server"

kubectl delete -f ./helm-charts/sqlserver -R --ignore-not-found=true || true

#############################
# Removendo SSL Manager
#############################

log "Removendo SSL Manager"

kubectl delete -f ./manager-ssl/manager-ssl.yaml -n $NAMESPACE_APP --ignore-not-found=true || true

kubectl delete job ssl-manager-manual \
  -n $NAMESPACE_APP \
  --ignore-not-found=true || true

#############################
# Removendo Secrets
#############################

log "Removendo Secrets"

kubectl delete secret rabbit-connection -n $NAMESPACE_APP --ignore-not-found=true || true
kubectl delete secret order-connection -n $NAMESPACE_APP --ignore-not-found=true || true
kubectl delete secret payment-connection -n $NAMESPACE_APP --ignore-not-found=true || true
kubectl delete secret cart-connection -n $NAMESPACE_APP --ignore-not-found=true || true
kubectl delete secret customer-connection -n $NAMESPACE_APP --ignore-not-found=true || true
kubectl delete secret catalog-connection -n $NAMESPACE_APP --ignore-not-found=true || true
kubectl delete secret identity-connection -n $NAMESPACE_APP --ignore-not-found=true || true
kubectl delete secret status-connection -n $NAMESPACE_APP --ignore-not-found=true || true

#############################
# Removendo namespaces
#############################

log "Removendo namespaces"

kubectl delete namespace $NAMESPACE_APP --ignore-not-found=true || true
kubectl delete namespace $NAMESPACE_INFRA --ignore-not-found=true || true

#############################
# Parando Minikube
#############################

log "Parando Minikube"

minikube stop -p $PROFILE || true

#############################
# Status final
#############################

log "Ambiente removido com sucesso"

echo ""
echo "✅ Todos os serviços foram removidos."
echo "🛑 Minikube parado."
echo ""