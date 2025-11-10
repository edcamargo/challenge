# Challenge — Gerenciamento de Tarefas  
<p align="left">  
  <img alt="dotnet" src="https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white" />
  <img alt="xUnit" src="https://img.shields.io/badge/Tests-xUnit-FF4081?logo=xunit&logoColor=white" />
  <img alt="coverage" src="https://img.shields.io/badge/Coverage-90.9%25-brightgreen" />
</p>

Uma API em .NET 9 para gerenciamento de tarefas (Users + Tasks) construída com princípios de Clean Architecture: Domain, Application, Infrastructure e Presentation.

- Linguagem: C# (.NET 9)
- Testes: xUnit + FluentAssertions + NSubstitute
- Persistência nos testes: EF Core InMemory

---

## Sumário

- [Visão geral](#visão-geral)
- [Arquitetura & Diagramas](#arquitetura--diagramas)
- [Como rodar](#como-rodar)
  - [Rodar local (.NET)](#rodar-local-net)
  - [Rodar com Docker](#rodar-com-docker)
- [Endpoints principais (exemplos)](#endpoints-principais-exemplos)
- [Padronização de respostas (ApiResponse)](#padronização-de-respostas-apiresponse)
- [Testes e cobertura](#testes-e-cobertura)
- [Contribuição](#contribuição)

---

## Visão geral

A aplicação é organizada por camadas:

- Domain: Entidades, ValueObjects e validações de domínio.
- Application: DTOs, Services (casos de uso), interfaces e mapeamentos.
- InfraStructure: Implementações de repositórios, DataContext (EF Core) e IoC.
- Presentation: API (controllers), middlewares e documentação (Swagger).

Principais decisões:
- Notification pattern (ApiResponse/ApiError) para retornar validações/erros sem lançar exceções.
- Repositório + UnitOfWork para controle de persistência.
- Testes automatizados (unit + integration) com provider InMemory para fácil execução.

---

## Arquitetura & Diagramas

- Diagrama da arquitetura: `docs/architecture.puml` / `docs/architecture.svg`
- Diagrama das entidades: `docs/entities.puml` / `docs/entities.svg`

Abaixo o diagrama simplificado da arquitetura da aplicação:

![Architecture diagram](docs/architecture.svg)

Breve explicação das camadas:

- Presentation.Api — controllers, endpoints e integração com Swagger/UI.
- Application — serviços que implementam as regras de negócio e tratam validações.
- Domain — entidades, value-objects e validações de domínio.
- InfraStructure.Data — DataContext, repositórios e UnitOfWork.
- InfraStructure.Ioc — composition root e registro de dependências.

---

## Como rodar

Requisitos: .NET 9 SDK (para execução local) e Docker (opcional).

### Rodar local (.NET)

```bash
# restaurar e compilar
dotnet restore
dotnet build -c Debug

# rodar API (Presentation.Api)
cd Presentation.Api
dotnet run --urls "http://localhost:5000"
```

Abra `http://localhost:5000/swagger/index.html` para explorar a API em modo de desenvolvimento.

### Rodar com Docker

Os comandos abaixo foram fornecidos para criar e executar uma imagem Docker localmente.

1) Build da imagem Docker (a partir da raiz do repositório):

```bash
docker build -t challenge:latest .
```

2) Executar a imagem em segundo plano e mapear a porta 8080:

```bash
docker run -d -p 8080:8080 --name challenge challenge:latest
```

Dicas úteis:

```bash
# ver logs do container
docker logs -f challenge

# parar e remover
docker stop challenge && docker rm challenge

# abrir um shell dentro do container
docker exec -it challenge /bin/bash
```

> Observação: a aplicação por padrão usa o provider InMemory nos testes e para execução local; ajuste variáveis de ambiente se quiser conectar a um banco externo.

---

## Endpoints principais (exemplos)

Base: `http://localhost:5000/api`

### Users

- POST /api/users — criar usuário
- GET /api/users — listar usuários
- GET /api/users/{id} — buscar por id
- PUT /api/users/{id} — atualizar
- DELETE /api/users/{id} — remover

### Tasks

- POST /api/tasks — criar tarefa
- GET /api/tasks — listar tarefas
- GET /api/tasks/{id} — obter por id
- GET /api/tasks/user/{userId} — tarefas de um usuário
- PUT /api/tasks/{id}/complete — atualizar (rota atual)
- DELETE /api/tasks/{id} — remover

Exemplo com `TaskCreateDto` (JSON):

```json
{
  "title": "Comprar leite",
  "description": "Ir ao supermercado",
  "createdAt": "2025-11-10T12:00:00Z",
  "dueDate": "2025-11-12T12:00:00Z",
  "userId": "<guid>",
  "isCompleted": false
}
```

---

## Padronização de respostas (ApiResponse)

Todos os controllers retornam `ApiResponse<T>` com duas propriedades principais:

- `data`: o payload quando sucesso
- `erros`: array de `ApiError` contendo { statusCode, message, key }

Exemplo de erro (400):

```json
{
  "data": null,
  "erros": [ { "statusCode": 400, "message": "O título da tarefa é obrigatório.", "key": "title" } ]
}
```

### ✅ Padrão envelopado — vantagens

Adotamos respostas envelopadas (`ApiResponse<T>`) nos endpoints; abaixo as vantagens principais:

- 🔄 Consistência: sempre o mesmo envelope (`data` + `erros`) facilita parsing e uso por clientes.
- 🧩 Centralização de erros: validações e mensagens ficam padronizadas, reduzindo lógica repetida em controllers.
- 📦 Robustez na evolução da API: permite adicionar campos (meta, paging, links) sem quebrar clientes existentes.
- 🧪 Testabilidade: facilita asserts nos testes (verificar `data` ou `erros`) e simular cenários de erro/sucesso.
- 🚦 Mapeamento HTTP claro: o envelope contém informação de status/erro que pode ser usada para mapear códigos HTTP coerentes.
- 🌍 Localização e contexto: erros podem incluir `key` e mensagens prontas para tradução/consumo pelo cliente.
- 📈 Observabilidade: facilita registro/telemetria de erros e métricas de negócio ao centralizar mensagens.

---

## Testes e cobertura

Executar suíte de testes (local):

```bash
dotnet test ./challenge.sln --collect:"XPlat Code Coverage"
```

Gerar relatório HTML (local) com `reportgenerator` (instale a ferramenta globalmente se necessário):

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:Challenge.Test/TestResults/*/coverage.cobertura.xml -targetdir:coverage-report -reporttypes "HtmlSummary;BadgeSummary"
open coverage-report/summary.html
```

O arquivo de cobertura XML gerado pelos testes está em `Challenge.Test/TestResults/*/coverage.cobertura.xml`.

---

## Métricas de cobertura (última execução)

- 📊 Coverage (linhas): **90.94%** — 633/696
- 🔀 Coverage (branches): **72.54%** — 74/102

_Dica:_ rode `reportgenerator` para gerar o badge SVG que pode ser colocado no topo do README.

---

## Contribuição

1. Fork → branch `feature/...` ou `fix/...`
2. Rode os testes localmente e garanta que tudo passe
3. Abra PR com descrição clara e referências aos arquivos alterados

---

<!-- fim do README -->
