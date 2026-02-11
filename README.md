# Service Management Guide

This project uses **RabbitMQ** (Message Broker) and **Redis** (Cache/Store). Below are the commands to manage these services using either Docker Compose or standalone Docker commands.

---

## 🚀 Using Docker Compose (Recommended)
Docker Compose is the easiest way to manage both services at once using your `docker-compose.yml` file.

### Basic Lifecycle
* **Start services (background):** `docker compose up -d`
* **Stop services:** `docker compose stop`
* **Stop and remove containers:** `docker compose down`
* **Wipe all data and containers:** `docker compose down -v`

### Monitoring
* **Check status/health:** `docker compose ps`
* **View live logs:** `docker compose logs -f`

---

## 🐳 Using Pure Docker Commands
Use these if you want to run or troubleshoot containers individually.

### 1. Network Setup
Create a network so containers can talk to each other:
`docker network create app-network`

### 2. Start RabbitMQ
`docker run -d --name rabbitmq --network app-network -p 5672:5672 -p 15672:15672 rabbitmq:3-management`

### 3. Start Redis
`docker run -d --name redis --network app-network -p 6379:6379 redis:latest redis-server --appendonly yes`

### 4. Container Management
* **List running containers:** `docker ps`
* **Stop a container:** `docker stop <name>`
* **Start a container:** `docker start <name>`
* **Remove a container:** `docker rm -f <name>`

---

## 🛠 Troubleshooting Connection Issues
If you see the error: `open //./pipe/dockerDesktopLinuxEngine: The system cannot find the file specified`:

1. **Check Status:** Ensure the Docker Desktop app is open and the whale icon is green.
2. **Restart Engine:** Right-click the Docker icon in the system tray and select **Restart Docker Desktop**.
3. **Verify Connection:** Run `docker info` in your terminal. If it returns text, the engine is fixed.

---

## 🔗 Access Points
| Service | Local Address | Credentials |
| :--- | :--- | :--- |
| **RabbitMQ UI** | http://localhost:15672 | `guest` / `guest` |
| **RabbitMQ Port** | localhost:5672 | - |
| **Redis Port** | localhost:6379 | No password (default) |