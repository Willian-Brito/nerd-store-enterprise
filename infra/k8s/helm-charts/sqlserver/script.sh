#!/bin/bash

# Aplicar os manifests do SQL-Server no cluster Kubernetes
kubectl apply -f ./sqlserver -R 