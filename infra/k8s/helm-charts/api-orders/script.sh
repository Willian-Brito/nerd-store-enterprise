#!/bin/bash

# Gerar yaml da secret de conexão com o banco de dados PostgreSQL
kubectl create secret generic order-connection -n nerdstore --from-literal=CUSTOMCONNSTR_DefaultConnection="Server=sqlserver.infra.svc.cluster.local,1433;Database=DBOrder;User Id=sa;Password=MyDB@123;TrustServerCertificate=True;" -o yaml --dry-run=client > ./api-orders/order-connection-secret.yaml

# Aplicar o secret da api-order
kubectl apply -f ./api-orders/order-connection-secret.yaml -n nerdstore

# Instalar o api-orders via Helm Chart
helm install --debug api-orders -f api-common.yaml -f ./api-orders/values.yaml ./api-template -n nerdstore

# Testar a conectividade de rede
kubectl run test-network --image=nicolaka/netshoot -i --tty --rm

# Testar o endpoint de saúde do api-orders
curl -k https://api-orders.nerdstore/healthz
curl -k https://api-orders.nerdstore/healthz-infra
