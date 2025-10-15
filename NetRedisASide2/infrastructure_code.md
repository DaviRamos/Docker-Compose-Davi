# Arquivos de Infraestrutura - NetRedisASide2

## 🐳 docker-compose.yml

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    container_name: netredisaside2-postgres
    environment:
      POSTGRES_USER: ${POSTGRES_ADMIN_USER}
      POSTGRES_PASSWORD: ${POSTGRES_ADMIN_PASSWORD}
      POSTGRES_DB: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - netredisaside2-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_ADMIN_USER}"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    container_name: netredisaside2-redis
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - netredisaside2-network
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  keycloak:
    image: quay.io/keycloak/keycloak:23.0
    container_name: netredisaside2-keycloak
    environment:
      KEYCLOAK_ADMIN: ${KEYCLOAK_ADMIN_USER}
      KEYCLOAK_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD}
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://postgres:5432/${KEYCLOAK_DB}
      KC_DB_USERNAME: ${KEYCLOAK_DB_USER}
      KC_DB_PASSWORD: ${KEYCLOAK_DB_PASSWORD}
      KC_HOSTNAME: localhost
      KC_HTTP_ENABLED: "true"
      KC_HOSTNAME_STRICT: "false"
      KC_HOSTNAME_STRICT_HTTPS: "false"
    command:
      - start-dev
      - --import-realm
    ports:
      - "8080:8080"
    volumes:
      - ./keycloak/realm-export.json:/opt/keycloak/data/import/realm-export.json
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - netredisaside2-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health/ready"]
      interval: 30s
      timeout: 10s
      retries: 5

  ollama:
    image: ollama/ollama:latest
    container_name: netredisaside2-ollama
    ports:
      - "11434:11434"
    volumes:
      - ollama_data:/root/.ollama
    networks:
      - netredisaside2-network
    deploy:
      resources:
        reservations:
          devices:
            - driver: nvidia
              count: all
              capabilities: [gpu]
    environment:
      - NVIDIA_VISIBLE_DEVICES=all
      - NVIDIA_DRIVER_CAPABILITIES=compute,utility
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:11434/api/tags"]
      interval: 30s
      timeout: 10s
      retries: 5

  ollama-setup:
    image: ollama/ollama:latest
    container_name: netredisaside2-ollama-setup
    depends_on:
      ollama:
        condition: service_healthy
    networks:
      - netredisaside2-network
    entrypoint: /bin/sh
    command: >
      -c "
      echo 'Aguardando Ollama estar pronto...';
      sleep 10;
      echo 'Baixando modelo llama2...';
      ollama pull llama2;
      echo 'Baixando modelo all-minilm...';
      ollama pull all-minilm;
      echo 'Baixando modelo mxbai-embed-large...';
      ollama pull mxbai-embed-large;
      echo 'Modelos baixados com sucesso!';
      "
    environment:
      - OLLAMA_HOST=ollama:11434

  weaviate:
    image: semitechnologies/weaviate:1.23.0
    container_name: netredisaside2-weaviate
    ports:
      - "8081:8080"
    environment:
      QUERY_DEFAULTS_LIMIT: 25
      AUTHENTICATION_ANONYMOUS_ACCESS_ENABLED: 'true'
      PERSISTENCE_DATA_PATH: '/var/lib/weaviate'
      DEFAULT_VECTORIZER_MODULE: 'text2vec-ollama'
      ENABLE_MODULES: 'text2vec-ollama,generative-ollama'
      CLUSTER_HOSTNAME: 'node1'
      OLLAMA_APIENDPOINT: 'http://ollama:11434'
    volumes:
      - weaviate_data:/var/lib/weaviate
    depends_on:
      ollama:
        condition: service_healthy
    networks:
      - netredisaside2-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/v1/.well-known/ready"]
      interval: 30s
      timeout: 10s
      retries: 5

networks:
  netredisaside2-network:
    driver: bridge

volumes:
  postgres_data:
  redis_data:
  ollama_data:
  weaviate_data:
```

---

## 🔐 .env

```bash
# PostgreSQL Admin
POSTGRES_ADMIN_USER=postgres
POSTGRES_ADMIN_PASSWORD=postgres_admin_pass_2024

# PostgreSQL - Database 1 (NetRedisASide2)
POSTGRES_DB1_NAME=netredisaside2_db
POSTGRES_DB1_USER=netredisaside2_user
POSTGRES_DB1_PASSWORD=netredisaside2_pass_2024

