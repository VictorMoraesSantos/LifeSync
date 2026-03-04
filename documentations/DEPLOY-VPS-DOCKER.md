# Deploy do LifeSync em VPS com Docker e Docker Compose

Guia completo e detalhado para fazer deploy de todos os microservicos do LifeSync em uma VPS usando Docker e Docker Compose.

---

## Sumario

1. [Visao Geral da Arquitetura](#1-visao-geral-da-arquitetura)
2. [Pre-requisitos](#2-pre-requisitos)
3. [Preparando a VPS](#3-preparando-a-vps)
4. [Instalando Docker e Docker Compose](#4-instalando-docker-e-docker-compose)
5. [Configurando o Firewall](#5-configurando-o-firewall)
6. [Clonando o Repositorio](#6-clonando-o-repositorio)
7. [Configurando Variaveis de Ambiente para Producao](#7-configurando-variaveis-de-ambiente-para-producao)
8. [Criando o docker-compose.production.yml](#8-criando-o-docker-composeproductionyml)
9. [Build e Deploy](#9-build-e-deploy)
10. [Configurando Nginx como Reverse Proxy com SSL](#10-configurando-nginx-como-reverse-proxy-com-ssl)
11. [Monitoramento e Logs](#11-monitoramento-e-logs)
12. [Backup do Banco de Dados](#12-backup-do-banco-de-dados)
13. [Atualizando a Aplicacao (CI/CD Basico)](#13-atualizando-a-aplicacao-cicd-basico)
14. [Troubleshooting](#14-troubleshooting)
15. [Checklist Final de Seguranca](#15-checklist-final-de-seguranca)

---

## 1. Visao Geral da Arquitetura

O LifeSync e composto pelos seguintes servicos:

### Microservicos (APIs .NET 10)
| Servico | Descricao | Porta Interna |
|---------|-----------|---------------|
| `taskmanager.api` | Gerenciamento de tarefas | 8080 |
| `users.api` | Autenticacao e usuarios (JWT + Identity) | 8080 |
| `nutrition.api` | Rastreamento nutricional | 8080 |
| `financial.api` | Gestao financeira | 8080 |
| `gym.api` | Gerenciamento de treinos | 8080 |
| `notification.api` | Servico de emails (event-driven via RabbitMQ) | 8080 |

### Infraestrutura
| Servico | Descricao | Porta |
|---------|-----------|-------|
| `lifesyncdb` | PostgreSQL 18 | 5432 |
| `rabbitmq` | Message Broker | 5672 / 15672 |
| `mailhog` | SMTP de desenvolvimento (trocar em producao) | 1025 / 8025 |

### Gateway e Frontend
| Servico | Descricao | Porta Exposta |
|---------|-----------|---------------|
| `yarpapigateway` | API Gateway (YARP) - ponto unico de entrada | 6006 |
| `lifesyncapp.webapp` | Frontend Blazor | 6007 |

### Diagrama de Comunicacao

```
Internet
   |
   v
[Nginx (80/443)] ---> [YARP API Gateway (:6006)] ---> [microservicos (:8080)]
                  |                                         |
                  |---> [Blazor WebApp (:6007)]             |---> [PostgreSQL (:5432)]
                                                            |---> [RabbitMQ (:5672)]
```

---

## 2. Pre-requisitos

- **VPS** com minimo de **4GB RAM** e **2 vCPUs** (recomendado para rodar todos os microservicos)
- **Sistema operacional**: Ubuntu 22.04 ou 24.04 LTS
- **Dominio** (opcional mas recomendado) apontando para o IP da VPS
- **Acesso SSH** a VPS
- **Repositorio Git** acessivel (GitHub, GitLab, etc.)

### Requisitos de Armazenamento
- ~5GB para imagens Docker
- ~2GB para o banco de dados (cresce com uso)
- ~1GB para logs e volumes
- **Total recomendado: 20GB+ de disco**

---

## 3. Preparando a VPS

### 3.1. Conectar via SSH

```bash
ssh root@SEU_IP_DA_VPS
```

### 3.2. Atualizar o sistema

```bash
apt update && apt upgrade -y
```

### 3.3. Criar usuario nao-root (seguranca)

```bash
adduser deploy
usermod -aG sudo deploy
su - deploy
```

### 3.4. Configurar SSH com chave (recomendado)

Na sua **maquina local**:
```bash
ssh-keygen -t ed25519 -C "deploy@lifesync"
ssh-copy-id deploy@SEU_IP_DA_VPS
```

Depois, desabilitar login por senha (opcional, aumenta seguranca):
```bash
sudo nano /etc/ssh/sshd_config
# Alterar: PasswordAuthentication no
sudo systemctl restart sshd
```

---

## 4. Instalando Docker e Docker Compose

### 4.1. Instalar Docker Engine

```bash
# Remover versoes antigas
sudo apt remove docker docker-engine docker.io containerd runc 2>/dev/null

# Instalar dependencias
sudo apt install -y ca-certificates curl gnupg lsb-release

# Adicionar chave GPG oficial do Docker
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg

# Adicionar repositorio
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Instalar Docker
sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```

### 4.2. Configurar Docker sem sudo

```bash
sudo usermod -aG docker deploy
newgrp docker
```

### 4.3. Verificar instalacao

```bash
docker --version
docker compose version
```

### 4.4. Habilitar Docker no boot

```bash
sudo systemctl enable docker
sudo systemctl start docker
```

---

## 5. Configurando o Firewall

```bash
# Instalar UFW (se nao estiver instalado)
sudo apt install -y ufw

# Regras basicas
sudo ufw default deny incoming
sudo ufw default allow outgoing

# SSH (IMPORTANTE: nao bloquear antes de permitir SSH!)
sudo ufw allow 22/tcp

# HTTP e HTTPS (para Nginx)
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Ativar firewall
sudo ufw enable
sudo ufw status
```

> **IMPORTANTE**: NAO exponha portas dos microservicos diretamente (5432, 5672, 8080, etc.). Tudo deve passar pelo Nginx.

---

## 6. Clonando o Repositorio

### 6.1. Instalar Git

```bash
sudo apt install -y git
```

### 6.2. Clonar o repositorio

```bash
mkdir -p ~/apps
cd ~/apps
git clone https://github.com/SEU_USUARIO/LifeSync.git
cd LifeSync
```

> Se o repositorio for privado, configure uma **deploy key** ou **personal access token**.

### Deploy Key (recomendado para repositorios privados)

```bash
# Gerar chave na VPS
ssh-keygen -t ed25519 -C "deploy-key-lifesync" -f ~/.ssh/lifesync_deploy

# Exibir chave publica (adicionar no GitHub/GitLab como Deploy Key)
cat ~/.ssh/lifesync_deploy.pub

# Configurar SSH para usar essa chave
cat >> ~/.ssh/config << 'EOF'
Host github.com
  HostName github.com
  User git
  IdentityFile ~/.ssh/lifesync_deploy
EOF
```

---

## 7. Configurando Variaveis de Ambiente para Producao

### 7.1. Criar arquivo `.env`

Crie um arquivo `.env` na raiz do projeto. Este arquivo **nunca deve ser commitado** no repositorio.

```bash
nano ~/apps/LifeSync/.env
```

Conteudo:

```env
# === BANCO DE DADOS ===
POSTGRES_USER=lifesync_admin
POSTGRES_PASSWORD=SUA_SENHA_SEGURA_AQUI_123!@#
POSTGRES_DB=LifeSyncDB

# === RABBITMQ ===
RABBITMQ_USER=lifesync_mq
RABBITMQ_PASSWORD=SUA_SENHA_MQ_AQUI_456!@#

# === JWT ===
JWT_KEY=SUA_CHAVE_JWT_SUPER_SECRETA_COM_PELO_MENOS_32_CARACTERES!@#$
JWT_ISSUER=LifeSyncAPI
JWT_AUDIENCE=LifeSyncApp
JWT_EXPIRY_MINUTES=60

# === SMTP (Producao - use um servico real) ===
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_ENABLE_SSL=true
SMTP_USER=seu-email@gmail.com
SMTP_PASSWORD=sua-app-password
SMTP_FROM=noreply@lifesync.com

# === AMBIENTE ===
ASPNETCORE_ENVIRONMENT=Production

# === URLs ===
BASE_API_URL=http://yarpapigateway:8080
```

### 7.2. Proteger o arquivo .env

```bash
chmod 600 ~/apps/LifeSync/.env
```

### 7.3. Adicionar .env ao .gitignore (se ainda nao estiver)

```bash
echo ".env" >> .gitignore
```

---

## 8. Criando o docker-compose.production.yml

Crie um arquivo de override para producao que substitui as configuracoes de desenvolvimento:

```bash
nano ~/apps/LifeSync/docker-compose.production.yml
```

```yaml
services:
  # ========================
  #  INFRAESTRUTURA
  # ========================
  lifesyncdb:
    container_name: lifesyncdb
    image: postgres:18
    environment:
      - POSTGRES_USER=${POSTGRES_USER}
      - POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
      - POSTGRES_DB=${POSTGRES_DB}
    restart: always
    # NAO exponha a porta em producao! Apenas rede interna.
    # ports:
    #   - "5433:5432"
    volumes:
      - postgres_lifesync:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER}"]
      interval: 10s
      timeout: 5s
      retries: 5
    deploy:
      resources:
        limits:
          memory: 512M

  rabbitmq:
    container_name: rabbitmq
    hostname: lifesync-mq
    environment:
      - RABBITMQ_DEFAULT_USER=${RABBITMQ_USER}
      - RABBITMQ_DEFAULT_PASS=${RABBITMQ_PASSWORD}
    restart: always
    # NAO exponha as portas em producao!
    # ports:
    #   - "5672:5672"
    #   - "15672:15672"
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5
    deploy:
      resources:
        limits:
          memory: 256M

  # ========================
  #  MICROSERVICOS
  # ========================
  taskmanager.api:
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__DefaultConnection=Server=lifesyncdb;Port=5432;Database=${POSTGRES_DB};User Id=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};
      - RabbitMQSettings__Host=rabbitmq
      - RabbitMQSettings__Port=5672
      - RabbitMQSettings__User=${RABBITMQ_USER}
      - RabbitMQSettings__Password=${RABBITMQ_PASSWORD}
    depends_on:
      lifesyncdb:
        condition: service_healthy
      rabbitmq:
        condition: service_healthy
    restart: always
    deploy:
      resources:
        limits:
          memory: 256M

  nutrition.api:
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__Database=Server=lifesyncdb;Port=5432;Database=${POSTGRES_DB};User Id=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};
    depends_on:
      lifesyncdb:
        condition: service_healthy
    restart: always
    deploy:
      resources:
        limits:
          memory: 256M

  financial.api:
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__Database=Server=lifesyncdb;Port=5432;Database=${POSTGRES_DB};User Id=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};
    depends_on:
      lifesyncdb:
        condition: service_healthy
    restart: always
    deploy:
      resources:
        limits:
          memory: 256M

  users.api:
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__Database=Server=lifesyncdb;Port=5432;Database=${POSTGRES_DB};User Id=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};
      - RabbitMQSettings__Host=rabbitmq
      - RabbitMQSettings__Port=5672
      - RabbitMQSettings__User=${RABBITMQ_USER}
      - RabbitMQSettings__Password=${RABBITMQ_PASSWORD}
      - JwtSettings__Key=${JWT_KEY}
      - JwtSettings__Issuer=${JWT_ISSUER}
      - JwtSettings__Audience=${JWT_AUDIENCE}
      - JwtSettings__ExpiryMinutes=${JWT_EXPIRY_MINUTES}
    depends_on:
      lifesyncdb:
        condition: service_healthy
      rabbitmq:
        condition: service_healthy
    restart: always
    deploy:
      resources:
        limits:
          memory: 256M

  gym.api:
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__Database=Server=lifesyncdb;Port=5432;Database=${POSTGRES_DB};User Id=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};
    depends_on:
      lifesyncdb:
        condition: service_healthy
    restart: always
    deploy:
      resources:
        limits:
          memory: 256M

  notification.api:
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__Database=Server=lifesyncdb;Port=5432;Database=${POSTGRES_DB};User Id=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};
      - RabbitMQSettings__Host=rabbitmq
      - RabbitMQSettings__Port=5672
      - RabbitMQSettings__User=${RABBITMQ_USER}
      - RabbitMQSettings__Password=${RABBITMQ_PASSWORD}
      - SmtpSettings__Host=${SMTP_HOST}
      - SmtpSettings__Port=${SMTP_PORT}
      - SmtpSettings__EnableSsl=${SMTP_ENABLE_SSL}
      - SmtpSettings__User=${SMTP_USER}
      - SmtpSettings__Password=${SMTP_PASSWORD}
      - SmtpSettings__From=${SMTP_FROM}
    depends_on:
      lifesyncdb:
        condition: service_healthy
      rabbitmq:
        condition: service_healthy
    restart: always
    deploy:
      resources:
        limits:
          memory: 256M

  # ========================
  #  GATEWAY E FRONTEND
  # ========================
  yarpapigateway:
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}
      - ASPNETCORE_URLS=http://+:8080
      - JwtSettings__Key=${JWT_KEY}
      - JwtSettings__Issuer=${JWT_ISSUER}
      - JwtSettings__Audience=${JWT_AUDIENCE}
    depends_on:
      - taskmanager.api
      - nutrition.api
      - financial.api
      - users.api
      - gym.api
      - notification.api
    ports:
      - "127.0.0.1:6006:8080"
    volumes:
      - yarpapigateway_dataprotection_keys:/keys
    restart: always
    deploy:
      resources:
        limits:
          memory: 256M

  lifesyncapp.webapp:
    environment:
      - ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}
      - ASPNETCORE_URLS=http://+:8080
      - BaseApiUrl=${BASE_API_URL}
    depends_on:
      - yarpapigateway
    ports:
      - "127.0.0.1:6007:8080"
    restart: always
    deploy:
      resources:
        limits:
          memory: 256M

volumes:
  postgres_lifesync:
  pgadmin_data:
  yarpapigateway_dataprotection_keys:
```

### Diferencas importantes em relacao ao ambiente de desenvolvimento

| Aspecto | Desenvolvimento | Producao |
|---------|----------------|----------|
| Portas do DB/RabbitMQ | Expostas ao host | Apenas rede interna Docker |
| Gateway/WebApp | Bind em `0.0.0.0` | Bind em `127.0.0.1` (so via Nginx) |
| Senhas | Hardcoded | Via `.env` |
| SMTP | MailHog (fake) | Servico real (Gmail, SendGrid, etc.) |
| Restart policy | `on-failure` | `always` |
| Memory limits | Sem limite | Definidos por servico |
| `Include Error Detail` | `true` | Removido (seguranca) |

---

## 9. Build e Deploy

### 9.1. Build das imagens

```bash
cd ~/apps/LifeSync

# Build de todas as imagens
docker compose -f docker-compose.yml -f docker-compose.production.yml build
```

> O primeiro build pode demorar 5-15 minutos dependendo da VPS. Builds subsequentes sao mais rapidos graças ao cache.

### 9.2. Subir todos os servicos

```bash
docker compose -f docker-compose.yml -f docker-compose.production.yml --env-file .env up -d
```

### 9.3. Verificar se todos os containers estao rodando

```bash
# Listar todos os containers
docker compose -f docker-compose.yml -f docker-compose.production.yml ps

# Verificar logs de um servico especifico
docker compose -f docker-compose.yml -f docker-compose.production.yml logs -f yarpapigateway

# Verificar logs de todos
docker compose -f docker-compose.yml -f docker-compose.production.yml logs -f --tail=50
```

### 9.4. Testar se a API esta respondendo

```bash
# Testar o gateway
curl http://localhost:6006/auth/health

# Testar o webapp
curl http://localhost:6007
```

### 9.5. Criar alias para facilitar (opcional)

Adicione ao `~/.bashrc`:

```bash
echo 'alias dc="docker compose -f docker-compose.yml -f docker-compose.production.yml --env-file .env"' >> ~/.bashrc
source ~/.bashrc

# Agora voce pode usar:
# dc up -d
# dc down
# dc logs -f
# dc ps
```

---

## 10. Configurando Nginx como Reverse Proxy com SSL

### 10.1. Instalar Nginx

```bash
sudo apt install -y nginx
```

### 10.2. Instalar Certbot (Let's Encrypt)

```bash
sudo apt install -y certbot python3-certbot-nginx
```

### 10.3. Configurar Nginx

```bash
sudo nano /etc/nginx/sites-available/lifesync
```

Conteudo (substitua `seu-dominio.com` pelo seu dominio real):

```nginx
# Redirecionar HTTP para HTTPS
server {
    listen 80;
    server_name seu-dominio.com api.seu-dominio.com;

    location /.well-known/acme-challenge/ {
        root /var/www/html;
    }

    location / {
        return 301 https://$host$request_uri;
    }
}

# API Gateway (api.seu-dominio.com)
server {
    listen 443 ssl http2;
    server_name api.seu-dominio.com;

    # SSL sera configurado pelo Certbot
    # ssl_certificate /etc/letsencrypt/live/api.seu-dominio.com/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/api.seu-dominio.com/privkey.pem;

    # Configuracoes de seguranca
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Referrer-Policy "strict-origin-when-cross-origin" always;

    # Limites
    client_max_body_size 10M;

    location / {
        proxy_pass http://127.0.0.1:6006;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_buffering off;
        proxy_read_timeout 90s;
    }
}

# Frontend (seu-dominio.com)
server {
    listen 443 ssl http2;
    server_name seu-dominio.com;

    # SSL sera configurado pelo Certbot
    # ssl_certificate /etc/letsencrypt/live/seu-dominio.com/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/seu-dominio.com/privkey.pem;

    # Configuracoes de seguranca
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;

    client_max_body_size 10M;

    location / {
        proxy_pass http://127.0.0.1:6007;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_buffering off;
    }
}
```

### 10.4. Ativar a configuracao

```bash
# Criar link simbolico
sudo ln -s /etc/nginx/sites-available/lifesync /etc/nginx/sites-enabled/

# Remover configuracao default
sudo rm /etc/nginx/sites-enabled/default

# Testar configuracao
sudo nginx -t

# Recarregar Nginx
sudo systemctl reload nginx
```

### 10.5. Gerar certificados SSL (Let's Encrypt)

```bash
sudo certbot --nginx -d seu-dominio.com -d api.seu-dominio.com
```

O Certbot vai:
1. Gerar os certificados automaticamente
2. Configurar o Nginx com SSL
3. Configurar renovacao automatica

### 10.6. Verificar renovacao automatica

```bash
# Testar renovacao
sudo certbot renew --dry-run

# O timer do systemd renova automaticamente
sudo systemctl status certbot.timer
```

---

## 11. Monitoramento e Logs

### 11.1. Verificar status dos containers

```bash
# Status de todos os servicos
docker compose -f docker-compose.yml -f docker-compose.production.yml ps

# Uso de recursos por container
docker stats --no-stream
```

### 11.2. Configurar rotacao de logs do Docker

Crie/edite o arquivo de configuracao do Docker:

```bash
sudo nano /etc/docker/daemon.json
```

```json
{
  "log-driver": "json-file",
  "log-opts": {
    "max-size": "10m",
    "max-file": "3"
  }
}
```

```bash
sudo systemctl restart docker
```

### 11.3. Script de health check

Crie um script para verificar a saude dos servicos:

```bash
nano ~/apps/lifesync-health.sh
```

```bash
#!/bin/bash

echo "=== LifeSync Health Check ==="
echo "Data: $(date)"
echo ""

# Verificar containers
echo "--- Containers ---"
docker compose -f ~/apps/LifeSync/docker-compose.yml \
  -f ~/apps/LifeSync/docker-compose.production.yml ps --format "table {{.Name}}\t{{.Status}}"

echo ""

# Verificar API Gateway
echo "--- API Gateway ---"
HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:6006/ 2>/dev/null)
if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "404" ]; then
    echo "Gateway: OK (HTTP $HTTP_CODE)"
else
    echo "Gateway: FALHA (HTTP $HTTP_CODE)"
fi

# Verificar WebApp
HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:6007/ 2>/dev/null)
if [ "$HTTP_CODE" = "200" ]; then
    echo "WebApp:  OK (HTTP $HTTP_CODE)"
else
    echo "WebApp:  FALHA (HTTP $HTTP_CODE)"
fi

echo ""

# Uso de disco
echo "--- Disco ---"
df -h / | tail -1
echo ""

# Uso de memoria
echo "--- Memoria ---"
free -h | head -2
echo ""

# Docker stats resumido
echo "--- Docker Resources ---"
docker stats --no-stream --format "table {{.Name}}\t{{.CPUPerc}}\t{{.MemUsage}}" 2>/dev/null
```

```bash
chmod +x ~/apps/lifesync-health.sh
```

### 11.4. Cron job para health check (opcional)

```bash
# Executar health check a cada 5 minutos e salvar log
crontab -e
```

Adicionar:
```
*/5 * * * * ~/apps/lifesync-health.sh >> ~/apps/health.log 2>&1
```

---

## 12. Backup do Banco de Dados

### 12.1. Backup manual

```bash
# Criar pasta de backups
mkdir -p ~/backups

# Fazer dump do banco
docker exec lifesyncdb pg_dump -U lifesync_admin -d LifeSyncDB -F c -f /tmp/backup.dump
docker cp lifesyncdb:/tmp/backup.dump ~/backups/lifesync_$(date +%Y%m%d_%H%M%S).dump
```

### 12.2. Script de backup automatico

```bash
nano ~/apps/lifesync-backup.sh
```

```bash
#!/bin/bash

BACKUP_DIR=~/backups
RETENTION_DAYS=7
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="${BACKUP_DIR}/lifesync_${TIMESTAMP}.dump"

# Criar diretorio se nao existir
mkdir -p "$BACKUP_DIR"

# Fazer backup
echo "[$(date)] Iniciando backup..."
docker exec lifesyncdb pg_dump -U lifesync_admin -d LifeSyncDB -F c -f /tmp/backup.dump

if [ $? -eq 0 ]; then
    docker cp lifesyncdb:/tmp/backup.dump "$BACKUP_FILE"
    docker exec lifesyncdb rm /tmp/backup.dump

    # Comprimir
    gzip "$BACKUP_FILE"
    echo "[$(date)] Backup salvo: ${BACKUP_FILE}.gz"

    # Remover backups antigos
    find "$BACKUP_DIR" -name "lifesync_*.dump.gz" -mtime +$RETENTION_DAYS -delete
    echo "[$(date)] Backups com mais de ${RETENTION_DAYS} dias removidos."
else
    echo "[$(date)] ERRO: Falha no backup!"
    exit 1
fi
```

```bash
chmod +x ~/apps/lifesync-backup.sh
```

### 12.3. Agendar backup diario

```bash
crontab -e
```

Adicionar (backup diario as 3h da manha):
```
0 3 * * * ~/apps/lifesync-backup.sh >> ~/backups/backup.log 2>&1
```

### 12.4. Restaurar backup

```bash
# Copiar backup para o container
docker cp ~/backups/lifesync_20260303_030000.dump lifesyncdb:/tmp/restore.dump

# Restaurar
docker exec lifesyncdb pg_restore -U lifesync_admin -d LifeSyncDB -c /tmp/restore.dump
```

---

## 13. Atualizando a Aplicacao (CI/CD Basico)

### 13.1. Script de deploy manual

```bash
nano ~/apps/deploy.sh
```

```bash
#!/bin/bash
set -e

APP_DIR=~/apps/LifeSync
COMPOSE_CMD="docker compose -f docker-compose.yml -f docker-compose.production.yml --env-file .env"

echo "=== Deploy LifeSync ==="
echo "Data: $(date)"
echo ""

cd "$APP_DIR"

# 1. Fazer backup do banco antes do deploy
echo "[1/5] Fazendo backup do banco..."
~/apps/lifesync-backup.sh

# 2. Puxar alteracoes do repositorio
echo "[2/5] Atualizando codigo..."
git pull origin main

# 3. Rebuild das imagens
echo "[3/5] Buildando imagens..."
$COMPOSE_CMD build

# 4. Reiniciar servicos (com zero-downtime parcial)
echo "[4/5] Reiniciando servicos..."
$COMPOSE_CMD up -d

# 5. Limpar imagens antigas
echo "[5/5] Limpando imagens nao utilizadas..."
docker image prune -f

echo ""
echo "=== Deploy concluido! ==="
$COMPOSE_CMD ps
```

```bash
chmod +x ~/apps/deploy.sh
```

### 13.2. Executar deploy

```bash
~/apps/deploy.sh
```

### 13.3. Rollback (se algo der errado)

```bash
cd ~/apps/LifeSync

# Voltar para o commit anterior
git log --oneline -5   # ver ultimos commits
git checkout <COMMIT_HASH_ANTERIOR>

# Rebuildar e reiniciar
docker compose -f docker-compose.yml -f docker-compose.production.yml --env-file .env build
docker compose -f docker-compose.yml -f docker-compose.production.yml --env-file .env up -d
```

### 13.4. GitHub Actions (CI/CD Automatizado - Opcional)

Se quiser automatizar deploys quando fizer push na branch `main`, crie o arquivo `.github/workflows/deploy.yml`:

```yaml
name: Deploy to VPS

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Deploy via SSH
        uses: appleboy/ssh-action@v1
        with:
          host: ${{ secrets.VPS_HOST }}
          username: ${{ secrets.VPS_USER }}
          key: ${{ secrets.VPS_SSH_KEY }}
          script: |
            cd ~/apps/LifeSync
            git pull origin main
            docker compose -f docker-compose.yml -f docker-compose.production.yml --env-file .env build
            docker compose -f docker-compose.yml -f docker-compose.production.yml --env-file .env up -d
            docker image prune -f
```

Configure os secrets no GitHub: `VPS_HOST`, `VPS_USER`, `VPS_SSH_KEY`.

---

## 14. Troubleshooting

### Container nao inicia

```bash
# Ver logs detalhados do container
docker compose -f docker-compose.yml -f docker-compose.production.yml logs <nome-do-servico>

# Exemplo:
docker compose -f docker-compose.yml -f docker-compose.production.yml logs users.api
```

### Banco de dados nao conecta

```bash
# Verificar se o container do postgres esta rodando
docker ps | grep lifesyncdb

# Testar conexao de dentro da rede Docker
docker exec -it lifesyncdb psql -U lifesync_admin -d LifeSyncDB -c "SELECT 1;"
```

### RabbitMQ nao conecta

```bash
# Verificar logs do RabbitMQ
docker logs rabbitmq

# Verificar se as filas foram criadas
docker exec rabbitmq rabbitmqctl list_queues
```

### Memoria insuficiente

```bash
# Verificar uso de memoria
docker stats --no-stream

# Se necessario, adicionar swap
sudo fallocate -l 2G /swapfile
sudo chmod 600 /swapfile
sudo mkswap /swapfile
sudo swapon /swapfile
echo '/swapfile none swap sw 0 0' | sudo tee -a /etc/fstab
```

### Reconstruir um servico especifico

```bash
docker compose -f docker-compose.yml -f docker-compose.production.yml build taskmanager.api
docker compose -f docker-compose.yml -f docker-compose.production.yml up -d taskmanager.api
```

### Limpar tudo e recomecar

```bash
# CUIDADO: remove containers, volumes e imagens
docker compose -f docker-compose.yml -f docker-compose.production.yml down -v
docker system prune -a --volumes

# Rebuildar tudo
docker compose -f docker-compose.yml -f docker-compose.production.yml --env-file .env up -d --build
```

### Verificar rede Docker

```bash
# Listar redes
docker network ls

# Inspecionar rede do compose
docker network inspect lifesync_default
```

---

## 15. Checklist Final de Seguranca

Antes de considerar o deploy como "pronto para producao", verifique:

### Credenciais e Segredos
- [ ] Senhas do PostgreSQL alteradas (nao usar `postgres/postgres`)
- [ ] Senhas do RabbitMQ alteradas (nao usar `guest/guest`)
- [ ] Chave JWT alterada para uma chave forte e unica
- [ ] Arquivo `.env` com permissao `600`
- [ ] `.env` adicionado ao `.gitignore`
- [ ] Nenhuma credencial hardcoded no codigo

### Rede e Firewall
- [ ] UFW ativado (apenas portas 22, 80, 443 abertas)
- [ ] Portas do PostgreSQL (5432) NAO expostas externamente
- [ ] Portas do RabbitMQ (5672, 15672) NAO expostas externamente
- [ ] Gateway e WebApp fazendo bind em `127.0.0.1` (apenas Nginx acessa)
- [ ] pgAdmin removido ou protegido com senha forte

### SSL/HTTPS
- [ ] Certificado SSL configurado (Let's Encrypt)
- [ ] Renovacao automatica do certificado funcionando
- [ ] Redirect de HTTP para HTTPS configurado

### Backup e Monitoramento
- [ ] Backup automatico do banco configurado
- [ ] Rotacao de logs do Docker configurada
- [ ] Health check script criado
- [ ] Cron jobs de backup e monitoramento ativos

### Sistema
- [ ] Sistema operacional atualizado
- [ ] Docker configurado para iniciar no boot
- [ ] Swap configurado (se VPS tem pouca RAM)
- [ ] Login SSH por senha desabilitado (usar chave)

---

## Comandos Rapidos de Referencia

```bash
# ===== OPERACOES DIARIAS =====

# Status de todos os servicos
dc ps

# Logs em tempo real (todos)
dc logs -f --tail=100

# Logs de um servico especifico
dc logs -f taskmanager.api

# Reiniciar um servico
dc restart users.api

# Parar tudo
dc down

# Subir tudo
dc up -d

# ===== MANUTENCAO =====

# Rebuild e restart de um servico
dc build taskmanager.api && dc up -d taskmanager.api

# Ver uso de recursos
docker stats --no-stream

# Limpar imagens nao utilizadas
docker image prune -f

# Backup manual
~/apps/lifesync-backup.sh

# Health check
~/apps/lifesync-health.sh

# Deploy completo
~/apps/deploy.sh
```

---

> **Nota**: Lembre-se de substituir `seu-dominio.com`, `SEU_IP_DA_VPS`, `SEU_USUARIO` e todas as credenciais de exemplo por valores reais antes de executar os comandos.
