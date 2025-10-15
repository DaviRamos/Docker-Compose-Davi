# Exemplos de Uso da API - NetRedisASide2

## 🔐 1. Autenticação

### Obter Token de Acesso (Admin)

```bash
curl -X POST 'http://localhost:8080/realms/NetRedisASide2/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'client_id=netredisaside2-api' \
  -d 'client_secret=your-generated-secret-here' \
  -d 'grant_type=password' \
  -d 'username=admin' \
  -d 'password=admin123'
```

**Resposta:**
```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCI...",
  "expires_in": 3600,
  "refresh_expires_in": 1800,
  "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI...",
  "token_type": "Bearer",
  "not-before-policy": 0,
  "session_state": "abc123...",
  "scope": "profile email"
}
```

### Salvar Token em Variável

```bash
export TOKEN=$(curl -s -X POST 'http://localhost:8080/realms/NetRedisASide2/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'client_id=netredisaside2-api' \
  -d 'client_secret=your-generated-secret-here' \
  -d 'grant_type=password' \
  -d 'username=admin' \
  -d 'password=admin123' | jq -r '.access_token')

echo $TOKEN
```

---

## 📚 2. API Assuntos

### Criar um Assunto

```bash
curl -X POST 'http://localhost:5000/api/assuntos' \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{
    "nome": "Desenvolvimento de Software",
    "descricao": "Assuntos relacionados ao desenvolvimento de aplicações"
  }'
```

**Resposta:**
```json
{
  "id": 1,
  "nome": "Desenvolvimento de Software",
  "descricao": "Assuntos relacionados ao desenvolvimento de aplicações",
  "dataCriacao": "2025-10-15T14:30:00Z",
  "dataAtualizacao": "2025-10-15T14:30:00Z"
}
```

### Listar Todos os Assuntos

```bash
curl -X GET 'http://localhost:5000/api/assuntos' \
  -H "Authorization: Bearer $TOKEN"
```

**Resposta:**
```json
[
  {
    "id": 1,
    "nome": "Desenvolvimento de Software",
    "descricao": "Assuntos relacionados ao desenvolvimento de aplicações",
    "dataCriacao": "2025-10-15T14:30:00Z",
    "dataAtualizacao": "2025-10-15T14:30:00Z"
  },
  {
    "id": 2,
    "nome": "Infraestrutura",
    "descricao": "Assuntos relacionados à infraestrutura de TI",
    "dataCriacao": "2025-10-15T14:35:00Z",
    "dataAtualizacao": "2025-10-15T14:35:00Z"
  }
]
```

### Buscar Assunto por ID

```bash
curl -X GET 'http://localhost:5000/api/assuntos/1' \
  -H "Authorization: Bearer $TOKEN"
```

### Atualizar um Assunto

```bash
curl -X PUT 'http://localhost:5000/api/assuntos/1' \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{
    "nome": "Desenvolvimento Web",
    "descricao": "Assuntos relacionados ao desenvolvimento web moderno"
  }'
```

### Excluir um Assunto

```bash
curl -X DELETE 'http://localhost:5000/api/assuntos/1' \
  -H "Authorization: Bearer $TOKEN"
```

---

## 🔄 3. API Movimentações

### Criar uma Movimentação

```bash
curl -X POST 'http://localhost:5000/api/movimentacoes' \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{
    "nome": "Entrada de Mercadorias",
    "descricao": "Registro de entrada de novos produtos no estoque"
  }'
```

### Listar Todas as Movimentações

```bash
curl -X GET 'http://localhost:5000/api/movimentacoes' \
  -H "Authorization: Bearer $TOKEN"
```

### Buscar Movimentação por ID

```bash
curl -X GET 'http://localhost:5000/api/movimentacoes/1' \
  -H "Authorization: Bearer $TOKEN"
```

### Atualizar uma Movimentação

```bash
curl -X PUT 'http://localhost:5000/api/movimentacoes/1' \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{
    "nome": "Saída de Mercadorias",
    "descricao": "Registro de saída de produtos vendidos"
  }'
```

### Excluir uma Movimentação

```bash
curl -X DELETE 'http://localhost:5000/api/movimentacoes/1' \
  -H "Authorization: Bearer $TOKEN"
```

---

## 📄 4. API Tipos de Documento

### Criar um Tipo de Documento

```bash
curl -X POST 'http://localhost:5000/api/tipos-documento' \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{
    "nome": "Nota Fiscal",
    "descricao": "Documento fiscal de venda de produtos"
  }'
```

### Listar Todos os Tipos de Documento

```bash
curl -X GET 'http://localhost:5000/api/tipos-documento' \
  -H "Authorization: Bearer $TOKEN"
```

### Buscar Tipo de Documento por ID

