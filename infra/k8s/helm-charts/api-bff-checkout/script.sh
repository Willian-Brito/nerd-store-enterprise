#!/bin/bash

# Instalar o api-bff-checkout via Helm Chart
helm install --debug api-bff-checkout -f api-common.yaml -f ./api-bff-checkout/values.yaml ./api-template -n nerdstore

# Testar a conectividade de rede
kubectl run test-network --image=nicolaka/netshoot -i --tty --rm

# Testar o endpoint de saúde do api-bff-checkout
curl -k https://api-bff-checkout.nerdstore/healthz
