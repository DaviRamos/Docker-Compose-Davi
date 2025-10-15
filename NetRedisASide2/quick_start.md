# 🚀 Guia Rápido de Instalação - NetRedisASide2

## Passo 1: Instalar Dependências

```bash
# Ubuntu/Debian
sudo apt update
sudo apt install -y docker.io docker-compose

# Instalar .NET 8
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Adicionar ao PATH
echo 'export PATH="$PATH:$HOME/.dotnet"' >> ~/.bashrc
source ~/.bashrc
```

## Passo 2: Configurar NVIDIA Docker (para GPU)

```bash
# Instalar NVIDIA Container Toolkit
distribution=$(. /etc/os-release;echo $ID$VERSION_ID)
curl -fsSL https://nvidia.github.io/libnvidia-container/gpgkey | sudo gpg --dearmor -o /usr/share/keyrings/nvidia-container-toolkit-keyring.gpg

curl -s -L https://nvidia.github.io/libnvidia-container/$distribution/libnvidia-container.list | \
    sed 's#deb https://#deb [signed-by=/usr/share/keyrings/nvidia-container-toolkit-keyring.gpg] https://#g' | \
    sudo tee /etc/apt/sources.list.d/nvidia-container-toolkit.list

sudo apt-get update
sudo apt-get install -y nvidia-container-toolkit
sudo nvidia-ctk runtime configure --runtime=docker
sudo systemctl restart docker

# Testar
docker run --rm --gpus all nvidia/cuda:11.8.0-base-ubuntu22.04 nvidia-smi
```

## Passo 3: Criar Estrutura do Projeto

```bash
# Criar diretórios
mkdir -p NetRedisASide2/src/NetRedisASide2.Api/{Models,Data,Repositories,Services,Endpoints,Extensions}
mkdir -p NetRedisASide2/docker/{keycloak,scripts}

cd NetRedisASide2
```

## Passo 4: Criar Arquivo .csproj

```bash
cd src/NetRedisASide2.Api

cat > NetRedisASide2.Api.csproj << 'EOF'
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.0.0" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
    <PackageReference Include="StackExchange.Redis" Version="2.7.10" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>
</Project>
EOF

cd ../..
```

## Passo 5: Copiar Todos os Arquivos

**Cole todos os arquivos fornecidos nos artifacts nos locais corretos:**

- Models/*.cs → src/NetRedisASide2.Api/Models/
- Data/*.cs → src/NetRedisASide2.Api/Data/
- Repositories/*.cs → src/NetRedisASide2.Api/Repositories/
- Services/*.cs → src/NetRedisASide2.Api/Services/
- Endpoints/*.cs → src/NetRedisASide2.Api/Endpoints/
- Extensions/*.cs → src/NetRedisASide2.Api/Extensions/
- Program.cs → src/NetRedisASide2.Api/
- appsettings*.json → src/NetRedisASide2.Api/
- docker-compose.yml → docker/
- .env → docker/
- realm-export.json → docker/keycloak/
- create-databases.sh → docker/scripts/

## Passo 6: Iniciar Serviços

```bash
cd docker

# Iniciar containers
docker-compose up -d

# Aguardar serviços ficarem prontos (2-3 minutos)
watch docker-compose ps

# Criar databases
cd scripts
chmod +x create-databases.sh
./create-databases.sh
```

## Passo 7: Configurar Aplicação .NET

```bash
cd ../../src/NetRedisASide2.Api

# Restaurar pacotes
dotnet restore

# Instalar EF Core Tools
dotnet tool install --global dotnet-ef

# Criar migrations
dotnet ef migrations add InitialCreate

# Aplicar no banco
dotnet ef database update
```

## Passo 8: Executar a API

```bash
dotnet run
```

## Passo 9: Testar

### Obter Token do Keycloak

```bash
TOKEN=$(curl -s -X POST 'http://localhost:8080/realms/NetRedisASide2/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'client_id=netredisaside2-api' \
  -d 'client_secret=your-generated-secret-here' \
  -d 'grant_type=password' \
  -d 'username=admin' \
  -d 'password=admin123' | jq -r '.access_token')

echo $TOKEN
```

### Criar um Assunto

```bash
curl -X POST 'http://localhost:5000/api/assuntos' \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{
    "nome": "Tecnologia",
    "descricao": "Assuntos relacionados à tecnologia"
  }'
```

### Listar Assuntos

```bash
curl -X GET 'http://localhost:5000/api/assuntos' \
  -H "Authorization: Bearer $TOKEN"
```

## 🎯 Verificações Importantes

### 1. PostgreSQL

```bash
docker exec -it netredisaside2-postgres psql -U postgres -c "\l"
```

### 2. Redis

```bash
docker exec -it netredisaside2-redis redis-cli ping
```

### 3. Keycloak

```bash
curl http://localhost:8080/realms/NetRedisASide2/.well-known/openid-configuration
```

### 4. Ollama

```bash
curl http://localhost:11434/api/tags
```

### 5. Weaviate

```bash
curl http://localhost:8081/v1/meta
```

## 📊 Monitoramento

### Ver logs de todos os serviços

```bash
cd docker
docker-compose logs -f
```

### Ver logs de um serviço específico

```bash
docker-compose logs -f postgres
docker-compose logs -f keycloak
docker-compose logs -f ollama
```

### Ver uso de recursos

```bash
docker stats
```

## 🛑 Parar Tudo

```bash
cd docker
docker-compose down
```

## 🔄 Reiniciar do Zero

```bash
cd docker
docker-compose down -v  # ⚠️ Remove todos os dados
docker-compose up -d
cd scripts
./create-databases.sh
cd ../../src/NetRedisASide2.Api
dotnet ef database update
dotnet run
```

## ✅ Checklist de Verificação

- [ ] Docker instalado e funcionando
- [ ] Docker Compose instalado
- [ ] .NET 8 SDK instalado
- [ ] NVIDIA Docker configurado (se usar GPU)
- [ ] Todos os containers rodando (docker-compose ps)
- [ ] Databases criados (./create-databases.sh)
- [ ] Migrations aplicadas (dotnet ef database update)
- [ ] API rodando (dotnet run)
- [ ] Token obtido do Keycloak
- [ ] Endpoint testado com sucesso

## 🆘 Problemas Comuns

### "Cannot connect to Docker daemon"

```bash
sudo systemctl start docker
sudo usermod -aG docker $USER
# Fazer logout e login novamente
```

### "Port already in use"

```bash
# Verificar quem está usando a porta
sudo lsof -i :5432
sudo lsof -i :8080

# Parar o serviço conflitante ou mudar a porta no docker-compose.yml
```

### "NVIDIA driver not found"

```bash
# Instalar drivers NVIDIA
sudo ubuntu-drivers autoinstall
sudo reboot
```

### "Keycloak not starting"

```bash
# Verificar logs
docker-compose logs keycloak

# Recriar database
docker-compose down
docker volume rm docker_postgres_data
docker-compose up -d
```

## 📱 URLs Importantes

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Keycloak**: http://localhost:8080 (admin/admin)
- **PostgreSQL**: localhost:5432
- **Redis**: localhost:6379
- **Ollama**: http://localhost:11434
- **Weaviate**: http://localhost:8081