```bash
curl -X GET 'http://localhost:5000/api/tipos-documento/1' \
  -H "Authorization: Bearer $TOKEN"
```

### Atualizar um Tipo de Documento

```bash
curl -X PUT 'http://localhost:5000/api/tipos-documento/1' \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{
    "nome": "Nota Fiscal Eletrônica",
    "descricao": "Documento fiscal eletrônico de venda"
  }'
```

### Excluir um Tipo de Documento

```bash
curl -X DELETE 'http://localhost:5000/api/tipos-documento/1' \
  -H "Authorization: Bearer $TOKEN"
```

---

## 🧪 5. Script de Teste Completo

Crie um arquivo `test-api.sh`:

```bash
#!/bin/bash

echo "======================================"
echo "Testando API NetRedisASide2"
echo "======================================"

# 1. Obter Token
echo ""
echo "1. Obtendo token de autenticação..."
TOKEN=$(curl -s -X POST 'http://localhost:8080/realms/NetRedisASide2/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'client_id=netredisaside2-api' \
  -d 'client_secret=your-generated-secret-here' \
  -d 'grant_type=password' \
  -d 'username=admin' \
  -d 'password=admin123' | jq -r '.access_token')

if [ -z "$TOKEN" ] || [ "$TOKEN" = "null" ]; then
    echo "❌ Erro ao obter token!"
    exit 1
fi

echo "✓ Token obtido com sucesso"

# 2. Criar Assunto
echo ""
echo "2. Criando um assunto..."
ASSUNTO=$(curl -s -X POST 'http://localhost:5000/api/assuntos' \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{
    "nome": "Tecnologia",
    "descricao": "Assuntos relacionados à tecnologia"
  }')

ASSUNTO_ID=$(echo $ASSUNTO | jq -r '.id')
echo "✓ Assunto criado com ID: $ASSUNTO_ID"
echo $ASSUNTO | jq .

# 3. Listar Assuntos
echo ""
echo "3. Listando todos os assuntos..."
curl -s -X GET 'http://localhost:5000/api/assuntos' \
  -H "Authorization: Bearer $TOKEN" | jq .

# 4. Buscar Assunto por ID
echo ""
echo "4. Buscando assunto por ID..."
curl -s -X GET "http://localhost:5000/api/assuntos/$ASSUNTO_ID" \
  -H "Authorization: Bearer $TOKEN" | jq .

# 5. Atualizar Assunto
echo ""
echo "5. Atualizando assunto..."
curl -s -X PUT "http://localhost:5000/api/assuntos/$ASSUNTO_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{
    "nome": "Tecnologia e Inovação",
    "descricao": "Assuntos sobre tecnologia e inovação"
  }' | jq .

# 6. Criar Movimentação
echo ""
echo "6. Criando uma movimentação..."
MOVIMENTACAO=$(curl -s -X POST 'http://localhost:5000/api/movimentacoes' \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{
    "nome": "Entrada",
    "descricao": "Movimentação de entrada"
  }')

MOV_ID=$(echo $MOVIMENTACAO | jq -r '.id')
echo "✓ Movimentação criada com ID: $MOV_ID"

# 7. Criar Tipo de Documento
echo ""
echo "7. Criando um tipo de documento..."
TIPO_DOC=$(curl -s -X POST 'http://localhost:5000/api/tipos-documento' \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{
    "nome": "Contrato",
    "descricao": "Documento de contrato"
  }')

TIPO_DOC_ID=$(echo $TIPO_DOC | jq -r '.id')
echo "✓ Tipo de documento criado com ID: $TIPO_DOC_ID"

# 8. Verificar Cache Redis
echo ""
echo "8. Verificando cache Redis..."
docker exec netredisaside2-redis redis-cli KEYS "NetRedisASide2:*"

echo ""
echo "======================================"
echo "✓ Testes concluídos com sucesso!"
echo "======================================"
```

Execute:
```bash
chmod +x test-api.sh
./test-api.sh
```

---

## 🔍 6. Verificar Cache Redis

### Listar chaves em cache

```bash
docker exec netredisaside2-redis redis-cli KEYS "NetRedisASide2:*"
```

### Ver valor de uma chave específica

```bash
docker exec netredisaside2-redis redis-cli GET "NetRedisASide2:assunto:1"
```

### Limpar todo o cache

```bash
docker exec netredisaside2-redis redis-cli FLUSHALL
```

### Verificar TTL de uma chave

```bash
docker exec netredisaside2-redis redis-cli TTL "NetRedisASide2:assunto:all"
```

---

## 🗄️ 7. Comandos PostgreSQL

### Conectar ao database

```bash
docker exec -it netredisaside2-postgres psql -U netredisaside2_user -d netredisaside2_db
```

