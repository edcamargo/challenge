# Gerenciamento de Tarefas — Documentação Técnica

Última atualização: 2025-11-10

Breve plano
-----------
- Criar um README claro, organizado e visualmente agradável (Markdown) que descreva: arquitetura, princípios de Clean Code adotados, padrões usados, contratos (endpoints), diagrama de entidades e dependências, instruções de execução e recomendações.
- Manter o foco em documentação útil ao desenvolvedor (onboarding, manutenção e extensão), sem alterar nenhum arquivo de código.

Este documento descreve a arquitetura, padrões, dependências e contratos HTTP da aplicação "Gerenciamento de Tarefas" (solution: `challenge.sln`). Inclui diagramas ASCII das entidades e das dependências entre os projetos.

Checklist (o que este README entrega)
-------------------------------------
- [x] Visão geral da arquitetura e responsabilidades por camada
- [x] Princípios de Clean Code e boas práticas aplicáveis ao projeto
- [x] Padrões de projeto usados (Notification, Result, Repository, UnitOfWork etc.)
- [x] Diagrama simplificado das entidades e dependências entre projetos
- [x] Contratos HTTP principais (entrada/saída) e exemplos
- [x] Como executar e testar localmente
- [x] Recomendações para evolução e testes

Sumário
-------
- Visão rápida
- Arquitetura por camadas
- Princípios Clean Code aplicados
- Padrões adotados
- Diagrama de entidades
- Diagrama de dependências entre projetos
- Endpoints principais (request / response)
- Execução local e testes
- Recomendações e próximos passos
- Docs (diagrama + OpenAPI)

Visão rápida
------------
Projeto: Gerenciamento de Tarefas
Solução: `challenge.sln`
Linguagem: C# (.NET 9)
Propósito: exemplo de API REST para criar/gerenciar usuários e tarefas com validação, padrão de notificações e repositórios.

Arquitetura por camadas
-----------------------
A aplicação segue uma separação clara de responsabilidades:

- Presentation (API): `Presentation.Api/`
  - Controllers, DTOs, middlewares, documentação (Swagger).
- Application (UseCases): `Application/`
  - Serviços que expõem operações de alto nível (orquestram repositórios e UoW).
- Domain: `Domain/`
  - Entidades, value objects e regras de negócio. Aqui vivem as validações e o padrão de notificações.
- Infrastructure Data: `InfraStructure.Data/`
  - Implementação do EF Core (`DataContext`), repositórios e `UnitOfWork`.
- Cross-Cutting: `InfraStructure.CrossCutting/`
  - Validações custom, atributos reutilizáveis e helpers.
- IoC: `InfraStructure.Ioc/`
  - Registro de dependências (injeção de dependência) e composição dos módulos.
- `Challenge.Test/`    — Projetos de testes (unitários / integração).

Este modelo facilita manutenção, testes e evolução (substituir detalhes de infra sem afetar o domínio).

Princípios de Clean Code aplicados
---------------------------------
1. Legibilidade antes de cleverness
   - Nomes significativos para classes, métodos e variáveis (ex.: `Tasks.Create`, `User.Create`).
2. Responsabilidade única
   - Cada camada tem papel definido: controllers apenas orquestram requisições; serviços contêm lógica de caso de uso; entidades cuidam de suas invariantes.
3. Código orientado a domínio
   - Regras e validações principais residem no `Domain` (fábricas e validators), não espalhadas pelos controllers.
4. Pequenas funções e métodos claros
   - Métodos curtos e descritivos favorecem testes e leitura.
5. Tratamento de erros sem exceções para validação
   - Padrão Notification/Notifiable para acumular erros e comunicar ao usuário sem usar `throw` para validações.

Padrões de projeto usados
-------------------------
- Notification / Notifiable
  - Entidades colecionam `Notification` em vez de lançar exceções nas validações.
- Result / Result<T> + ResultMapper
  - Envelopes padronizados para responses HTTP, permitindo retorno consistente de sucesso / falhas.
- Repository (genérico) + UnitOfWork
  - Repositório genérico para operações CRUD e `IUnitOfWork.SaveChangesAsync()` para controlar persistência e transações.
- DTOs + DataAnnotations
  - Validação inicial do contrato HTTP nas camadas de apresentação com atributos como `[Required]`, `[EmailAddress]` e atributos customizados (`NotEmptyGuid`, `FutureDate`).
- FluentValidation
  - Validações de domínio (ex.: `UserValidator`) para regras complexas do business model.

