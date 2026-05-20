#!/bin/bash

# Adicionar entrada no arquivo hosts para o domínio nerdstore.io
sudo nano /etc/hosts
192.168.58.2 nerdstore.io

# Configurar o túnel do Minikube para expor aplicação web 
minikube tunnel -p nerdstore-enterprise

# Instalar o web-mvc via Helm Chart
helm install --debug web-mvc -f ./web-mvc/values.yaml ./api-template -n nerdstore

# Testar a conectividade de rede
kubectl run test-network --image=nicolaka/netshoot -i --tty --rm

# Testar o endpoint de saúde do web-mvc
curl -k https://web-mvc.nerdstore/healthz
