#!/bin/bash

# Criando Secret
kubectl create secret generic rabbit-connection -n nerdstore --from-literal=MessageBroker__Provider="RabbitMq" --from-literal=MessageBroker__ConnectionString="host=rabbitmq.infra.svc.cluster.local:5672;publisherConfirms=true;timeout=30;username=nerdstore;password=nerdstore" -o yaml --dry-run=client > ./rabbitmq/secret.yaml

# Aplicar os manifests do RabbitMQ no cluster Kubernetes
kubectl apply -f ./rabbitmq -R 

# PostgreSQL via Helm Chart
helm repo add cetic https://cetic.github.io/helm-charts

helm install postgresql cetic/postgresql -n infra