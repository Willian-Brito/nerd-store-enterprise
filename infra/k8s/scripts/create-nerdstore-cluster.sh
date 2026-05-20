#!/bin/bash

set -e

##########################
# Variáveis
##########################

PROFILE="nerdstore-enterprise"
NAMESPACE_APP="nerdstore"
NAMESPACE_INFRA="infra"

##########################
# Funções
##########################

log() {
  echo ""
  echo "======================================"
  echo "🚀 $1"
  echo "======================================"
}

wait_deployment() {
  kubectl rollout status deployment/$1 -n $2
}

##########################
# Iniciando Minikube
##########################

log "Iniciando Minikube"

minikube start -n 2 -p $PROFILE

log "Habilitando Ingress"

minikube addons enable ingress -p $PROFILE

##########################
# Criando Namespaces
##########################

log "Criando namespaces"

kubectl create namespace $NAMESPACE_APP --dry-run=client -o yaml | kubectl apply -f -
kubectl create namespace $NAMESPACE_INFRA --dry-run=client -o yaml | kubectl apply -f -

##########################
# Configurando HTTPS
##########################

log "Configurando SSL Manager"

kubectl apply -f ../helm-charts/manager-ssl/manager-ssl.yaml -n $NAMESPACE_APP

sleep 20

log "Executando Job SSL"

kubectl delete job ssl-manager-manual -n $NAMESPACE_APP --ignore-not-found

kubectl create job \
  --from=cronjob/ssl-manager-job \
  ssl-manager-manual \
  -n $NAMESPACE_APP

##########################
# Infraestrutura
##########################

log "Subindo RabbitMQ"

kubectl apply -f ../helm-charts/rabbitmq -R

log "Subindo SQL Server"

kubectl apply -f ../helm-charts/sqlserver -R

log "Configurando Helm PostgreSQL"

helm repo add cetic https://cetic.github.io/helm-charts || true
helm repo update

log "Subindo PostgreSQL"

helm upgrade --install postgresql \
  cetic/postgresql \
  -n $NAMESPACE_INFRA

##########################
# Secrets
##########################

log "Criando secrets"

kubectl apply -f ../helm-charts/api-identity/identity-connection-secret.yaml -n $NAMESPACE_APP
kubectl apply -f ../helm-charts/api-catalog/catalog-connection-secret.yaml -n $NAMESPACE_APP
kubectl apply -f ../helm-charts/api-customer/customer-connection-secret.yaml -n $NAMESPACE_APP
kubectl apply -f ../helm-charts/api-cart/cart-connection-secret.yaml -n $NAMESPACE_APP
kubectl apply -f ../helm-charts/api-payment/payment-connection-secret.yaml -n $NAMESPACE_APP
kubectl apply -f ../helm-charts/api-orders/order-connection-secret.yaml -n $NAMESPACE_APP
kubectl apply -f ../helm-charts/web-status/status-connection-secret.yaml -n $NAMESPACE_APP

##########################
# Microsserviços
##########################

deploy_service() {
  SERVICE_NAME=$1
  VALUES_FILE=$2

  log "Deployando $SERVICE_NAME"

  helm upgrade --install $SERVICE_NAME \
    -f ../helm-charts/api-common.yaml \
    -f $VALUES_FILE \
    ../helm-charts/api-template \
    -n $NAMESPACE_APP
}

deploy_service api-identity ../helm-charts/api-identity/values.yaml
deploy_service api-catalog ../helm-charts/api-catalog/values.yaml
deploy_service api-customer ../helm-charts/api-customer/values.yaml
deploy_service api-cart ../helm-charts/api-cart/values.yaml
deploy_service api-payment ../helm-charts/api-payment/values.yaml
deploy_service api-orders ../helm-charts/api-orders/values.yaml
deploy_service api-bff-checkout ../helm-charts/api-bff-checkout/values.yaml

##########################
# Aplicações Web
##########################

log "Deployando web-mvc"

helm upgrade --install web-mvc \
  -f ../helm-charts/web-mvc/values.yaml \
  ../helm-charts/api-template \
  -n $NAMESPACE_APP

log "Deployando web-status"

helm upgrade --install web-status \
  -f ../helm-charts/api-common.yaml \
  -f ../helm-charts/web-status/values.yaml \
  ../helm-charts/api-template \
  -n $NAMESPACE_APP

##########################
# Status Final
##########################

log "Ambiente criado com sucesso"

echo ""
echo "🌐 Execute o tunnel em outro terminal:"
echo ""
echo "minikube tunnel -p $PROFILE"
echo ""

echo "📝 Adicione no /etc/hosts:"
echo "OBS: Usar IP do minikube tunnel"
echo ""
echo "192.168.49.2 nerdstore.io"
echo "192.168.49.2 status.nerdstore.io"
echo ""

echo "🔎 Verificando Pods"
echo ""

kubectl get pods -A

echo ""
echo "✅ Ambiente pronto!"