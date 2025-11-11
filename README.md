# 🚀 Challenge — Sistema de Gerenciamento de Tarefas

<div align="center">
  
  ![Coverage Badge](./coverage-badge.svg)
  
  <p>
    <img alt="dotnet" src="https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white" />
    <img alt="xUnit" src="https://img.shields.io/badge/Tests-xUnit-FF4081?logo=xunit&logoColor=white" />
    <img alt="Docker" src="https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white" />
    <img alt="CI/CD" src="https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?logo=github-actions&logoColor=white" />
  </p>

  <p>
    <strong>API .NET 9 robusta e bem testada para gerenciamento de Usuários e Tarefas</strong>
  </p>
  
  <p>
    Construída com <strong>Clean Architecture</strong> e padrões modernos de desenvolvimento
  </p>

</div>

---

## 📋 Sobre o Projeto

Uma API RESTful completa para gerenciamento de tarefas e usuários, desenvolvida com foco em:

- 🏗️ **Arquitetura Limpa** - Separação clara entre Domain, Application, Infrastructure e Presentation
- 🧪 **Testes Abrangentes** - Alta cobertura com testes unitários e de integração
- 📋 **Contratos Consistentes** - Padrão envelope para todas as respostas da API
- 🐳 **Containerização** - Pronto para deploy com Docker
- 📊 **Documentação Interativa** - Swagger UI integrado

> 💡 **Ideal para**: Avaliações técnicas, base para novos projetos ou referência para Clean Architecture em .NET

---

## ✨ Principais Características

<div align="center">

| 🏗️ **Arquitetura** | 🧪 **Qualidade** | 🚀 **Performance** | 📋 **API** |
|:-----------------:|:---------------:|:----------------:|:-----------:|
| Clean Architecture | 90%+ Cobertura | .NET 9 | RESTful |
| DDD Patterns | Testes Integração | InMemory DB | Swagger UI |
| SOLID Principles | CI/CD Pipeline | Docker Ready | Envelope Pattern |

</div>

### 🎯 Por que Tech Leads vão adorar

- ✅ **Separação clara de responsabilidades** (Domain / Application / Infrastructure / Presentation)
- ✅ **Padrão Notification** consistente para tratamento de erros e validações
- ✅ **Alta cobertura de testes** com cenários reais de integração
- ✅ **Base de código enxuta** - fácil de revisar, entender e estender
- ✅ **Documentação completa** com diagramas de arquitetura e exemplos práticos

---

## 🚀 Início Rápido

### Pré-requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) ou [Docker](https://www.docker.com/)

### 💻 Executando localmente

```bash
# 1. Clone o repositório
git clone https://github.com/edcamargo/challenge.git
cd challenge

# 2. Restore e build
dotnet restore
dotnet build -c Release

# 3. Execute a API
cd Presentation.Api
dotnet run --urls "http://localhost:5000"
```

✅ **API rodando em**: http://localhost:5000  
📋 **Swagger UI**: http://localhost:5000/swagger

### 🐳 Executando com Docker

```bash
# Build da imagem
docker build -t challenge:latest .

# Execute o container
docker run -d -p 8080:8080 --name challenge challenge:latest
```

<details>
<summary>🔧 Comandos úteis do Docker</summary>

```bash
# Ver logs
docker logs -f challenge

# Parar e remover
docker stop challenge && docker rm challenge

# Executar em modo interativo
docker run -it -p 8080:8080 challenge:latest
```
</details>

## 🏗️ Arquitetura do Sistema

<div align="center">
  <img src="docs/architecture.svg" alt="Diagrama de Arquitetura" width="700"/>
</div>

### 📚 Estrutura das Camadas

```
📁 Domain/              # Regras de negócio e entidades
├── Entities/           # User, Tasks
├── ValueObjects/       # Objetos de valor
├── Validations/        # Validações de domínio
└── Interfaces/         # Contratos de repositórios

📁 Application/         # Casos de uso e serviços
├── Services/           # UserService, TaskService
├── DTOs/              # Objetos de transferência
└── Common/            # ApiResponse, Notifications

📁 Infrastructure/      # Implementações técnicas
├── Data/              # EF Core, Repositórios
└── IoC/               # Injeção de dependência

📁 Presentation.Api/    # Controllers e configurações
├── Controllers/       # REST endpoints
├── Middlewares/       # Error handling
└── Extensions/        # Configurações
```

### 🔄 Padrões Implementados

