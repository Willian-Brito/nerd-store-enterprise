#!/bin/bash

# Gerar o template do Helm para o api-identity
helm template --debug -f ./api-identity/values.yaml ./api-template > api-identity.yaml

# Gerar o yaml da secret de conexão com o banco de dados PostgreSQL
kubectl create secret generic identity-connection -n nerdstore --from-literal=CUSTOMCONNSTR_DefaultConnection="Server=postgresql.infra.svc.cluster.local;Port=5432;Database=DBUsers;User Id=postgres;Password=postgres;" -o yaml --dry-run=client > ./api-identity/identity-connection-secret.yaml

# Habilitar o addon de ingress no minikube (perfil nerdstore-enterprise)
minikube addons enable ingress -p nerdstore-enterprise

# Aplicar o manifest do identity-connection-secret no cluster Kubernetes
kubectl apply -f ./api-identity/identity-connection-secret.yaml -n nerdstore

# Instalar o api-identity via Helm Chart
helm install --debug api-identity -f api-common.yaml -f ./api-identity/values.yaml ./api-template -n nerdstore

# Desinstalar o api-identity via Helm Chart
helm uninstall api-identity -n nerdstore

# Testar a conectividade de rede
kubectl run test-network --image=nicolaka/netshoot -i --tty --rm

# Testar o endpoint de saúde do api-identity
curl -k https://api-identity.nerdstore/healthz
curl -k https://api-identity.nerdstore/healthz-infra