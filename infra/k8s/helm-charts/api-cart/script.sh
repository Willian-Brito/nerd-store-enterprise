#!/bin/bash

# Gerar yaml da secret de conexão com o banco de dados PostgreSQL
kubectl create secret generic cart-connection -n nerdstore --from-literal=CUSTOMCONNSTR_DefaultConnection="Server=postgresql.infra.svc.cluster.local;Port=5432;Database=DBCart;User Id=postgres;Password=postgres;" -o yaml --dry-run=client > ./api-cart/cart-connection-secret.yaml

# Aplicar o secret da api-cart
kubectl apply -f ./api-cart/cart-connection-secret.yaml -n nerdstore

# Instalar o api-cart via Helm Chart
helm install --debug api-cart -f api-common.yaml -f ./api-cart/values.yaml ./api-template -n nerdstore

# Testar a conectividade de rede
kubectl run test-network --image=nicolaka/netshoot -i --tty --rm

# Testar o endpoint de saúde do api-cart
curl -k https://api-cart.nerdstore/healthz
curl -k https://api-cart.nerdstore/healthz-infra
