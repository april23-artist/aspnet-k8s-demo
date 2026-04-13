# ASP.NET K8s Demo

![Build](https://img.shields.io/github/actions/workflow/status/username/repo/ci.yml)
![License](https://img.shields.io/github/license/username/repo)

## 簡介

使用 Multipass + K3s 搭建模擬 k8s 環境並且搭配 Github Action 、 Dockerhub、 Argo CD 進行自動化佈署流程。
以簡單模擬購票高併發場景，以 Api 接收交易請求並透過 Redis 實作簡單的訊息隊列，再由 Background worker 消化交易。

## 目錄

- [專案規劃](#專案規劃)

## 專案規劃

src
├─ Demo.Api
├─ Demo.Worker
└─ aspnet-k8s-demo.sln
docker
├─ api.Dockerfile
└─ worker.Dockerfile
deploy
└─ ticket-system // Helm chart
.github
└─ workflows/ci.yaml
docker-compose.yaml
test.js
