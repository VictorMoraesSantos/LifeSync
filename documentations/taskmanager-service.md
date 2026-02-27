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

| Migration | Data |
|---|---|
| `20250704233455_initialCreate` | 2025-07-04 |
| `20250704234109_initialCreate1` | 2025-07-04 |
| `20250704234535_initialCreate2` | 2025-07-04 |
| `20250704235010_initialCreate3` | 2025-07-04 |
| `20251025221643_newChanges` | 2025-10-25 |
| `20260106035023_manylabels` | 2026-01-06 |

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
| `Microsoft.EntityFrameworkCore` | 10.0.1 | ORM |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 10.0.1 | Identity |
| `Swashbuckle.AspNetCore` | 10.1.0 | Swagger |

### Referências Internas

| Projeto | Uso |
|---|---|
| `BuildingBlocks` | CQRS, Result, Validation |
| `BuildingBlocks.Messaging` | RabbitMQ (publicar `TaskDueReminderEvent`) |
| `Core.Domain` | BaseEntity, Specification, IRepository |
| `Core.API` | ApiController base |

### Schema do Banco

```
TaskItems           TaskItems_TaskLabels    TaskLabels
─────────────       ────────────────────    ──────────────
Id (PK)             TaskItemId (FK) ──→     Id (PK)
UserId              TaskLabelId (FK) ──→    UserId
Title                                       Name
Description                                 LabelColor
Status                                      CreatedAt
Priority                                    UpdatedAt
DueDate                                     IsDeleted
CreatedAt
UpdatedAt
IsDeleted
```
