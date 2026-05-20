#!/bin/bash

# Gerar yaml da secret de conexão com o banco de dados PostgreSQL
kubectl create secret generic payment-connection -n nerdstore --from-literal=CUSTOMCONNSTR_DefaultConnection="Server=postgresql.infra.svc.cluster.local;Port=5432;Database=DBpayment;User Id=postgres;Password=postgres;" -o yaml --dry-run=client > ./api-payment/payment-connection-secret.yaml

# Aplicar o secret da api-payment
kubectl apply -f ./api-payment/payment-connection-secret.yaml -n nerdstore

# Instalar o api-payment via Helm Chart
helm install --debug api-payment -f api-common.yaml -f ./api-payment/values.yaml ./api-template -n nerdstore

# Testar a conectividade de rede
kubectl run test-network --image=nicolaka/netshoot -i --tty --rm

# Testar o endpoint de saúde do api-payment
curl -k https://api-payment.nerdstore/healthz
curl -k https://api-payment.nerdstore/healthz-infra