- **🏛️ Clean Architecture** - Dependências apontando para dentro
- **📋 Domain-Driven Design** - Entidades ricas com validações
- **🔔 Notification Pattern** - Coleta e tratamento de erros
- **🎯 Dependency Injection** - IoC Container configurado
- **📊 Repository Pattern** - Abstração do acesso a dados

## 📋 Documentação da API

### 🌐 Swagger UI Interativo

<div align="center">
  <img src="docs/swagger.png" alt="Swagger UI" width="800"/>
  <p><em>Explore todos os endpoints, modelos e teste a API diretamente pelo navegador</em></p>
</div>

### 📨 Padrão Envelope de Resposta

Todas as respostas da API seguem um **envelope consistente** para facilitar o tratamento no frontend:

```json
{
  "data": "/* payload de sucesso ou null */", 
  "erros": ["/* array de objetos ApiError */"]
}
```

**Estrutura do ApiError:**
```json
{ 
  "statusCode": 400, 
  "message": "Mensagem amigável", 
  "key": "Campo" 
}
```

### 🎯 Benefícios do Padrão

- ✅ **Previsibilidade** - Respostas sempre no mesmo formato
- ✅ **Tratamento de Erros** - Erros estruturados e padronizados  
- ✅ **Frontend-Friendly** - Fácil mapeamento para componentes UI
- ✅ **Validações** - Erros por campo específico

## 🔧 Exemplos de Uso da API

### 👤 Gerenciamento de Usuários

<details>
<summary><strong>POST /api/users</strong> - Criar usuário</summary>

**Requisição:**
```bash
curl -X POST http://localhost:5000/api/users \
  -H 'Content-Type: application/json' \
  -d '{"name":"Edwin","email":"edwin@example.com"}'
```

**Resposta de Sucesso (201):**
```json
{
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "name": "Edwin",
    "email": "edwin@example.com"
  },
  "erros": []
}
```

**Resposta de Erro (400):**
```json
{
  "data": null,
  "erros": [
    {
      "statusCode": 400,
      "message": "E-mail inválido",
      "key": "email"
    }
  ]
}
```
</details>

### ✅ Gerenciamento de Tarefas

<details>
<summary><strong>POST /api/tasks</strong> - Criar tarefa</summary>

**Requisição:**
```bash
curl -X POST http://localhost:5000/api/tasks \
  -H 'Content-Type: application/json' \
  -d '{
    "title": "Implementar feature X",
    "description": "Desenvolver nova funcionalidade",
    "dueDate": "2025-11-15T12:00:00Z",
    "userId": "123e4567-e89b-12d3-a456-426614174000"
  }'
```
</details>

<details>
<summary><strong>GET /api/tasks/user/{userId}</strong> - Listar tarefas do usuário</summary>

**Resposta de Sucesso (200):**
```json
{
  "data": [
    {
      "id": "11111111-1111-1111-1111-111111111111",
      "title": "Implementar feature X",
      "description": "Desenvolver nova funcionalidade",
      "createdAt": "2025-11-11T12:00:00Z",
      "dueDate": "2025-11-15T12:00:00Z",
      "userId": "123e4567-e89b-12d3-a456-426614174000",
      "user": {
        "id": "123e4567-e89b-12d3-a456-426614174000",
        "name": "Edwin",
        "email": "edwin@example.com"
      },
      "isCompleted": false
    }
  ],
  "erros": []
}
```

**Resposta de Erro (404):**
```json
{
  "data": null,
  "erros": [
    {
      "statusCode": 404,
      "message": "Usuário não encontrado",
      "key": "userId"
    }
  ]
}
```
</details>

---

## Testes e Cobertura

Executar testes localmente:

```bash
dotnet test ./challenge.sln --collect:"XPlat Code Coverage"
```

Gerar relatório de cobertura legível (local):

```bash
# instalar uma vez
dotnet tool install -g dotnet-reportgenerator-globaltool
# gerar relatório
reportgenerator -reports:Challenge.Test/TestResults/*/coverage.cobertura.xml -targetdir:coverage-report -reporttypes "HtmlSummary;BadgeSummary"
open coverage-report/summary.html
```

**Métricas (última execução):**

- 📊 Coverage (linhas): **90.94%** — 633/696
- 🔀 Coverage (branches): **72.54%** — 74/102

---

## Contribuindo

1. Faça fork do repositório, crie branch `feature/...` ou `fix/...`
2. Execute a suíte de testes e adicione testes para novos comportamentos
3. Abra um PR com descrição e contexto

---

<!-- EOF -->
