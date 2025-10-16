# 🚀 Guia de Uso - Coleções Postman NetRedisASide2

## 📥 Como Importar as Coleções

### 1. Importar a Coleção Principal

1. Abra o Postman
2. Clique em **Import** (botão no canto superior esquerdo)
3. Selecione a aba **Raw text**
4. Cole o conteúdo do arquivo `postman_collection.json`
5. Clique em **Import**

### 2. Importar o Environment

1. No Postman, clique no ícone de **⚙️ Configurações** (canto superior direito)
2. Selecione **Environments**
3. Clique em **Import**
4. Cole o conteúdo do arquivo `postman_environment.json`
5. Clique em **Import**

### 3. Ativar o Environment

1. No dropdown no canto superior direito (onde mostra "No Environment")
2. Selecione **NetRedisASide2 - Development**

---

## ⚙️ Configuração Inicial

### Obter o Client Secret do Keycloak

Antes de fazer as requisições, você precisa configurar o `keycloak_client_secret`:

```bash
# Acessar o container do Keycloak
docker exec -it netredisaside2-keycloak bash

# Entrar no console de administração do Keycloak
# OU acessar via browser: http://localhost:8080
# Login: admin / admin
```

**Via Interface Web:**
1. Acesse http://localhost:8080
2. Login: `admin` / `admin`
3. Selecione o realm **netredisaside2**
4. Vá em **Clients** → **netredisaside2-api**
5. Vá na aba **Credentials**
6. Copie o **Client Secret**

**Atualizar no Postman:**
1. No Postman, vá em **Environments**
2. Selecione **NetRedisASide2 - Development**
3. Edite a variável `keycloak_client_secret`
4. Cole o valor do Client Secret
5. Salve (Save)

---

## 🔐 Fluxo de Autenticação

### 1. Fazer Login

Escolha um dos usuários disponíveis:

#### **Admin** (Todas as permissões)
- Username: `admin`
- Password: `admin123`
- Permissões: Criar, Ler, Atualizar, Deletar

#### **Manager** (Leitura e Escrita)
- Username: `manager`
- Password: `manager123`
- Permissões: Criar, Ler, Atualizar

#### **User** (Somente Leitura)
- Username: `user`
- Password: `user123`
- Permissões: Ler apenas

### 2. Executar o Login

1. Vá para a pasta **🔐 Authentication**
2. Selecione uma das requisições de login (ex: **Login - Admin**)
3. Clique em **Send**
4. O token será **automaticamente salvo** nas variáveis de ambiente

### 3. Verificar o Token

Após o login bem-sucedido:
- ✅ A variável `access_token` será preenchida automaticamente
- ✅ A variável `refresh_token` também será salva
- ✅ Todas as outras requisições usarão esse token automaticamente

---

## 📝 Testando os Endpoints

### Ordem Recomendada de Testes

#### 1️⃣ **Assuntos** (pasta 📚)

```
1. Listar Todos os Assuntos (GET)
2. Criar Novo Assunto (POST) → Salva o ID automaticamente
3. Buscar Assunto por ID (GET) → Usa o ID salvo
4. Atualizar Assunto (PUT) → Usa o ID salvo
5. Deletar Assunto (DELETE) → Usa o ID salvo
```

#### 2️⃣ **Movimentações** (pasta 📄)

```
1. Listar Todas as Movimentações (GET)
2. Criar Nova Movimentação (POST) → Salva o ID automaticamente
3. Buscar Movimentação por ID (GET) → Usa o ID salvo
4. Atualizar Movimentação (PUT) → Usa o ID salvo
5. Deletar Movimentação (DELETE) → Usa o ID salvo
```

#### 3️⃣ **Tipos de Documento** (pasta 📋)

```
1. Listar Todos os Tipos de Documento (GET)
2. Criar Novo Tipo de Documento (POST) → Salva o ID automaticamente
3. Buscar Tipo de Documento por ID (GET) → Usa o ID salvo
4. Atualizar Tipo de Documento (PUT) → Usa o ID salvo
5. Deletar Tipo de Documento (DELETE) → Usa o ID salvo
```

---

## 🧪 Testes Automatizados

