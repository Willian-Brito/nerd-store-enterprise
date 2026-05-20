#!/bin/bash

# Adicionar entrada no arquivo hosts para o domínio status.nerdstore.io
sudo nano /etc/hosts
192.168.58.2 status.nerdstore.io

# Configurar o túnel do Minikube para expor aplicação web 
minikube tunnel -p nerdstore-enterprise

# Gerar yaml da secret de conexão com o banco de dados PostgreSQL
kubectl create secret generic status-connection -n nerdstore --from-literal=CUSTOMCONNSTR_DefaultConnection="Server=postgresql.infra.svc.cluster.local;Port=5432;Database=DBStatus;User Id=postgres;Password=postgres;" -o yaml --dry-run=client > ./web-status/status-connection-secret.yaml

# Aplicar o secret da web-status
kubectl apply -f ./web-status/status-connection-secret.yaml -n nerdstore

# Instalar o web-status via Helm Chart
helm install --debug web-status -f api-common.yaml -f ./web-status/values.yaml ./api-template -n nerdstore

# Testar a conectividade de rede
kubectl run test-network --image=nicolaka/netshoot -i --tty --rm

# Testar o endpoint de saúde do web-status
curl -k https://web-status.nerdstore/healthz
curl -k https://web-status.nerdstore/healthz-infra
