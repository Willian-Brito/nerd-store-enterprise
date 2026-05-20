#!/bin/bash

# Gerar yaml da secret de conexão com o banco de dados PostgreSQL
kubectl create secret generic customer-connection -n nerdstore --from-literal=CUSTOMCONNSTR_DefaultConnection="Server=postgresql.infra.svc.cluster.local;Port=5432;Database=DBCustomer;User Id=postgres;Password=postgres;" -o yaml --dry-run=client > ./api-customer/customer-connection-secret.yaml

# Aplicar o secret da api-customer
kubectl apply -f ./api-customer/customer-connection-secret.yaml -n nerdstore

# Instalar o api-customer via Helm Chart
helm install --debug api-customer -f api-common.yaml -f ./api-customer/values.yaml ./api-template -n nerdstore

# Testar a conectividade de rede
kubectl run test-network --image=nicolaka/netshoot -i --tty --rm

# Testar o endpoint de saúde do api-customer
curl -k https://api-customer.nerdstore/healthz
curl -k https://api-customer.nerdstore/healthz-infra
