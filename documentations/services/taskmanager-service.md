# TaskManager Service

Responsável pelo gerenciamento de tarefas e labels no LifeSync.

## Índice

- [Visão Geral](#visão-geral)
- [Estrutura de Pastas](#estrutura-de-pastas)
- [Domínio](#domínio)
- [Aplicação](#aplicação)
- [Infraestrutura](#infraestrutura)
- [API](#api)
- [Testes](#testes)
- [Configuração](#configuração)
- [Dependências](#dependências)

---

## Problemas Críticos

> **Fonte:** Code Review - 03/03/2026  
> **Alerta:** Issues que impedem o serviço de ir para produção

### Tabela de Issues Críticos e Altos

| # | Severidade | Camada | Issue | Arquivo | Impacto |
|---|-----------|--------|-------|---------|---------|
| 1 | **CRÍTICO** | API | JWT Secret Key hardcoded no appsettings.json | `appsettings.json` | Segurança: chave exposta no repositório |
| 2 | **CRÍTICO** | API | `[Authorize]` comentado no ApiController base | `ApiController.cs:675` | Segurança: todos os endpoints abertos |
| 3 | **CRÍTICO** | Application | UserId passado no body em vez de extraído do JWT | `*CommandHandler.cs` | Segurança: usuários podem acessar dados de outros |
| 4 | **CRÍTICO** | Infrastructure | Paginação em memória (carrega todos os registros antes de paginar) | `TaskItemService.cs:379` | Performance: OutOfMemoryException com 100k+ registros |
| 5 | **CRÍTICO** | Infrastructure | SaveChanges individual em operações batch | `TaskItemRepository.cs:416` | Performance: 50 itens = 50 round-trips ao banco |
| 6 | **ALTO** | Infrastructure | Ausência de Unit of Work explícito | Repository pattern | Consistência: impossível operações atômicas entre repositórios |
| 7 | **ALTO** | Infrastructure | Global Query Filter para soft delete ausente | `ApplicationDbContext.cs` | Consistência: registros deletados retornam em queries |
| 8 | **ALTO** | Domain | Enum `Status` e `LabelColor` sem validação com `Enum.IsDefined()` | `TaskItem.cs`, `TaskLabel.cs` | Integridade: valores inválidos podem ser persistidos |
| 9 | **ALTO** | Domain | Erros de query/paginação no Domain (violação de fronteira) | `TaskLabelErrors.cs` | Arquitetura: Domain conhece conceitos de Application |
| 10 | **ALTO** | Domain | Aggregate Root não declarado (`IAggregateRoot` não implementado) | `TaskItem.cs`, `TaskLabel.cs` | DDD: sem fronteiras claras de aggregates |
| 11 | **ALTO** | Application | Validação duplicada (FluentValidation vs Domain entity) | Múltiplos | Manutenção: regras divergentes entre camadas |
| 12 | **ALTO** | Application | `UpdateTaskItemCommandValidator` não valida `DueDate >= hoje` | `UpdateTaskItemCommandValidator.cs` | Biz: é possível atualizar task com data no passado |
| 13 | **ALTO** | Infrastructure | DueDateReminderService fire-and-forget sem retry | `DueDateReminderService.cs` | Confiabilidade: lembretes perdidos silenciosamente |
| 14 | **ALTO** | Infrastructure | DueDateReminderService sem controle de duplicatas | `DueDateReminderService.cs` | UX: usuários recebem múltiplos lembretes da mesma tarefa |

### Resumo Quantitativo

| Severidade | Quantidade | Status |
|------------|-----------|--------|
| CRÍTICO | 5 | Requer correção imediata |
| ALTO | 9 | Corrigir no próximo sprint |
| MÉDIO | 13 | Backlog |
| BAIXO | 6 | Longo prazo |

---

## Recomendações de Correção

### Prioridade 1 — Críticos (Sprint Atual)

| # | Correção | Esforço | Arquivos |
|---|----------|---------|-----------|
| 1 | Descomentar `[Authorize]` no ApiController | 5 min | `Core.API/Controllers/ApiController.cs` |
| 2 | Remover JWT Key do appsettings.json, usar User Secrets | 30 min | `appsettings.json`, `Program.cs` |
| 3 | Extrair UserId do JWT via `IHttpContextAccessor` | 2h | `*CommandHandler.cs` |
| 4 | Corrigir paginação — usar Skip/Take no IQueryable | 2h | `TaskItemService.cs`, `TaskItemRepository.cs` |

### Prioridade 2 — Altos (Próximo Sprint)

| # | Correção | Esforço | Arquivos |
|---|----------|---------|-----------|
| 5 | Implementar Global Exception Handler | 2h | `Program.cs` |
| 6 | Adicionar Global Query Filter para `IsDeleted` | 30 min | `ApplicationDbContext.cs` |
| 7 | Implementar Unit of Work | 4h | `IUnitOfWork.cs`, repositories |
| 8 | Resolver validação duplicada (handlers vs pipeline) | 2h | `*Handler.cs`, `ValidationBehavior.cs` |
| 9 | Mover erros de query do Domain para Application | 1h | `TaskLabelErrors.cs` |
| 10 | Adicionar controle de duplicatas no DueDateReminderService | 3h | `DueDateReminderService.cs` |
| 11 | Implementar Rate Limiting | 1h | `Program.cs` |
| 12 | Health checks com verificação de dependências | 1h | `Program.cs` |

### Prioridade 3 — Médios (Backlog)

| # | Correção | Esforço |
|---|----------|---------|
| 13 | Padronizar validação de enums no Domain | 1h |
| 14 | Case-insensitive search (`EF.Functions.ILike`) | 1h |
| 15 | Cachear tipos no CQRS Sender | 2h |
| 16 | Adicionar métodos funcionais ao `Result<T>` | 2h |
| 17 | Configurar CORS | 30 min |
| 18 | Implementar API Versioning | 2h |
| 19 | Configurar Swagger com JWT | 30 min |
| 20 | Logging estruturado com Serilog | 2h |
| 21 | Adicionar índices compostos no banco | 30 min |
| 22 | Batch size limit nos endpoints | 30 min |
| 23 | Auto-migration condicional por environment | 30 min |
| 24 | Eager loading condicional | 1h |

### Prioridade 4 — Baixos / Longo Prazo

| # | Correção | Esforço |
|---|----------|---------|
| 25 | Padronizar enums para 1-based | 30 min |
| 26 | Renomear `Priority.ToString()` para `ToFriendlyString()` | 15 min |
| 27 | Implementar `IAggregateRoot` | 1h |
| 28 | Domain Events em entidades | 4h |
| 29 | Value Objects (`TaskTitle`, `TaskDescription`) | 8h |
| 30 | Caching strategy (Redis/Memory) | 4h |
| 31 | Outbox Pattern para eventos | 8h |
| 32 | Testes de concorrência | 4h |

---

## Score / Qualidade do Serviço

### Avaliação Geral

| Dimensão | Score | Observação |
|----------|-------|------------|
| **Arquitetura** | 7.5/10 | Clean Architecture bem implementada com CQRS e Repository Pattern |
| **Segurança** | 3.0/10 | CRÍTICO: Authorize desabilitado, JWT exposto, UserId no body |
| **Performance** | 5.0/10 | Paginação em memória, SaveChanges individual em batch |
| **Código** | 6.5/10 | Padrões bem aplicados, mas com inconsistências |
| **Testes** | 6.0/10 | 42 testes unitários, carece de integração e E2E |
| **DDD** | 5.5/10 | Aggregate Roots não declarados, Domain Events não utilizados |

### Score Consolidado: **5.5/10**

> **Nota:** O serviço possui uma arquitetura sólida de base (Clean Architecture + CQRS), porém apresenta **falhas críticas de segurança** e **problemas de performance** que impedem sua utilização em produção. As correções de Prioriade 1 devem ser implementadas antes de qualquer deploy.

### Principais Pontos Fortes

- Clean Architecture bem definida com separação clara de responsabilidades
- CQRS implementado com Commands/Queries e Handlers
- Repository Pattern com Specification Pattern
- Result Pattern para tratamento de erros
- Validação com FluentValidation
- Background service para lembretes via RabbitMQ

### Principais Pontos Fracos

- **Segurança:** Autorização desabilitada e JWT exposto
- **Performance:** Paginação em memória e operações batch ineficientes
- **DDD:** Aggregate Roots não declarados, Domain Events não disparados
- **Consistência:** Unit of Work ausente, Global Query Filter ausente

---

## Visão Geral

O TaskManager Service gerencia o ciclo de vida completo de tarefas dos usuários — criação, atualização, organização com labels, prioridades e status. Um background service monitora tarefas próximas do vencimento e publica eventos de lembrete via RabbitMQ para o Notification Service.

### Responsabilidades

- CRUD de tarefas (`TaskItem`) com suporte a prioridades e status
- CRUD de labels (`TaskLabel`) com suporte a cores
- Relacionamento many-to-many entre tarefas e labels
- Criação em lote de tarefas
- Filtros e busca avançada com paginação
- Background service para lembretes de vencimento de tarefas

---

## Estrutura de Pastas

```
TaskManager/
├── TaskManager.API/
│   ├── Controllers/
│   │   ├── TaskItemsController.cs
│   │   └── TaskLabelsController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── TaskManager.API.csproj
├── TaskManager.Application/
│   ├── Contracts/
│   │   ├── ITaskItemService.cs
│   │   └── ITaskLabelService.cs
│   ├── DTOs/
│   │   ├── TaskItem/
│   │   │   ├── CreateTaskItemDTO.cs
│   │   │   ├── TaskItemDTO.cs
│   │   │   ├── UpdateTaskItemDTO.cs
│   │   │   ├── TaskItemSimpleDTO.cs
│   │   │   ├── TaskItemFilterDTO.cs
│   │   │   └── UpdateLabelsDTO.cs
│   │   └── TaskLabel/
│   │       ├── CreateTaskLabelDTO.cs
│   │       ├── TaskLabelDTO.cs
│   │       ├── UpdateTaskLabelDTO.cs
│   │       ├── TaskLabelSimpleDTO.cs
│   │       └── TaskLabelFilterDTO.cs
│   ├── Features/
│   │   ├── TaskItems/
│   │   │   ├── Commands/        # Create, Update, Delete, AddLabel, RemoveLabel
│   │   │   └── Queries/         # GetAll, GetById, GetByFilter, GetByUser
│   │   └── TaskLabels/
│   │       ├── Commands/        # Create, Update, Delete
│   │       └── Queries/         # GetAll, GetById, GetByFilter, GetByUser
│   ├── Mapping/
│   │   ├── TaskItemMapper.cs
│   │   └── TaskLabelMapper.cs
│   └── TaskManager.Application.csproj
├── TaskManager.Domain/
│   ├── Entities/
│   │   ├── TaskItem.cs
│   │   └── TaskLabel.cs
│   ├── Enums/
│   │   ├── Priority.cs
│   │   ├── Status.cs
│   │   └── LabelColor.cs
│   ├── Errors/
│   │   ├── TaskItemErrors.cs
│   │   └── TaskLabelErrors.cs
│   ├── Events/TaskDueReminderEvent.cs
│   ├── Filters/
│   │   ├── TaskItemQueryFilter.cs
│   │   ├── TaskLabelQueryFilter.cs
│   │   └── Specifications/
│   │       ├── TaskItemSpecification.cs
│   │       └── TaskLabelSpecification.cs
│   └── Repositories/
│       ├── ITaskItemRepository.cs
│       └── ITaskLabelRepository.cs
└── TaskManager.Infrastructure/
    ├── Persistence/
    │   ├── Data/
    │   │   ├── ApplicationDbContext.cs
    │   │   └── MigrationHostedService.cs
    │   ├── Configuration/
    │   │   ├── TaskItemConfiguration.cs
    │   │   └── TaskLabelConfiguration.cs
    │   ├── Repositories/
    │   │   ├── TaskItemRepository.cs
    │   │   └── TaskLabelRepository.cs
    │   └── Migrations/
    ├── Services/
    │   ├── TaskItemService.cs
    │   └── TaskLabelService.cs
    ├── BackgroundServices/DueDateReminderService.cs
    ├── Options/DueDateReminderOptions.cs
    └── TaskManager.Infrastructure.csproj
```

---

## Domínio

### Entidade: `TaskItem`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Descrição |
|---|---|---|
| `UserId` | `int` | Dono da tarefa |
| `Title` | `string` | Título (max 200 chars) |
| `Description` | `string` | Descrição (max 1000 chars) |
| `Status` | `Status` | Status atual |
| `Priority` | `Priority` | Prioridade |
| `DueDate` | `DateOnly` | Data de vencimento |
| `Labels` | `IReadOnlyCollection<TaskLabel>` | Labels vinculadas |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `Update(title, description, status, priority, dueDate)` | Atualiza todos os campos |
| `SetTitle(title)` | Define título com validação |
| `SetDescription(description)` | Define descrição com validação |
| `SetPriority(priority)` | Define prioridade com validação |
| `SetDueDate(dueDate)` | Define data (não pode ser no passado) |
| `ChangeStatus(status)` | Altera o status |
| `AddLabel(label)` | Adiciona label (impede duplicatas) |
| `RemoveLabel(label)` | Remove label |
| `IsOverdue()` | Retorna `true` se vencida |
| `IsComplete()` | Retorna `true` se concluída |
| `HasLabel(labelId)` | Verifica se label está vinculada |

---

### Entidade: `TaskLabel`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Descrição |
|---|---|---|
| `UserId` | `int` | Dono do label |
| `Name` | `string` | Nome (max 50 chars) |
| `LabelColor` | `LabelColor` | Cor do label |
| `Items` | `IReadOnlyCollection<TaskItem>` | Tarefas vinculadas |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `Update(name, labelColor)` | Atualiza nome e cor |
| `AddTaskItem(item)` | Vincula tarefa (impede duplicatas) |
| `RemoveTaskItem(item)` | Remove vínculo com tarefa |

---

### Enumerações

#### `Priority`

| Valor | Int | Nome PT |
|---|---|---|
| `Low` | 1 | Baixa |
| `Medium` | 2 | Média |
| `High` | 3 | Alta |
| `Urgent` | 4 | Urgente |

#### `Status`

| Valor | Int | Nome PT |
|---|---|---|
| `Pending` | 1 | Pendente |
| `InProgress` | 2 | Em progresso |
| `Completed` | 3 | Completado |

#### `LabelColor`

| Valor | Int | Hex |
|---|---|---|
| `Red` | 0 | `#FF0000` |
| `Green` | 1 | `#00FF00` |
| `Blue` | 2 | `#0000FF` |
| `Yellow` | 3 | `#FFFF00` |
| `Purple` | 4 | `#800080` |
| `Orange` | 5 | `#FFA500` |
| `Pink` | 6 | `#FFC0CB` |
| `Brown` | 7 | `#A52A2A` |
| `Gray` | 8 | `#808080` |

---

### Evento de Integração: `TaskDueReminderEvent`

Publicado pelo `DueDateReminderService` quando uma tarefa está próxima do vencimento.

| Propriedade | Tipo |
|---|---|
| `TaskId` | `int` |
| `UserId` | `int` |
| `TaskTitle` | `string` |
| `DueDate` | `DateOnly` |

Exchange: `task_exchange` | Routing Key: `task.due.reminder`

---

### Erros de Domínio

#### `TaskItemErrors`

- `InvalidTitle`, `InvalidDescription`, `InvalidPriority`, `DueDateInPast`
- `NullLabel`, `DuplicateLabel`, `LabelNotFound`
- `NotFound(id)`, `CreateError`, `UpdateError`, `DeleteError`

#### `TaskLabelErrors`

- `InvalidName`, `NotFound(id)`
- `NullItem`, `DuplicateItem`, `ItemNotFound`
- `EmptyOrNullList`, `InvalidIds`, `SomeNotFound()`, `AllNotFound`
- `CreateError`, `UpdateError`, `DeleteError`

---

### Filtros e Especificações

#### `TaskItemQueryFilter`

| Filtro | Tipo |
|---|---|
| `Id` | `int?` |
| `UserId` | `int?` |
| `TitleContains` | `string?` |
| `Status` | `Status?` |
| `Priority` | `Priority?` |
| `DueDate` | `DateOnly?` |
| `LabelId` | `int?` |
| `CreatedAt`, `UpdatedAt`, `IsDeleted` | herdados |
| `SortBy`, `SortDesc`, `Page`, `PageSize` | paginação |

#### `TaskLabelQueryFilter`

| Filtro | Tipo |
|---|---|
| `Id` | `int?` |
| `UserId` | `int?` |
| `TaskItemId` | `int?` |
| `NameContains` | `string?` |
| `LabelColor` | `LabelColor?` |
| Paginação padrão: `Page=1`, `PageSize=50` | |

---

## Aplicação

### Commands — TaskItem

| Command | Retorno | Validação | Descrição |
|---|---|---|---|
| `CreateTaskItemCommand` | `CreateTaskItemResult(int Id)` | Título 3-100, Descrição 5-500, DueDate >= hoje | Cria tarefa com labels opcionais |
| `UpdateTaskItemCommand` | `UpdateTaskItemResult(bool IsUpdated)` | Mesmo que Create | Atualiza tarefa existente |
| `DeleteTaskItemCommand` | `DeleteTaskItemResult(bool IsDeleted)` | — | Remove tarefa |
| `AddLabelCommand` | `AddLabelResult(bool IsSuccess)` | IDs > 0, lista não vazia | Adiciona labels à tarefa |
| `RemoveLabelCommand` | `RemoveLabelResult(bool IsSuccess)` | IDs > 0, lista não vazia | Remove labels da tarefa |

### Commands — TaskLabel

| Command | Retorno | Validação | Descrição |
|---|---|---|---|
| `CreateTaskLabelCommand` | `CreateTaskLabelResult(int Id)` | Nome 2-50 chars | Cria label |
| `UpdateTaskLabelCommand` | `UpdateTaskLabelResult(bool IsUpdated)` | Nome 2-50 chars | Atualiza label |
| `DeleteTaskLabelCommand` | `DeleteTaskLabelResult(bool IsDeleted)` | — | Remove label |

---

### Queries — TaskItem

| Query | Retorno | Descrição |
|---|---|---|
| `GetAllTaskItemsQuery` | `IEnumerable<TaskItemDTO>` | Todas as tarefas com labels |
| `GetTaskItemByIdQuery(id)` | `TaskItemDTO` | Tarefa por ID com labels |
| `GetByUserQuery(userId)` | `IEnumerable<TaskItemDTO>` | Tarefas de um usuário |
| `GetTaskItemsByFilterQuery(filter)` | `IEnumerable<TaskItemDTO>` + `PaginationData` | Filtro avançado com paginação |

### Queries — TaskLabel

| Query | Retorno | Descrição |
|---|---|---|
| `GetAllTaskLabelsQuery` | `IEnumerable<TaskLabelDTO>` | Todos os labels |
| `GetTaskLabelByIdQuery(id)` | `TaskLabelDTO` | Label por ID |
| `GetByUserQuery(userId)` | `IEnumerable<TaskLabelDTO>` | Labels de um usuário |
| `GetTaskLabelsByFilterQuery(filter)` | `IEnumerable<TaskLabelDTO>` + `PaginationData` | Filtro avançado com paginação |

---

### DTOs — TaskItem

```csharp
// Leitura completa
TaskItemDTO(Id, CreatedAt, UpdatedAt, Title, Description, Status, Priority, DueDate, UserId, Labels: List<TaskLabelDTO>)

// Criação
CreateTaskItemDTO(Title, Description, Priority, DueDate, UserId, TaskLabelsId: List<int>?)

// Atualização
UpdateTaskItemDTO(Id, Title, Description, Status, Priority, DueDate)

// Gerenciar labels
UpdateLabelsDTO(TaskItemId, TaskLabelsId: List<int>)

// Filtro
TaskItemFilterDTO(Id?, UserId?, TitleContains?, Status?, Priority?, DueDate?, LabelId?, ...paginação)
```

### DTOs — TaskLabel

```csharp
// Leitura completa
TaskLabelDTO(Id, CreatedAt, UpdatedAt, Name, LabelColor, UserId)

// Criação
CreateTaskLabelDTO(Name, LabelColor, UserId)

// Atualização
UpdateTaskLabelDTO(Id, Name, LabelColor)

// Filtro
TaskLabelFilterDTO(Id?, UserId?, ItemId?, NameContains?, LabelColor?, ...paginação)
```

---

## Infraestrutura

### `ApplicationDbContext`

| DbSet | Tipo |
|---|---|
| `TaskItems` | `DbSet<TaskItem>` |
| `TaskLabels` | `DbSet<TaskLabel>` |

### Configuração de Banco

**TaskItems:**
- Tabela: `TaskItems`
- `Title`: obrigatório, max 200
- `Description`: obrigatório, max 1000
- `Status` / `Priority`: convertidos para int
- `IsDeleted`: default `false`
- Relacionamento **many-to-many** com `TaskLabel`
- Índices: `UserId`, `Status`, `DueDate`, `IsDeleted`

**TaskLabels:**
- `Name`: obrigatório, max 50
- Índice: `UserId`

### Migrations

| Migration | Data | Descrição |
|---|---|---|
| `20250704233455_initialCreate` | 2025-07-04 | Schema inicial |
| `20250704234109_initialCreate1` | 2025-07-04 | Ajustes |
| `20250704234535_initialCreate2` | 2025-07-04 | Ajustes |
| `20250704235010_initialCreate3` | 2025-07-04 | Schema final inicial |
| `20251025221643_newChanges` | 2025-10-25 | Alterações de propriedades/relacionamentos |
| `20260106035023_manylabels` | 2026-01-06 | Converteu 1:N para M:N — criou join table `TaskItemTaskLabel` |

### Background Service: `DueDateReminderService`

Serviço em background que monitora tarefas próximas do vencimento:

| Opção | Padrão | Descrição |
|---|---|---|
| `ReminderThreshold` | 1 dia | Antecedência para enviar lembrete |
| `PollingInterval` | 1 hora | Intervalo entre verificações |
| `ExchangeName` | `task_exchange` | Exchange RabbitMQ |
| `RoutingKey` | `task.due.reminder` | Routing key |
| `MaxTasksPerRun` | 100 | Limite de tarefas por execução |

---

## API

### `TaskItemsController` — `/api/task-items`

| Método | Rota | Body / Params | Retorno |
|---|---|---|---|
| GET | `/{id}` | `id: int` | `TaskItemDTO` |
| GET | `/user/{userId}` | `userId: int` | `IEnumerable<TaskItemDTO>` |
| GET | `/search` | `TaskItemFilterDTO` (query) | Paginado |
| GET | `/` | — | `IEnumerable<TaskItemDTO>` |
| POST | `/` | `CreateTaskItemCommand` | `201 { Id }` |
| POST | `/batch` | `IEnumerable<CreateTaskItemCommand>` | `201 { count }` |
| POST | `/{id}/addLabels` | `AddLabelCommand` | `200` |
| POST | `/{id}/removeLabels` | `RemoveLabelCommand` | `200` |
| PUT | `/{id}` | `UpdateTaskItemCommand` | `200` |
| DELETE | `/{id}` | `id: int` | `200` |

---

### `TaskLabelsController` — `/api/task-labels`

| Método | Rota | Body / Params | Retorno |
|---|---|---|---|
| GET | `/{id}` | `id: int` | `TaskLabelDTO` |
| GET | `/user/{userId}` | `userId: int` | `IEnumerable<TaskLabelDTO>` |
| GET | `/search` | `TaskLabelFilterDTO` (query) | Paginado |
| GET | `/` | — | `IEnumerable<TaskLabelDTO>` |
| POST | `/` | `CreateTaskLabelCommand` | `201 { Id }` |
| POST | `/batch` | `IEnumerable<CreateTaskLabelCommand>` | `201 { count }` |
| PUT | `/{id}` | `UpdateTaskLabelCommand` | `200` |
| DELETE | `/{id}` | `id: int` | `200` |

---

### Health Check

```
GET /health
→ { "status": "healthy", "service": "TaskManager", "timestamp": "...", "environment": "..." }
```

---

## Testes

O TaskManager possui cobertura de testes completa com a pirâmide de três camadas:

### Projetos de Teste

| Projeto | Tipo | Tecnologias |
|---|---|---|
| `TaskManager.UnitTests` | Unitários | xUnit, Moq, FluentAssertions, AutoFixture, Bogus |
| `TaskManager.IntegrationTests` | Integração | xUnit, Testcontainers.PostgreSql, Respawn, FluentAssertions, Bogus |
| `TaskManager.E2ETests` | E2E | xUnit, WebApplicationFactory, Testcontainers.PostgreSql, FluentAssertions |

### Executando os Testes

```bash
# Unitários
dotnet test tests/TaskManager.UnitTests

# Integração (requer Docker)
dotnet test tests/TaskManager.IntegrationTests

# E2E (requer Docker)
dotnet test tests/TaskManager.E2ETests

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

---

## Configuração

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=LifeSync;Include Error Detail=true;"
  },
  "JwtSettings": {
    "Key": "SuperSecretKeyForJWTAuthentication2024!@#$%",
    "Issuer": "LifeSyncAPI",
    "Audience": "LifeSyncApp",
    "ExpiryMinutes": 60
  },
  "RabbitMQSettings": {
    "Host": "rabbitmq",
    "User": "guest",
    "Password": "guest",
    "Port": 5672
  }
}
```

---

## Dependências

### Pacotes NuGet

| Pacote | Versão | Uso |
|---|---|---|
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.0 | Provider PostgreSQL |
| `Microsoft.EntityFrameworkCore` | 10.0.3 | ORM |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.3 | Migrations CLI |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 10.0.3 | Identity |
| `Swashbuckle.AspNetCore` | 10.1.4 | Swagger |
| `Microsoft.Extensions.Hosting.Abstractions` | 10.0.3 | BackgroundService |

### Referências Internas

| Projeto | Uso |
|---|---|
| `BuildingBlocks` | CQRS, Result, Validation |
| `BuildingBlocks.Messaging` | RabbitMQ (publicar `TaskDueReminderEvent`) |
| `Core.Domain` | BaseEntity, Specification, IRepository |
| `Core.API` | ApiController base |

### Schema do Banco

```
TaskItems           TaskItemTaskLabel       TaskLabels
─────────────       ────────────────────    ──────────────
Id (PK)             ItemsId (FK) ──→        Id (PK)
UserId              LabelsId (FK) ──→       UserId
Title                                       Name
Description                                 LabelColor
Status                                      CreatedAt
Priority                                    UpdatedAt
DueDate                                     IsDeleted
CreatedAt
UpdatedAt
IsDeleted
```