# PostgreSQL - Database 2 (Assuntos)
POSTGRES_DB2_NAME=assuntos_db
POSTGRES_DB2_USER=assuntos_user
POSTGRES_DB2_PASSWORD=assuntos_pass_2024

# PostgreSQL - Database 3 (Movimentacoes)
POSTGRES_DB3_NAME=movimentacoes_db
POSTGRES_DB3_USER=movimentacoes_user
POSTGRES_DB3_PASSWORD=movimentacoes_pass_2024

# PostgreSQL - Database 4 (TiposDocumento)
POSTGRES_DB4_NAME=tiposdocumento_db
POSTGRES_DB4_USER=tiposdocumento_user
POSTGRES_DB4_PASSWORD=tiposdocumento_pass_2024

# Keycloak Database
KEYCLOAK_DB=keycloak_db
KEYCLOAK_DB_USER=keycloak_user
KEYCLOAK_DB_PASSWORD=keycloak_pass_2024

# Keycloak Admin
KEYCLOAK_ADMIN_USER=admin
KEYCLOAK_ADMIN_PASSWORD=admin

# Application Settings
POSTGRES_HOST=localhost
POSTGRES_PORT=5432
POSTGRES_DB=netredisaside2_db
POSTGRES_USER=netredisaside2_user
POSTGRES_PASSWORD=netredisaside2_pass_2024

REDIS_HOST=localhost
REDIS_PORT=6379

KEYCLOAK_URL=http://localhost:8080
KEYCLOAK_REALM=NetRedisASide2
KEYCLOAK_CLIENT_ID=netredisaside2-api
KEYCLOAK_CLIENT_SECRET=your-generated-secret-here
```

---

## 📜 scripts/create-databases.sh

```bash
#!/bin/bash

# Script para criar múltiplos databases no PostgreSQL com credenciais individualizadas
# Autor: NetRedisASide2 Project
# Data: 2025

set -e

# Carregar variáveis do arquivo .env
if [ -f ../.env ]; then
    export $(cat ../.env | grep -v '^#' | xargs)
else
    echo "Arquivo .env não encontrado!"
    exit 1
fi

echo "=========================================="
echo "Criando Databases e Usuários no PostgreSQL"
echo "=========================================="

# Definir array de databases
declare -a DATABASES=(
    "${POSTGRES_DB1_NAME}:${POSTGRES_DB1_USER}:${POSTGRES_DB1_PASSWORD}"
    "${POSTGRES_DB2_NAME}:${POSTGRES_DB2_USER}:${POSTGRES_DB2_PASSWORD}"
    "${POSTGRES_DB3_NAME}:${POSTGRES_DB3_USER}:${POSTGRES_DB3_PASSWORD}"
    "${POSTGRES_DB4_NAME}:${POSTGRES_DB4_USER}:${POSTGRES_DB4_PASSWORD}"
    "${KEYCLOAK_DB}:${KEYCLOAK_DB_USER}:${KEYCLOAK_DB_PASSWORD}"
)

# Função para criar database e usuário
create_database() {
    local DB_NAME=$1
    local DB_USER=$2
    local DB_PASS=$3

    echo ""
    echo "Processando: ${DB_NAME}"
    echo "------------------------------------------"

    # Criar usuário se não existir
    echo "Criando usuário: ${DB_USER}"
    docker exec -i netredisaside2-postgres psql -U ${POSTGRES_ADMIN_USER} -c "
    DO \$\$
    BEGIN
        IF NOT EXISTS (SELECT FROM pg_catalog.pg_user WHERE usename = '${DB_USER}') THEN
            CREATE USER ${DB_USER} WITH PASSWORD '${DB_PASS}';
        END IF;
    END
    \$\$;
    " || echo "Usuário ${DB_USER} já existe ou erro ao criar."

    # Criar database se não existir
    echo "Criando database: ${DB_NAME}"
    docker exec -i netredisaside2-postgres psql -U ${POSTGRES_ADMIN_USER} -c "
    SELECT 'CREATE DATABASE ${DB_NAME} OWNER ${DB_USER}'
    WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '${DB_NAME}')\gexec
    " || echo "Database ${DB_NAME} já existe ou erro ao criar."

    # Conceder privilégios
    echo "Concedendo privilégios ao usuário ${DB_USER}"
    docker exec -i netredisaside2-postgres psql -U ${POSTGRES_ADMIN_USER} -d ${DB_NAME} -c "
    GRANT ALL PRIVILEGES ON DATABASE ${DB_NAME} TO ${DB_USER};
    GRANT ALL PRIVILEGES ON SCHEMA public TO ${DB_USER};
    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO ${DB_USER};
    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO ${DB_USER};
    "

    echo "✓ Database ${DB_NAME} configurado com sucesso!"
}