### Listar tabelas

```sql
\dt
```

### Ver dados de uma tabela

```sql
SELECT * FROM "Assuntos";
SELECT * FROM "Movimentacoes";
SELECT * FROM "TiposDocumento";
```

### Contar registros

```sql
SELECT COUNT(*) FROM "Assuntos";
```

---

## 🤖 8. Testar Ollama

### Listar modelos instalados

```bash
curl http://localhost:11434/api/tags
```

### Gerar texto com llama2

```bash
curl http://localhost:11434/api/generate -d '{
  "model": "llama2",
  "prompt": "Por que o céu é azul?"
}'
```

### Gerar embedding

```bash
curl http://localhost:11434/api/embeddings -d '{
  "model": "mxbai-embed-large",
  "prompt": "Representação vetorial desta frase"
}'
```

---

## 🔎 9. Testar Weaviate

### Verificar status

```bash
curl http://localhost:8081/v1/meta
```

### Criar schema

```bash
curl -X POST http://localhost:8081/v1/schema \
  -H 'Content-Type: application/json' \
  -d '{
    "class": "Document",
    "vectorizer": "text2vec-ollama",
    "moduleConfig": {
      "text2vec-ollama": {
        "apiEndpoint": "http://ollama:11434",
        "model": "mxbai-embed-large"
      }
    },
    "properties": [
      {
        "name": "title",
        "dataType": ["string"]
      },
      {
        "name": "content",
        "dataType": ["text"]
      }
    ]
  }'
```

### Inserir dados

```bash
curl -X POST http://localhost:8081/v1/objects \
  -H 'Content-Type: application/json' \
  -d '{
    "class": "Document",
    "properties": {
      "title": "API NetRedisASide2",
      "content": "Documentação completa da API desenvolvida em .NET"
    }
  }'
```

---

## 📊 10. Monitoramento

### Ver logs da API

```bash
cd src/NetRedisASide2.Api
dotnet run --verbosity detailed
```

### Ver logs dos containers

```bash
# Todos os serviços
docker-compose logs -f

# Serviço específico
docker-compose logs -f postgres
docker-compose logs -f keycloak
docker-compose logs -f ollama
```

### Verificar uso de recursos

```bash
docker stats
```

### Health checks

```bash
# PostgreSQL
docker exec netredisaside2-postgres pg_isready -U postgres

# Redis
docker exec netredisaside2-redis redis-cli ping

# Keycloak
curl http://localhost:8080/health/ready

# Ollama
curl http://localhost:11434/api/tags

# Weaviate
curl http://localhost:8081/v1/.well-known/ready
```

---

## 🐛 11. Troubleshooting

### API retorna 401 Unauthorized

```bash
# Verificar se o token é válido
echo $TOKEN

# Obter novo token
export TOKEN=$(curl -s -X POST 'http://localhost:8080/realms/NetRedisASide2/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'client_id=netredisaside2-api' \
  -d 'client_secret=your-generated-secret-here' \
  -d 'grant_type=password' \
  -d 'username=admin' \
  -d 'password=admin123' | jq -r '.access_token')
```

### Cache não está funcionando

```bash
# Verificar se Redis está rodando
docker ps | grep redis

# Testar conexão Redis
docker exec netredisaside2-redis redis-cli ping

# Ver logs da API
cd src/NetRedisASide2.Api
dotnet run
```

### Database não conecta

```bash
# Verificar se PostgreSQL está rodando
docker ps | grep postgres

# Testar conexão
docker exec -it netredisaside2-postgres psql -U postgres -c "SELECT version();"

# Verificar databases criados
docker exec -it netredisaside2-postgres psql -U postgres -c "\l"
```

---

## 📝 12. Swagger UI

Acesse a documentação interativa da API:

**URL**: http://localhost:5000/swagger

No Swagger UI você pode:
- Ver todos os endpoints disponíveis
- Testar as requisições diretamente
- Ver os modelos de dados
- Copiar exemplos de requisições

### Autorizar no Swagger

1. Clique em "Authorize"
2. Cole o token no formato: `Bearer YOUR_TOKEN`
3. Clique em "Authorize"
4. Agora você pode testar os endpoints

---

## ✅ Checklist de Testes

- [ ] Token obtido com sucesso
- [ ] Criar assunto funciona
- [ ] Listar assuntos funciona
- [ ] Buscar por ID funciona
- [ ] Atualizar assunto funciona
- [ ] Excluir assunto funciona
- [ ] Cache Redis está funcionando
- [ ] Mesmo fluxo para Movimentações
- [ ] Mesmo fluxo para Tipos de Documento
- [ ] Swagger UI acessível
- [ ] Ollama respondendo
- [ ] Weaviate funcionando