Cada requisição inclui **testes automatizados** que verificam:

✅ Status code correto (200, 201, 204, 404)  
✅ Estrutura da resposta  
✅ Presença de campos obrigatórios  
✅ Salvamento automático de IDs

### Ver Resultados dos Testes

Após executar uma requisição:
1. Clique na aba **Test Results** (abaixo da resposta)
2. Veja os testes que passaram (✅) ou falharam (❌)
3. No **Console** (View → Show Postman Console), veja logs detalhados

---

## 🔄 Renovar Token Expirado

Se o token expirar (erro 401):

1. Vá para **🔐 Authentication**
2. Execute **Refresh Token**
3. Um novo `access_token` será gerado automaticamente

---

## 📊 Collection Runner - Executar Todos os Testes

Para executar todos os testes de uma vez:

1. Clique com o botão direito na **coleção NetRedisASide2 API v2**
2. Selecione **Run collection**
3. Configure:
   - Iterations: 1
   - Delay: 500ms (entre requisições)
   - Data: Nenhum
4. Clique em **Run NetRedisASide2 API v2**

Isso executará TODOS os endpoints em sequência e mostrará um relatório completo!

---

## 🐛 Troubleshooting

### ❌ Erro 401 Unauthorized

**Causa:** Token não está configurado ou expirou

**Solução:**
1. Execute uma das requisições de **Login**
2. Verifique se o token foi salvo: vá em **Environments** e veja se `access_token` está preenchido
3. Se persistir, execute **Refresh Token**

### ❌ Erro 403 Forbidden

**Causa:** Usuário não tem permissão para a operação

**Solução:**
- Use o usuário **admin** para operações de DELETE
- Use **manager** ou **admin** para operações de POST/PUT
- Use qualquer usuário para operações de GET

### ❌ Erro 404 Not Found

**Causa:** Recurso não existe (ID inválido)

**Solução:**
1. Execute **Listar Todos** para ver os IDs disponíveis
2. Ou crie um novo recurso com **POST** primeiro

### ❌ Erro de Conexão

**Causa:** API ou Keycloak não estão rodando

**Solução:**
```bash
# Verificar se os containers estão rodando
docker ps

# Subir os containers
docker-compose up -d

# Verificar logs
docker-compose logs -f
```

### ❌ Client Secret Inválido

**Causa:** O `keycloak_client_secret` está incorreto

**Solução:**
1. Obtenha o Client Secret correto do Keycloak (veja seção "Configuração Inicial")
2. Atualize a variável no Environment

---

## 💡 Dicas Úteis

### 🎯 Atalhos do Postman

- `Ctrl/Cmd + Enter` - Enviar requisição
- `Ctrl/Cmd + L` - Limpar console
- `Ctrl/Cmd + K` - Busca rápida
- `Ctrl/Cmd + Alt + C` - Abrir console

### 📝 Editar Corpo das Requisições

Você pode editar os JSONs de exemplo em qualquer requisição POST/PUT:

```json
{
  "nome": "Seu Nome Personalizado",
  "descricao": "Sua Descrição Personalizada"
}
```

### 🔍 Inspecionar Respostas

- Aba **Body**: Veja o JSON de resposta
- Aba **Headers**: Veja os cabeçalhos HTTP
- Aba **Test Results**: Veja os testes automatizados
- Aba **Console**: Veja logs detalhados

### 💾 Exportar Resultados

Para documentação ou compartilhamento:
1. Execute o Collection Runner
2. Clique em **Export Results**
3. Escolha o formato (JSON, HTML)

---

## 📚 Recursos Adicionais

- **Swagger da API**: http://localhost:5000/swagger
- **Keycloak Admin**: http://localhost:8080 (admin/admin)
- **Documentação do Projeto**: Veja o README.md

---

## ✅ Checklist Rápido

Antes de começar os testes:

- [ ] Docker containers estão rodando (`docker ps`)
- [ ] Postman está aberto
- [ ] Coleção importada
- [ ] Environment importado e ativado
- [ ] Client Secret configurado
- [ ] Login executado com sucesso
- [ ] Token salvo automaticamente (`access_token` preenchido)

Pronto! Agora você pode testar todos os endpoints da API! 🎉