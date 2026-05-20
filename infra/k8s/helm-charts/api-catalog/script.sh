#!/bin/bash

# Gerar o yaml da secret de conexão com o banco de dados PostgreSQL
kubectl create secret generic catalog-connection -n nerdstore --from-literal=CUSTOMCONNSTR_DefaultConnection="Server=postgresql.infra.svc.cluster.local;Port=5432;Database=DBCatalog;User Id=postgres;Password=postgres;" -o yaml --dry-run=client > ./api-catalog/catalog-connection-secret.yaml

# Aplicar o secret da api-catalog
kubectl apply -f ./api-catalog/catalog-connection-secret.yaml -n nerdstore

# Instalar o api-catalog via Helm Chart
helm install --debug api-catalog -f api-common.yaml -f ./api-catalog/values.yaml ./api-template -n nerdstore

# Testar a conectividade de rede
kubectl run test-network --image=nicolaka/netshoot -i --tty --rm

# Testar o endpoint de saúde do api-catalog
curl -k https://api-catalog.nerdstore/healthz
curl -k https://api-catalog.nerdstore/healthz-infra
curl -k https://api-catalog.nerdstore/catalog/products