# Aguardar PostgreSQL estar pronto
echo "Aguardando PostgreSQL estar pronto..."
sleep 5

# Verificar se o container está rodando
if ! docker ps | grep -q netredisaside2-postgres; then
    echo "Erro: Container PostgreSQL não está rodando!"
    echo "Execute: docker-compose up -d postgres"
    exit 1
fi

# Processar cada database
for db_info in "${DATABASES[@]}"; do
    IFS=':' read -r DB_NAME DB_USER DB_PASS <<< "$db_info"
    create_database "$DB_NAME" "$DB_USER" "$DB_PASS"
done

echo ""
echo "=========================================="
echo "✓ Todos os databases foram criados!"
echo "=========================================="
echo ""
echo "Databases criados:"
for db_info in "${DATABASES[@]}"; do
    IFS=':' read -r DB_NAME DB_USER DB_PASS <<< "$db_info"
    echo "  - ${DB_NAME} (user: ${DB_USER})"
done

echo ""
echo "Para conectar a um database específico:"
echo "  psql -h localhost -U <username> -d <database>"
echo ""
echo "Exemplo:"
echo "  psql -h localhost -U ${POSTGRES_DB1_USER} -d ${POSTGRES_DB1_NAME}"
echo ""
```

---

## 🔑 keycloak/realm-export.json

```json
{
  "id": "netredisaside2",
  "realm": "NetRedisASide2",
  "displayName": "NetRedisASide2 Realm",
  "enabled": true,
  "sslRequired": "none",
  "registrationAllowed": false,
  "loginWithEmailAllowed": true,
  "duplicateEmailsAllowed": false,
  "resetPasswordAllowed": true,
  "editUsernameAllowed": false,
  "bruteForceProtected": true,
  "permanentLockout": false,
  "maxFailureWaitSeconds": 900,
  "minimumQuickLoginWaitSeconds": 60,
  "waitIncrementSeconds": 60,
  "quickLoginCheckMilliSeconds": 1000,
  "maxDeltaTimeSeconds": 43200,
  "failureFactor": 5,
  "roles": {
    "realm": [
      {
        "name": "user",
        "description": "Usuário padrão do sistema"
      },
      {
        "name": "admin",
        "description": "Administrador do sistema"
      },
      {
        "name": "assuntos_read",
        "description": "Permissão de leitura para Assuntos"
      },
      {
        "name": "assuntos_write",
        "description": "Permissão de escrita para Assuntos"
      },
      {
        "name": "movimentacoes_read",
        "description": "Permissão de leitura para Movimentações"
      },
      {
        "name": "movimentacoes_write",
        "description": "Permissão de escrita para Movimentações"
      },
      {
        "name": "tiposdocumento_read",
        "description": "Permissão de leitura para Tipos de Documento"
      },
      {
        "name": "tiposdocumento_write",
        "description": "Permissão de escrita para Tipos de Documento"
      }
    ]
  },
  "clients": [
    {
      "clientId": "netredisaside2-api",
      "name": "NetRedisASide2 API",
      "description": "Cliente para a API do NetRedisASide2",
      "enabled": true,
      "clientAuthenticatorType": "client-secret",
      "secret": "your-generated-secret-here",
      "redirectUris": [
        "http://localhost:5000/*",
        "https://localhost:5001/*"
      ],
      "webOrigins": [
        "http://localhost:5000",
        "https://localhost:5001"
      ],
      "publicClient": false,
      "protocol": "openid-connect",
      "attributes": {
        "access.token.lifespan": "3600",
        "client.secret.creation.time": "1704067200"
      },
      "directAccessGrantsEnabled": true,
      "serviceAccountsEnabled": true,
      "authorizationServicesEnabled": true,
      "standardFlowEnabled": true,
      "implicitFlowEnabled": false,
      "fullScopeAllowed": true
    },
    {
      "clientId": "netredisaside2-frontend",
      "name": "NetRedisASide2 Frontend",
      "description": "Cliente público para aplicações frontend",
      "enabled": true,
      "publicClient": true,
      "redirectUris": [
        "http://localhost:3000/*",
        "http://localhost:4200/*"
      ],
      "webOrigins": [
        "http://localhost:3000",
        "http://localhost:4200"
      ],
      "protocol": "openid-connect",
      "standardFlowEnabled": true,
      "implicitFlowEnabled": false,
      "directAccessGrantsEnabled": true,
      "fullScopeAllowed": true
    }
  ],
  "users": [
    {
      "username": "admin",
      "enabled": true,
      "emailVerified": true,
      "firstName": "Admin",
      "lastName": "User",
      "email": "admin@netredisaside2.com",
      "credentials": [
        {
          "type": "password",
          "value": "admin123",
          "temporary": false
        }
      ],
      "realmRoles": [
        "admin",
        "user",
        "assuntos_read",
        "assuntos_write",
        "movimentacoes_read",
        "movimentacoes_write",
        "tiposdocumento_read",
        "tiposdocumento_write"
      ]
    },
    {
      "username": "user",
      "enabled": true,
      "emailVerified": true,
      "firstName": "Regular",
      "lastName": "User",
      "email": "user@netredisaside2.com",
      "credentials": [
        {
          "type": "password",
          "value": "user123",
          "temporary": false
        }
      ],
      "realmRoles": [
        "user",
        "assuntos_read",
        "movimentacoes_read",
        "tiposdocumento_read"
      ]
    }
  ],
  "defaultRoles": [
    "user"
  ],
  "requiredCredentials": [
    "password"
  ],
  "passwordPolicy": "length(8) and notUsername and notEmail",
  "otpPolicyType": "totp",
  "otpPolicyAlgorithm": "HmacSHA1",
  "otpPolicyInitialCounter": 0,
  "otpPolicyDigits": 6,
  "otpPolicyLookAheadWindow": 1,
  "otpPolicyPeriod": 30,
  "smtpServer": {},
  "eventsEnabled": true,
  "eventsExpiration": 259200,
  "eventsListeners": [
    "jboss-logging"
  ],
  "enabledEventTypes": [
    "LOGIN",
    "LOGIN_ERROR",
    "REGISTER",
    "REGISTER_ERROR",
    "LOGOUT",
    "LOGOUT_ERROR"
  ],
  "adminEventsEnabled": true,
  "adminEventsDetailsEnabled": true,
  "internationalizationEnabled": true,
  "supportedLocales": [
    "pt-BR",
    "en"
  ],
  "defaultLocale": "pt-BR",
  "authenticationFlows": [],
  "authenticatorConfig": [],
  "requiredActions": [],
  "browserFlow": "browser",
  "registrationFlow": "registration",
  "directGrantFlow": "direct grant",
  "resetCredentialsFlow": "reset credentials",
  "clientAuthenticationFlow": "clients",
  "dockerAuthenticationFlow": "docker auth",
  "attributes": {
    "frontendUrl": "http://localhost:8080",
    "userProfileEnabled": "true"
  }
}
```

---

## 📋 Resumo dos Serviços

### PostgreSQL
- **Porta**: 5432
- **Admin**: postgres / postgres_admin_pass_2024
- **5 Databases**: netredisaside2_db, assuntos_db, movimentacoes_db, tiposdocumento_db, keycloak_db
- **Volume**: postgres_data

### Redis
- **Porta**: 6379
- **Sem autenticação** (desenvolvimento)
- **Volume**: redis_data

### Keycloak
- **Porta**: 8080
- **Admin**: admin / admin
- **Realm**: NetRedisASide2
- **Import automático**: realm-export.json
- **2 Clientes**: netredisaside2-api, netredisaside2-frontend

### Ollama
- **Porta**: 11434
- **GPU**: NVIDIA support
- **Modelos**: llama2, all-minilm, mxbai-embed-large
- **Volume**: ollama_data

### Weaviate
- **Porta**: 8081
- **Vectorizer**: text2vec-ollama
- **Generative**: generative-ollama
- **Volume**: weaviate_data

---

## 🚀 Comandos de Implantação

### Iniciar todos os serviços
```bash
cd docker
docker-compose up -d
```

### Ver status dos serviços
```bash
docker-compose ps
```

### Ver logs
```bash
docker-compose logs -f
```

### Parar serviços
```bash
docker-compose down
```

### Limpar tudo (⚠️ remove dados)
```bash
docker-compose down -v
```

### Criar databases
```bash
cd scripts
chmod +x create-databases.sh
./create-databases.sh
```