Diagrama de entidades (simplificado)
-----------------------------------
```
  +----------------+        1     *      +----------------+
  |     User       |--------------------|     Tasks      |
  |----------------|                    |----------------|
  | Id : Guid      |                    | Id : Guid      |
  | Name : string  |                    | Title : string |
  | Email : Email  |                    | Description ?  |
  | CreatedAt      |                    | CreatedAt      |
  +----------------+                    | DueDate ?      |
                                        | UserId : Guid  |
                                        | IsCompleted    |
                                        +----------------+
```

Observação:
- `Email` é um value object (ex.: `Domain.ValueObjects.Email`) e mapeado como owned type pelo EF Core.
- `Notification` é utilizado como objeto de domínio e por padrão não é persistido (`[NotMapped]`).

Diagrama de dependências entre projetos
--------------------------------------
```
Solution: challenge.sln

  +----------------------+       +------------------------------+
  |  Presentation.Api    | ----> |  Application                 |
  |  (Web API)           |       |  (Application services)      |
  +----------------------+       +------------------------------+
           |   ^                              |
           |   |                              v
           |   |                       +----------------------+   
           |   +---------------------> |  Domain              |   
           |                           |  (Entities, VO, Vlds)|   
           |                           +----------------------+   
           |                                   ^   ^             
           |                                   |   |             
           v                                   |   |             
  +----------------------+                    |   |             
  |  InfraStructure.Ioc  | <------------------+   |             
  |  (DI wiring)         |                        |             
  +----------------------+                        |             
           |                                      |             
           v                                      v             
  +----------------------+               +----------------------+  
  |  InfraStructure.Data |  <----------- | InfraStructure.Cross-|
  |  (EF Core, Repos,    |               | Cutting (Validation) |
  |   UnitOfWork)        |               +----------------------+  
  +----------------------+                                 
```

Docs (diagrama + OpenAPI)
-------------------------
- Diagrama visual: `docs/architecture.svg`

![Arquitetura do projeto](./docs/architecture.svg)

- OpenAPI (JSON): `docs/openapi.json`

Você pode abrir `docs/openapi.json` em uma ferramenta como Swagger UI ou https://editor.swagger.io/ para explorar os endpoints e os exemplos.

Formato de resposta
-------------------
A API usa um envelope consistente para respostas:
- Sucesso: `{ "success": true, "data": <objeto>, "notifications": [] }`
- Erro: `{ "success": false, "errors": [ "<Key>: <Message>", ... ] }`

Execução local (rápido)
-----------------------
Pré-requisitos: .NET 9 SDK

```bash
# compilar
dotnet build challenge.sln

# rodar API (Presentation.Api)
dotnet run --project Presentation.Api/Presentation.Api.csproj

# rodar testes
dotnet test challenge.sln
```

Observações importantes
----------------------
- Use a constraint `{id:guid}` nas rotas que recebem GUID para evitar passar strings para `DbSet.Find`.
- Controllers devem propagar `CancellationToken` para serviços e repositórios para permitir cancelamento cooperativo.
- Evite `SaveChanges` dentro de cada repositório: prefira `UnitOfWork` para controlar a transação por request.

Persistncia / EF Core (observaes)
------------------------------------
- `DataContext` configura `DbSet<User>` e `DbSet<Tasks>`.
- `Email` (value object)  mapeado com `OwnsOne` para persistir o `Endereco` como coluna.
- `Notification`  marcado `[NotMapped]` (no persistido por padro) — se quiser persistir notificaes, crie uma entidade mapevel (ex.: `NotificationEntity`).
- `Repository` genrico expe operaes async e `SaveChangesAsync`  centralizado via `UnitOfWork`.

Recomendações de evolução
-------------------------
- Documentação automática: gerar OpenAPI/Swagger com exemplos e usar `Swashbuckle` para enriquecer a documentação.
- Testes: adicionar testes de integração com `WebApplicationFactory` + InMemory provider (já existe um esqueleto de testes).
- Observability: adicionar logging estruturado e métricas (OpenTelemetry).
- Resiliência: politicas de retry/circuit-breaker para chamadas externas (Polly) se o projeto crescer.

Encerramento
------------
Este README tem por objetivo facilitar a leitura do projeto, apoiar onboard de novos desenvolvedores e orientar decisões arquiteturais. Se desejar, eu posso:
- melhorar a apresentação com imagens (PlantUML + PNG/SVG);
- gerar um `openapi.json` com exemplos e tipos; ou
- criar testes de integração mais completos e automatizados.

Escolha um dos próximos passos e eu executo: gerar PlantUML, OpenAPI/Swagger com exemplos, ou criar uma suíte de testes de integração completa.
