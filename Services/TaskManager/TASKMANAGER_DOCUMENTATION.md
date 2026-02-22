# TaskManager Microservice — Documentação Completa

> **Projeto:** LifeSync
> **Serviço:** TaskManager
> **Framework:** ASP.NET Core (.NET 10)
> **Arquitetura:** Clean Architecture + CQRS + DDD
> **Banco de Dados:** PostgreSQL
> **Mensageria:** RabbitMQ

---

## Sumário

1. [Visão Geral](#1-visão-geral)
2. [Estrutura do Projeto](#2-estrutura-do-projeto)
3. [URLs de Acesso](#3-urls-de-acesso)
4. [Endpoints — Task Items](#4-endpoints--task-items)
5. [Endpoints — Task Labels](#5-endpoints--task-labels)
6. [DTOs — Data Transfer Objects](#6-dtos--data-transfer-objects)
7. [Enums](#7-enums)
8. [Entidades de Domínio](#8-entidades-de-domínio)
9. [CQRS — Commands e Queries](#9-cqrs--commands-e-queries)
10. [Repositórios e Filtros](#10-repositórios-e-filtros)
11. [Serviços de Aplicação](#11-serviços-de-aplicação)
12. [Infraestrutura e Banco de Dados](#12-infraestrutura-e-banco-de-dados)
13. [Serviços em Background](#13-serviços-em-background)
14. [Mensageria — Eventos de Domínio](#14-mensageria--eventos-de-domínio)
15. [Padrão de Resposta da API](#15-padrão-de-resposta-da-api)
16. [Tratamento de Erros de Domínio](#16-tratamento-de-erros-de-domínio)
17. [Configuração e Variáveis de Ambiente](#17-configuração-e-variáveis-de-ambiente)
18. [Autenticação e Segurança](#18-autenticação-e-segurança)
19. [Infraestrutura Docker](#19-infraestrutura-docker)
20. [Diagrama de Arquitetura](#20-diagrama-de-arquitetura)

---

## 1. Visão Geral

O **TaskManager** é um microserviço responsável por gerenciar tarefas e etiquetas (labels) de usuários dentro do ecossistema LifeSync. Ele faz parte de uma arquitetura de microserviços, exposto externamente através de um API Gateway baseado em **YARP (Yet Another Reverse Proxy)**.

### Responsabilidades
- Criar, atualizar, deletar e consultar tarefas (`TaskItem`)
- Criar, atualizar, deletar e consultar etiquetas (`TaskLabel`)
- Associar e desassociar etiquetas de tarefas (relação many-to-many)
- Enviar eventos de lembrete de prazo via RabbitMQ

### Padrões e Tecnologias
| Padrão / Tecnologia | Uso |
|---|---|
| Clean Architecture | Separação em camadas: API, Application, Domain, Infrastructure |
| CQRS | Separação de Commands (escrita) e Queries (leitura) |
| Domain-Driven Design (DDD) | Entidades ricas com validações no domínio |
| Repository Pattern | Abstração do acesso a dados |
| Specification Pattern | Lógica de filtragem reutilizável |
| Result Pattern | Retorno explícito de erros sem exceções |
| FluentValidation | Validação dos comandos de entrada |
| YARP | API Gateway com proxy reverso |
| RabbitMQ | Mensageria assíncrona para eventos |
| PostgreSQL + EF Core | Persistência de dados |

---

## 2. Estrutura do Projeto

```
Services/TaskManager/
├── TaskManager.API/                        # Camada de apresentação (Web API)
│   ├── Controllers/
│   │   ├── TaskItemsController.cs
│   │   └── TaskLabelsController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── appsettings.Production.json
│
├── TaskManager.Application/               # Camada de aplicação (CQRS)
│   ├── Contracts/
│   │   ├── ITaskItemService.cs
│   │   └── ITaskLabelService.cs
│   ├── DTOs/
│   │   ├── TaskItem/
│   │   │   ├── TaskItemDTO.cs
│   │   │   ├── CreateTaskItemDTO.cs
│   │   │   ├── UpdateTaskItemDTO.cs
│   │   │   ├── TaskItemFilterDTO.cs
│   │   │   ├── TaskItemSimpleDTO.cs
│   │   │   └── UpdateLabelsDTO.cs
│   │   └── TaskLabel/
│   │       ├── TaskLabelDTO.cs
│   │       ├── CreateTaskLabelDTO.cs
│   │       ├── UpdateTaskLabelDTO.cs
│   │       ├── TaskLabelFilterDTO.cs
│   │       └── TaskLabelSimpleDTO.cs
│   ├── Features/
│   │   ├── TaskItems/
│   │   │   ├── Commands/
│   │   │   │   ├── Create/
│   │   │   │   ├── Update/
│   │   │   │   ├── Delete/
│   │   │   │   ├── AddLabel/
│   │   │   │   └── RemoveLabel/
│   │   │   └── Queries/
│   │   │       ├── GetAll/
│   │   │       ├── GetById/
│   │   │       ├── GetByUser/
│   │   │       └── GetByFilter/
│   │   └── TaskLabels/
│   │       ├── Commands/
│   │       │   ├── Create/
│   │       │   ├── Update/
│   │       │   └── Delete/
│   │       └── Queries/
│   │           ├── GetAll/
│   │           ├── GetById/
│   │           ├── GetByUser/
│   │           └── GetByFilter/
│   ├── Mapping/
│   │   ├── TaskItemMapper.cs
│   │   └── TaskLabelMapper.cs
│   └── DependencyInjection.cs
│
├── TaskManager.Domain/                    # Camada de domínio (DDD)
│   ├── Entities/
│   │   ├── TaskItem.cs
│   │   └── TaskLabel.cs
│   ├── Enums/
│   │   ├── Status.cs
│   │   ├── Priority.cs
│   │   └── LabelColor.cs
│   ├── Errors/
│   │   ├── TaskItemErrors.cs
│   │   └── TaskLabelErrors.cs
│   ├── Events/
│   │   └── TaskDueReminderEvent.cs
│   ├── Filters/
│   │   ├── TaskItemQueryFilter.cs
│   │   ├── TaskLabelQueryFilter.cs
│   │   └── Specifications/
│   │       ├── TaskItemSpecification.cs
│   │       └── TaskLabelSpecification.cs
│   └── Repositories/
│       ├── ITaskItemRepository.cs
│       └── ITaskLabelRepository.cs
│
└── TaskManager.Infrastructure/            # Camada de infraestrutura
    ├── BackgroundServices/
    │   └── DueDateReminderService.cs
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
    └── DependencyInjection.cs
```

---

## 3. URLs de Acesso

### Ambiente de Desenvolvimento (Docker)

| Componente | URL |
|---|---|
| **API Gateway (YARP)** | `http://localhost:6006` |
| **TaskManager (direto, interno)** | `http://taskmanager.api:8080` |
| **TaskManager (via Gateway)** | `http://localhost:6006/taskmanager-service` |
| **Swagger UI (desenvolvimento)** | `http://localhost:8080/swagger` (acesso direto ao serviço) |
| **Health Check** | `http://localhost:6006/taskmanager-service/health` |
| **PgAdmin** | `http://localhost:5050` |
| **RabbitMQ Management** | `http://localhost:15672` |
| **MailHog** | `http://localhost:8025` |

### Roteamento no API Gateway (YARP)

O YARP está configurado para capturar todas as rotas com o prefixo `/taskmanager-service/` e encaminhá-las para o serviço interno, **removendo o prefixo** antes do repasse:

```
REQUISIÇÃO EXTERNA:
  http://localhost:6006/taskmanager-service/api/task-items

  ↓ YARP remove o prefixo /taskmanager-service

REPASSE INTERNO:
  http://taskmanager.api:8080/api/task-items
```

**Configuração da rota no YARP (`appsettings.json`):**
```json
"taskmanager-route": {
  "ClusterId": "taskmanager-cluster",
  "Match": {
    "Path": "/taskmanager-service/{**catch-all}"
  },
  "Transforms": [
    { "PathRemovePrefix": "/taskmanager-service" }
  ]
}

"taskmanager-cluster": {
  "Destinations": {
    "destination1": {
      "Address": "http://taskmanager.api:8080"
    }
  }
}
```

### Base URL para as Requisições (via Gateway)

```
http://localhost:6006/taskmanager-service
```

---

## 4. Endpoints — Task Items

**Base:** `http://localhost:6006/taskmanager-service/api/task-items`

---

### GET `/api/task-items/{id}`
Busca uma tarefa pelo ID.

**URL via Gateway:**
```
GET http://localhost:6006/taskmanager-service/api/task-items/{id}
```

**Path Params:**
| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `id` | `int` | Sim | ID da tarefa |

**Resposta de Sucesso (200):**
```json
{
  "data": {
    "id": 1,
    "createdAt": "2025-07-04T23:34:55Z",
    "updatedAt": null,
    "title": "Estudar Clean Architecture",
    "description": "Ler o livro e implementar exemplos",
    "status": 1,
    "priority": 2,
    "dueDate": "2025-12-31",
    "userId": 42,
    "labels": [
      {
        "id": 3,
        "createdAt": "2025-07-04T23:34:55Z",
        "updatedAt": null,
        "name": "Estudo",
        "labelColor": 2,
        "userId": 42
      }
    ]
  },
  "pagination": null
}
```

**Resposta de Erro (404):**
```json
{
  "error": "Tarefa com id 1 não encontrada."
}
```

---

### GET `/api/task-items/user/{userId}`
Retorna todas as tarefas de um usuário.

**URL via Gateway:**
```
GET http://localhost:6006/taskmanager-service/api/task-items/user/{userId}
```

**Path Params:**
| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `userId` | `int` | Sim | ID do usuário dono das tarefas |

**Resposta de Sucesso (200):**
```json
{
  "data": [
    {
      "id": 1,
      "createdAt": "2025-07-04T23:34:55Z",
      "updatedAt": null,
      "title": "Estudar Clean Architecture",
      "description": "Ler o livro e implementar exemplos",
      "status": 1,
      "priority": 2,
      "dueDate": "2025-12-31",
      "userId": 42,
      "labels": []
    }
  ],
  "pagination": null
}
```

---

### GET `/api/task-items/search`
Busca avançada com filtros, ordenação e paginação.

**URL via Gateway:**
```
GET http://localhost:6006/taskmanager-service/api/task-items/search
```

**Query Params (todos opcionais):**
| Campo | Tipo | Descrição |
|---|---|---|
| `id` | `int?` | Filtrar por ID exato |
| `userId` | `int?` | Filtrar por ID do usuário |
| `titleContains` | `string?` | Busca parcial no título |
| `status` | `int?` | Filtrar por status (1=Pending, 2=InProgress, 3=Completed) |
| `priority` | `int?` | Filtrar por prioridade (1=Low, 2=Medium, 3=High, 4=Urgent) |
| `dueDate` | `string?` | Filtrar por data de vencimento (formato: `yyyy-MM-dd`) |
| `labelId` | `int?` | Filtrar por ID de etiqueta associada |
| `createdAt` | `string?` | Filtrar por data de criação |
| `updatedAt` | `string?` | Filtrar por data de atualização |
| `isDeleted` | `bool?` | Incluir itens deletados (soft delete) |
| `sortBy` | `string?` | Campo para ordenação (ex: `"dueDate"`, `"priority"`) |
| `sortDesc` | `bool?` | Ordenação decrescente (`true`) ou crescente (`false`) |
| `page` | `int?` | Número da página (começa em 1) |
| `pageSize` | `int?` | Itens por página |

**Exemplo de Requisição:**
```
GET /taskmanager-service/api/task-items/search?userId=42&status=1&priority=3&sortBy=dueDate&sortDesc=false&page=1&pageSize=10
```

**Resposta de Sucesso (200):**
```json
{
  "data": [...],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 25,
    "totalPages": 3
  }
}
```

---

### GET `/api/task-items`
Retorna todas as tarefas sem filtro.

**URL via Gateway:**
```
GET http://localhost:6006/taskmanager-service/api/task-items
```

**Resposta de Sucesso (200):**
```json
{
  "data": [...],
  "pagination": null
}
```

---

### POST `/api/task-items`
Cria uma nova tarefa.

**URL via Gateway:**
```
POST http://localhost:6006/taskmanager-service/api/task-items
```

**Content-Type:** `application/json`

**Body (CreateTaskItemCommand):**
```json
{
  "title": "Estudar CQRS",
  "description": "Implementar pattern CQRS no projeto pessoal",
  "priority": 3,
  "dueDate": "2025-12-31",
  "userId": 42,
  "taskLabelsId": [1, 2]
}
```

**Campos do Body:**
| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `title` | `string` | Sim | Título da tarefa (não pode ser vazio) |
| `description` | `string` | Sim | Descrição da tarefa (não pode ser vazio) |
| `priority` | `int` | Sim | Prioridade: `1`=Low, `2`=Medium, `3`=High, `4`=Urgent |
| `dueDate` | `string` | Sim | Data de vencimento no formato `yyyy-MM-dd` (não pode ser no passado) |
| `userId` | `int` | Sim | ID do usuário dono da tarefa |
| `taskLabelsId` | `int[]?` | Não | Lista de IDs de etiquetas a associar |

**Resposta de Sucesso (201):**
```json
{
  "data": {
    "id": 15
  }
}
```

**Resposta de Erro (400):**
```json
{
  "error": "Título da tarefa é obrigatório."
}
```

---

### POST `/api/task-items/batch`
Cria múltiplas tarefas em lote.

**URL via Gateway:**
```
POST http://localhost:6006/taskmanager-service/api/task-items/batch
```

**Content-Type:** `application/json`

**Body:**
```json
[
  {
    "title": "Tarefa 1",
    "description": "Descrição da tarefa 1",
    "priority": 1,
    "dueDate": "2025-12-31",
    "userId": 42,
    "taskLabelsId": null
  },
  {
    "title": "Tarefa 2",
    "description": "Descrição da tarefa 2",
    "priority": 2,
    "dueDate": "2025-11-30",
    "userId": 42,
    "taskLabelsId": [3]
  }
]
```

**Resposta de Sucesso (201):**
```json
{
  "data": {
    "id": 2
  }
}
```
> O campo `id` representa o total de tarefas criadas com sucesso.

**Resposta de Erro Parcial (400):**
```json
{
  "errors": ["Título da tarefa é obrigatório.", "Data de vencimento não pode ser no passado."]
}
```

---

### POST `/api/task-items/{id}/addLabels`
Associa etiquetas a uma tarefa existente.

**URL via Gateway:**
```
POST http://localhost:6006/taskmanager-service/api/task-items/{id}/addLabels
```

**Path Params:**
| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `id` | `int` | Sim | ID da tarefa |

**Body:**
```json
{
  "taskItemId": 0,
  "taskLabelsId": [1, 2, 3]
}
```

> **Nota:** O campo `taskItemId` no body é ignorado; o ID da tarefa é sempre obtido da URL.

**Campos do Body:**
| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `taskLabelsId` | `int[]` | Sim | Lista de IDs de etiquetas a associar |

**Resposta de Sucesso (200):**
```json
{
  "data": true
}
```

---

### POST `/api/task-items/{id}/removeLabels`
Remove associação de etiquetas de uma tarefa.

**URL via Gateway:**
```
POST http://localhost:6006/taskmanager-service/api/task-items/{id}/removeLabels
```

**Path Params:**
| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `id` | `int` | Sim | ID da tarefa |

**Body:**
```json
{
  "taskItemId": 0,
  "taskLabelsId": [1, 2]
}
```

**Resposta de Sucesso (200):**
```json
{
  "data": true
}
```

---

### PUT `/api/task-items/{id}`
Atualiza uma tarefa existente.

**URL via Gateway:**
```
PUT http://localhost:6006/taskmanager-service/api/task-items/{id}
```

**Path Params:**
| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `id` | `int` | Sim | ID da tarefa a atualizar |

**Body (UpdateTaskItemCommand):**
```json
{
  "id": 0,
  "title": "Estudar CQRS Avançado",
  "description": "Aprofundar nos patterns avançados de CQRS",
  "status": 2,
  "priority": 3,
  "dueDate": "2026-01-15"
}
```

> **Nota:** O campo `id` no body é ignorado; o ID é sempre obtido da URL.

**Campos do Body:**
| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `title` | `string` | Sim | Novo título da tarefa |
| `description` | `string` | Sim | Nova descrição |
| `status` | `int` | Sim | Novo status: `1`=Pending, `2`=InProgress, `3`=Completed |
| `priority` | `int` | Sim | Nova prioridade: `1`=Low, `2`=Medium, `3`=High, `4`=Urgent |
| `dueDate` | `string` | Sim | Nova data de vencimento (`yyyy-MM-dd`) |

**Resposta de Sucesso (200):**
```json
{
  "data": "Atualizado com sucesso."
}
```

---

### DELETE `/api/task-items/{id}`
Deleta uma tarefa pelo ID.

**URL via Gateway:**
```
DELETE http://localhost:6006/taskmanager-service/api/task-items/{id}
```

**Path Params:**
| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `id` | `int` | Sim | ID da tarefa a deletar |

**Resposta de Sucesso (200):**
```json
{
  "data": "Deletado com sucesso."
}
```

---

## 5. Endpoints — Task Labels

**Base:** `http://localhost:6006/taskmanager-service/api/task-labels`

---

### GET `/api/task-labels/{id}`
Busca uma etiqueta pelo ID.

**URL via Gateway:**
```
GET http://localhost:6006/taskmanager-service/api/task-labels/{id}
```

**Resposta de Sucesso (200):**
```json
{
  "data": {
    "id": 3,
    "createdAt": "2025-07-04T23:34:55Z",
    "updatedAt": null,
    "name": "Estudo",
    "labelColor": 2,
    "userId": 42
  }
}
```

---

### GET `/api/task-labels/user/{userId}`
Retorna todas as etiquetas de um usuário.

**URL via Gateway:**
```
GET http://localhost:6006/taskmanager-service/api/task-labels/user/{userId}
```

**Resposta de Sucesso (200):**
```json
{
  "data": [
    {
      "id": 3,
      "createdAt": "2025-07-04T23:34:55Z",
      "updatedAt": null,
      "name": "Estudo",
      "labelColor": 2,
      "userId": 42
    }
  ]
}
```

---

### GET `/api/task-labels/search`
Busca avançada de etiquetas com filtros e paginação.

**URL via Gateway:**
```
GET http://localhost:6006/taskmanager-service/api/task-labels/search
```

**Query Params (todos opcionais):**
| Campo | Tipo | Descrição |
|---|---|---|
| `id` | `int?` | Filtrar por ID exato |
| `userId` | `int?` | Filtrar por ID do usuário |
| `itemId` | `int?` | Filtrar etiquetas associadas a um TaskItem específico |
| `nameContains` | `string?` | Busca parcial no nome da etiqueta |
| `labelColor` | `int?` | Filtrar por cor: `0`=Red, `1`=Green, `2`=Blue, etc. |
| `createdAt` | `string?` | Filtrar por data de criação |
| `updatedAt` | `string?` | Filtrar por data de atualização |
| `isDeleted` | `bool?` | Incluir itens deletados |
| `sortBy` | `string?` | Campo para ordenação |
| `sortDesc` | `bool?` | Ordenação decrescente |
| `page` | `int?` | Número da página |
| `pageSize` | `int?` | Itens por página (padrão: 50) |

**Exemplo de Requisição:**
```
GET /taskmanager-service/api/task-labels/search?userId=42&nameContains=estudo&page=1&pageSize=20
```

---

### GET `/api/task-labels`
Retorna todas as etiquetas sem filtro.

**URL via Gateway:**
```
GET http://localhost:6006/taskmanager-service/api/task-labels
```

---

### POST `/api/task-labels`
Cria uma nova etiqueta.

**URL via Gateway:**
```
POST http://localhost:6006/taskmanager-service/api/task-labels
```

**Content-Type:** `application/json`

**Body (CreateTaskLabelCommand):**
```json
{
  "name": "Trabalho",
  "labelColor": 0,
  "userId": 42
}
```

**Campos do Body:**
| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `name` | `string` | Sim | Nome da etiqueta (não pode ser vazio) |
| `labelColor` | `int` | Sim | Cor da etiqueta (ver enum `LabelColor`) |
| `userId` | `int` | Sim | ID do usuário dono da etiqueta |

**Resposta de Sucesso (201):**
```json
{
  "data": {
    "id": 7
  }
}
```

---

### POST `/api/task-labels/batch`
Cria múltiplas etiquetas em lote.

**URL via Gateway:**
```
POST http://localhost:6006/taskmanager-service/api/task-labels/batch
```

**Body:**
```json
[
  { "name": "Trabalho", "labelColor": 0, "userId": 42 },
  { "name": "Pessoal", "labelColor": 1, "userId": 42 },
  { "name": "Urgente", "labelColor": 5, "userId": 42 }
]
```

**Resposta de Sucesso (201):**
```json
{
  "data": {
    "id": 3
  }
}
```
> O campo `id` representa o total de etiquetas criadas.

---

### PUT `/api/task-labels/{id}`
Atualiza uma etiqueta existente.

**URL via Gateway:**
```
PUT http://localhost:6006/taskmanager-service/api/task-labels/{id}
```

**Body (UpdateTaskLabelCommand):**
```json
{
  "id": 0,
  "name": "Trabalho Remoto",
  "labelColor": 3
}
```

> **Nota:** O campo `id` no body é ignorado; o ID é sempre obtido da URL.

**Campos do Body:**
| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `name` | `string` | Sim | Novo nome da etiqueta |
| `labelColor` | `int` | Sim | Nova cor (ver enum `LabelColor`) |

**Resposta de Sucesso (200):**
```json
{
  "data": "Atualizado com sucesso."
}
```

---

### DELETE `/api/task-labels/{id}`
Deleta uma etiqueta pelo ID.

**URL via Gateway:**
```
DELETE http://localhost:6006/taskmanager-service/api/task-labels/{id}
```

**Resposta de Sucesso (200):**
```json
{
  "data": "Deletado com sucesso."
}
```

---

## 6. DTOs — Data Transfer Objects

### 6.1 TaskItem DTOs

#### `TaskItemDTO` (Response — leitura)
Retornado nas consultas de tarefas.

```csharp
public record TaskItemDTO(
    int Id,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Title,
    string Description,
    Status Status,
    Priority Priority,
    DateOnly DueDate,
    int UserId,
    List<TaskLabelDTO> Labels
) : DTOBase(Id, CreatedAt, UpdatedAt);
```

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | `int` | Identificador único da tarefa |
| `createdAt` | `DateTime` | Data/hora de criação (UTC) |
| `updatedAt` | `DateTime?` | Data/hora da última atualização (nullable) |
| `title` | `string` | Título da tarefa |
| `description` | `string` | Descrição detalhada |
| `status` | `int` | Status atual (ver enum `Status`) |
| `priority` | `int` | Prioridade (ver enum `Priority`) |
| `dueDate` | `string` | Data de vencimento no formato `yyyy-MM-dd` |
| `userId` | `int` | ID do usuário dono |
| `labels` | `TaskLabelDTO[]` | Lista de etiquetas associadas |

---

#### `CreateTaskItemDTO` / `CreateTaskItemCommand` (Request — criação)
Utilizado na criação de tarefas.

```csharp
public record CreateTaskItemDTO(
    string Title,
    string Description,
    Priority Priority,
    DateOnly DueDate,
    int UserId,
    List<int>? TaskLabelsId
);
```

| Campo | Tipo | Obrigatório | Validação |
|---|---|---|---|
| `title` | `string` | Sim | Não pode ser nulo ou vazio |
| `description` | `string` | Sim | Não pode ser nulo ou vazio |
| `priority` | `int` | Sim | Deve ser valor válido do enum `Priority` |
| `dueDate` | `string` | Sim | Não pode ser data passada |
| `userId` | `int` | Sim | — |
| `taskLabelsId` | `int[]?` | Não | IDs de labels existentes |

---

#### `UpdateTaskItemDTO` / `UpdateTaskItemCommand` (Request — atualização)

```csharp
public record UpdateTaskItemDTO(
    int Id,
    string Title,
    string Description,
    Status Status,
    Priority Priority,
    DateOnly DueDate
);
```

| Campo | Tipo | Obrigatório | Validação |
|---|---|---|---|
| `id` | `int` | Sim (via URL) | ID da tarefa a atualizar |
| `title` | `string` | Sim | Não pode ser nulo ou vazio |
| `description` | `string` | Sim | Não pode ser nulo ou vazio |
| `status` | `int` | Sim | Deve ser valor válido do enum `Status` |
| `priority` | `int` | Sim | Deve ser valor válido do enum `Priority` |
| `dueDate` | `string` | Sim | Não pode ser data passada |

---

#### `TaskItemFilterDTO` (Query Params — filtragem)

```csharp
public record TaskItemFilterDTO(
    int? Id,
    int? UserId,
    string? TitleContains,
    Status? Status,
    Priority? Priority,
    DateOnly? DueDate,
    int? LabelId,
    DateOnly? CreatedAt,
    DateOnly? UpdatedAt,
    bool? IsDeleted,
    string? SortBy,
    bool? SortDesc,
    int? Page,
    int? PageSize
) : DomainQueryFilterDTO(...);
```

Todos os campos são opcionais e usados como filtros cumulativos (AND).

---

#### `UpdateLabelsDTO` / `AddLabelCommand` / `RemoveLabelCommand` (Request — labels)

```csharp
public record UpdateLabelsDTO(int TaskItemId, List<int> TaskLabelsId);
```

| Campo | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `taskItemId` | `int` | Ignorado | Substituído pelo ID da URL |
| `taskLabelsId` | `int[]` | Sim | Lista de IDs de etiquetas |

---

### 6.2 TaskLabel DTOs

#### `TaskLabelDTO` (Response — leitura)

```csharp
public record TaskLabelDTO(
    int Id,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Name,
    LabelColor LabelColor,
    int UserId
) : DTOBase(Id, CreatedAt, UpdatedAt);
```

| Campo | Tipo | Descrição |
|---|---|---|
| `id` | `int` | Identificador único da etiqueta |
| `createdAt` | `DateTime` | Data/hora de criação (UTC) |
| `updatedAt` | `DateTime?` | Data/hora da última atualização (nullable) |
| `name` | `string` | Nome da etiqueta |
| `labelColor` | `int` | Cor da etiqueta (ver enum `LabelColor`) |
| `userId` | `int` | ID do usuário dono |

---

#### `CreateTaskLabelDTO` / `CreateTaskLabelCommand` (Request — criação)

```csharp
public record CreateTaskLabelDTO(
    string Name,
    LabelColor LabelColor,
    int UserId
);
```

| Campo | Tipo | Obrigatório | Validação |
|---|---|---|---|
| `name` | `string` | Sim | Não pode ser nulo ou vazio; é trimado |
| `labelColor` | `int` | Sim | Deve ser valor válido do enum `LabelColor` |
| `userId` | `int` | Sim | — |

---

#### `UpdateTaskLabelDTO` / `UpdateTaskLabelCommand` (Request — atualização)

```csharp
public record UpdateTaskLabelDTO(int Id, string Name, LabelColor LabelColor);
```

| Campo | Tipo | Obrigatório | Validação |
|---|---|---|---|
| `id` | `int` | Sim (via URL) | — |
| `name` | `string` | Sim | Não pode ser nulo ou vazio |
| `labelColor` | `int` | Sim | Deve ser valor válido do enum `LabelColor` |

---

#### `TaskLabelFilterDTO` (Query Params — filtragem)

```csharp
public record TaskLabelFilterDTO(
    int? Id,
    int? UserId,
    int? ItemId,
    string? NameContains,
    LabelColor? LabelColor,
    DateOnly? CreatedAt,
    DateOnly? UpdatedAt,
    bool? IsDeleted,
    string? SortBy,
    bool? SortDesc,
    int? Page,
    int? PageSize
) : DomainQueryFilterDTO(...);
```

> **Padrão de paginação:** `PageSize` padrão = 50.

---

## 7. Enums

### `Status` — Status da Tarefa

| Valor | Nome | Descrição |
|---|---|---|
| `1` | `Pending` | Pendente |
| `2` | `InProgress` | Em progresso |
| `3` | `Completed` | Completado |

---

### `Priority` — Prioridade da Tarefa

| Valor | Nome | Descrição |
|---|---|---|
| `1` | `Low` | Baixa |
| `2` | `Medium` | Média |
| `3` | `High` | Alta |
| `4` | `Urgent` | Urgente |

---

### `LabelColor` — Cor da Etiqueta

| Valor | Nome | Hex |
|---|---|---|
| `0` | `Red` | `#FF0000` |
| `1` | `Green` | `#00FF00` |
| `2` | `Blue` | `#0000FF` |
| `3` | `Yellow` | `#FFFF00` |
| `4` | `Purple` | `#800080` |
| `5` | `Orange` | `#FFA500` |
| `6` | `Pink` | `#FFC0CB` |
| `7` | `Brown` | `#A52A2A` |
| `8` | `Gray` | `#808080` |

---

## 8. Entidades de Domínio

### `TaskItem`

Entidade raiz do agregado de tarefas. Herda de `BaseEntity<int>`.

```
Tabela: TaskItems
```

| Propriedade | Tipo | Coluna | Restrições |
|---|---|---|---|
| `Id` | `int` | `Id` | PK, auto-increment |
| `UserId` | `int` | `UserId` | NOT NULL, indexed |
| `Title` | `string` | `Title` | VARCHAR(200), NOT NULL |
| `Description` | `string` | `Description` | VARCHAR(1000), NOT NULL |
| `Status` | `Status` (int) | `Status` | NOT NULL, indexed |
| `Priority` | `Priority` (int) | `Priority` | NOT NULL |
| `DueDate` | `DateOnly` | `DueDate` | DATE, NOT NULL, indexed |
| `IsDeleted` | `bool` | `IsDeleted` | DEFAULT false, indexed |
| `CreatedAt` | `DateTime` | `CreatedAt` | NOT NULL |
| `UpdatedAt` | `DateTime?` | `UpdatedAt` | nullable |
| `Labels` | `IReadOnlyCollection<TaskLabel>` | — | N:N via junction table |

**Regras de Negócio:**
- `Title` não pode ser nulo ou vazio
- `Description` não pode ser nulo ou vazio
- `Priority` deve ser um valor válido do enum
- `DueDate` não pode ser uma data passada
- Labels não podem ser nulas, duplicadas, ou inexistentes

**Métodos do Domínio:**

| Método | Descrição |
|---|---|
| `Update(title, description, status, priority, dueDate)` | Atualiza a tarefa |
| `SetTitle(title)` | Define título com validação |
| `SetDescription(description)` | Define descrição com validação |
| `SetPriority(priority)` | Define prioridade com validação |
| `SetDueDate(dueDate)` | Define data com validação |
| `ChangeStatus(status)` | Muda o status |
| `AddLabel(label)` | Associa etiqueta |
| `RemoveLabel(label)` | Desassocia etiqueta |
| `IsOverdue()` | Verifica se está atrasada |
| `IsComplete()` | Verifica se está concluída |
| `HasLabel(labelId)` | Verifica se possui uma etiqueta |

---

### `TaskLabel`

Entidade de etiqueta. Herda de `BaseEntity<int>`.

```
Tabela: TaskLabels
```

| Propriedade | Tipo | Coluna | Restrições |
|---|---|---|---|
| `Id` | `int` | `Id` | PK, auto-increment |
| `UserId` | `int` | `UserId` | NOT NULL |
| `Name` | `string` | `Name` | VARCHAR, NOT NULL |
| `LabelColor` | `LabelColor` (int) | `LabelColor` | NOT NULL |
| `IsDeleted` | `bool` | `IsDeleted` | DEFAULT false |
| `CreatedAt` | `DateTime` | `CreatedAt` | NOT NULL |
| `UpdatedAt` | `DateTime?` | `UpdatedAt` | nullable |
| `Items` | `IReadOnlyCollection<TaskItem>` | — | N:N via junction table |

**Regras de Negócio:**
- `Name` não pode ser nulo ou vazio
- `Name` é trimado automaticamente antes de salvar

**Métodos do Domínio:**

| Método | Descrição |
|---|---|
| `Update(name, labelColor)` | Atualiza nome e cor com validação |
| `AddTaskItem(item)` | Associa tarefa |
| `RemoveTaskItem(item)` | Desassocia tarefa |

---

## 9. CQRS — Commands e Queries

### 9.1 Commands (escrita)

#### TaskItem Commands

| Command | Input | Output | Descrição |
|---|---|---|---|
| `CreateTaskItemCommand` | `(Title, Description, Priority, DueDate, UserId, TaskLabelsId?)` | `CreateTaskItemResult(Id)` | Cria nova tarefa |
| `UpdateTaskItemCommand` | `(Id, Title, Description, Status, Priority, DueDate)` | `UpdateTaskItemResult(IsUpdated)` | Atualiza tarefa |
| `DeleteTaskItemCommand` | `(Id)` | `DeleteTaskItemResult(IsDeleted)` | Deleta tarefa |
| `AddLabelCommand` | `(TaskItemId, TaskLabelsId[])` | `AddLabelResult(IsSuccess)` | Associa labels |
| `RemoveLabelCommand` | `(TaskItemId, TaskLabelsId[])` | `RemoveLabelResult(IsSuccess)` | Desassocia labels |

#### TaskLabel Commands

| Command | Input | Output | Descrição |
|---|---|---|---|
| `CreateTaskLabelCommand` | `(Name, LabelColor, UserId)` | `CreateTaskLabelResult(Id)` | Cria nova etiqueta |
| `UpdateTaskLabelCommand` | `(Id, Name, LabelColor)` | `UpdateTaskLabelResult(IsUpdated)` | Atualiza etiqueta |
| `DeleteTaskLabelCommand` | `(Id)` | `DeleteTaskLabelResult(IsDeleted)` | Deleta etiqueta |

---

### 9.2 Queries (leitura)

#### TaskItem Queries

| Query | Input | Output | Descrição |
|---|---|---|---|
| `GetTaskItemByIdQuery` | `(Id)` | `GetTaskItemByIdResult(TaskItem: TaskItemDTO)` | Busca por ID |
| `GetByUserQuery` | `(UserId)` | Coleção de `TaskItemDTO` | Busca por usuário |
| `GetTaskItemsByFilterQuery` | `(Filter: TaskItemFilterDTO)` | `GetTaskItemsByFilterResult(Items, Pagination)` | Busca com filtros |
| `GetAllTaskItemsQuery` | — | `GetAllTaskItemsResult(TaskItems)` | Retorna tudo |

#### TaskLabel Queries

| Query | Input | Output | Descrição |
|---|---|---|---|
| `GetTaskLabelByIdQuery` | `(Id)` | `GetTaskLabelByIdResult(TaskLabel: TaskLabelDTO)` | Busca por ID |
| `GetByUserQuery` | `(UserId)` | Coleção de `TaskLabelDTO` | Busca por usuário |
| `GetTaskLabelsByFilterQuery` | `(Filter: TaskLabelFilterDTO)` | `GetTaskLabelsByFilterResult(TaskLabels, Pagination)` | Busca com filtros |
| `GetAllTaskLabelsQuery` | — | `GetAllTaskLabelsResult(TaskLabels)` | Retorna tudo |

---

## 10. Repositórios e Filtros

### Interfaces

#### `ITaskItemRepository`
```csharp
public interface ITaskItemRepository : IRepository<TaskItem, int, TaskItemQueryFilter>
```

Herda todas as operações CRUD:
- `GetById(int id)` — carrega com labels
- `GetAll()` — carrega com labels
- `Find(predicate)` — LINQ com labels
- `FindByFilter(TaskItemQueryFilter filter)` — usa Specification Pattern
- `Create(TaskItem)` — insere e salva
- `CreateRange(IEnumerable<TaskItem>)` — inserção em lote
- `Update(TaskItem)` — atualiza e salva
- `Delete(TaskItem)` — deleta e salva

#### `ITaskLabelRepository`
```csharp
public interface ITaskLabelRepository : IRepository<TaskLabel, int, TaskLabelQueryFilter>
```
Mesma estrutura do `ITaskItemRepository`.

---

### Filtros de Domínio

#### `TaskItemQueryFilter`

Herda de `DomainQueryFilter`. Campos de filtragem:

| Campo | Tipo | Operação |
|---|---|---|
| `Id` | `int?` | Igualdade exata |
| `UserId` | `int?` | Igualdade exata |
| `TitleContains` | `string?` | LIKE parcial |
| `Status` | `Status?` | Igualdade exata |
| `Priority` | `Priority?` | Igualdade exata |
| `DueDate` | `DateOnly?` | Igualdade exata |
| `LabelId` | `int?` | Filtro por label associada |
| `CreatedAt` | `DateOnly?` | Range de data |
| `UpdatedAt` | `DateOnly?` | Range de data |
| `IsDeleted` | `bool?` | Soft delete |
| `SortBy` | `string?` | Nome da coluna |
| `SortDesc` | `bool?` | Direção |
| `Page` | `int?` | Página |
| `PageSize` | `int?` | Tamanho da página |

#### `TaskLabelQueryFilter`

| Campo | Tipo | Operação |
|---|---|---|
| `Id` | `int?` | Igualdade exata |
| `UserId` | `int?` | Igualdade exata |
| `TaskItemId` | `int?` | Filtro por tarefa associada |
| `NameContains` | `string?` | LIKE parcial |
| `LabelColor` | `LabelColor?` | Igualdade exata |
| `CreatedAt` | `DateOnly?` | Range de data |
| `IsDeleted` | `bool?` | Soft delete |
| `SortBy`, `SortDesc`, `Page`, `PageSize` | — | Paginação e ordenação |

> **PageSize padrão:** 50

---

## 11. Serviços de Aplicação

### `ITaskItemService`

```csharp
public interface ITaskItemService :
    IReadService<TaskItemDTO, int, TaskItemFilterDTO>,
    ICreateService<CreateTaskItemDTO>,
    IUpdateService<UpdateTaskItemDTO>,
    IDeleteService<int>
{
    Task<Result<bool>> AddLabelAsync(UpdateLabelsDTO dto, CancellationToken cancellationToken);
    Task<Result<bool>> RemoveLabelAsync(UpdateLabelsDTO dto, CancellationToken cancellationToken);
}
```

**Métodos disponíveis:**

| Método | Retorno | Descrição |
|---|---|---|
| `GetByIdAsync(int id)` | `Result<TaskItemDTO>` | Busca por ID com labels |
| `GetAllAsync()` | `Result<IEnumerable<TaskItemDTO>>` | Busca tudo |
| `GetPagedAsync(page, pageSize)` | `Result<IEnumerable<TaskItemDTO>>` | Paginado |
| `GetByFilterAsync(filter)` | `Result<(Items, Pagination)>` | Filtros avançados |
| `FindAsync(predicate)` | `Result<IEnumerable<TaskItemDTO>>` | LINQ |
| `CountAsync(predicate?)` | `Result<int>` | Contagem |
| `CreateAsync(CreateTaskItemDTO)` | `Result<int>` | Cria tarefa, retorna ID |
| `CreateRangeAsync(IEnumerable<CreateTaskItemDTO>)` | `Result<bool>` | Criação em lote |
| `UpdateAsync(UpdateTaskItemDTO)` | `Result<bool>` | Atualiza tarefa |
| `AddLabelAsync(UpdateLabelsDTO)` | `Result<bool>` | Associa labels |
| `RemoveLabelAsync(UpdateLabelsDTO)` | `Result<bool>` | Desassocia labels |
| `DeleteAsync(int id)` | `Result<bool>` | Deleta tarefa |
| `DeleteRangeAsync(IEnumerable<int>)` | `Result<bool>` | Deleção em lote |

---

### `ITaskLabelService`

```csharp
public interface ITaskLabelService :
    IReadService<TaskLabelDTO, int, TaskLabelFilterDTO>,
    ICreateService<CreateTaskLabelDTO>,
    IUpdateService<UpdateTaskLabelDTO>,
    IDeleteService<int>
```

Mesma estrutura de métodos CRUD do `ITaskItemService` (sem métodos de label association).

---

## 12. Infraestrutura e Banco de Dados

### `ApplicationDbContext`

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<TaskLabel> TaskLabels { get; set; }
}
```

**Banco de dados:** PostgreSQL
**String de conexão (desenvolvimento):**
```
Server=lifesyncdb;Port=5432;Database=LifeSyncDB;User Id=postgres;Password=postgres;Include Error Detail=true
```

### Histórico de Migrations

| Migration | Data | Descrição |
|---|---|---|
| `20250704233455_initialCreate` | 04/07/2025 | Schema inicial |
| `20250704233455_initialCreate1` | 04/07/2025 | Ajuste inicial |
| `20250704234535_initialCreate2` | 04/07/2025 | Ajuste de schema |
| `20250704235010_initialCreate3` | 04/07/2025 | Finalização do schema inicial |
| `20251025221643_newChanges` | 25/10/2025 | Modificações de schema |
| `20260106035023_manylabels` | 06/01/2026 | Suporte many-to-many TaskItem ↔ TaskLabel |

### Dependency Injection (Infrastructure)

```csharp
services.AddDbContext(configuration);         // PostgreSQL + EF Core
services.AddMessaging(configuration);         // RabbitMQ

// Repositories (Scoped)
services.AddScoped<ITaskItemRepository, TaskItemRepository>();
services.AddScoped<ITaskLabelRepository, TaskLabelRepository>();

// Services (Scoped)
services.AddScoped<ITaskItemService, TaskItemService>();
services.AddScoped<ITaskLabelService, TaskLabelService>();

// Background Services
services.AddHostedService<MigrationHostedService>();
services.AddHostedService<DueDateReminderService>();
```

---

## 13. Serviços em Background

### `DueDateReminderService`

Serviço de background responsável por detectar tarefas com prazo próximo e publicar eventos de lembrete.

**Funcionamento:**
1. Executa em loop com intervalo configurável (padrão: **1 hora**)
2. Busca tarefas com `DueDate` dentro do limiar configurado (padrão: **1 dia**)
3. Para cada tarefa encontrada, publica um `TaskDueReminderEvent` no RabbitMQ
4. O serviço de Notificação consome o evento e envia o lembrete ao usuário

**Configuração (`DueDateReminderOptions`):**

| Opção | Padrão | Descrição |
|---|---|---|
| `ReminderThreshold` | 1 dia | Antecedência para lembrete |
| `PollingInterval` | 1 hora | Frequência de verificação |
| `ExchangeName` | `task_exchange` | Exchange do RabbitMQ |
| `RoutingKey` | `task.due.reminder` | Routing key da mensagem |
| `MaxTasksPerRun` | 100 | Máximo de tarefas por ciclo |

---

### `MigrationHostedService`

Executa automaticamente as migrations do Entity Framework Core na inicialização do serviço. Garante que o schema do banco esteja sempre atualizado ao subir o container.

---

## 14. Mensageria — Eventos de Domínio

### `TaskDueReminderEvent`

Evento de integração publicado quando uma tarefa está próxima do prazo.

```csharp
public class TaskDueReminderEvent : IntegrationEvent
{
    public int TaskId { get; }
    public int UserId { get; }
    public string TaskTitle { get; }
    public DateOnly DueDate { get; }
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `taskId` | `int` | ID da tarefa |
| `userId` | `int` | ID do usuário responsável |
| `taskTitle` | `string` | Título da tarefa |
| `dueDate` | `string` | Data de vencimento |

**Publisher:** `DueDateReminderService` (TaskManager)
**Consumer:** `Notification.API` (serviço de notificação)
**Exchange:** `task_exchange`
**Routing Key:** `task.due.reminder`

---

## 15. Padrão de Resposta da API

Todas as respostas seguem o wrapper `HttpResult<object>`:

### Resposta de Sucesso — Dados

```json
{
  "data": { ... },
  "pagination": null
}
```

### Resposta de Sucesso — Lista com Paginação

```json
{
  "data": [ ... ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 50,
    "totalPages": 5
  }
}
```

### Resposta de Criação (201)

```json
{
  "data": {
    "id": 15
  }
}
```

### Resposta de Atualização (200)

```json
{
  "data": "Atualizado com sucesso."
}
```

### Resposta de Deleção (200)

```json
{
  "data": "Deletado com sucesso."
}
```

### Resposta de Erro — Not Found (404)

```json
{
  "error": "Tarefa com id 99 não encontrada."
}
```

### Resposta de Erro — Bad Request (400)

```json
{
  "error": "Título da tarefa é obrigatório."
}
```

### Códigos de Status HTTP

| Código | Uso |
|---|---|
| `200 OK` | Leitura ou operação de sucesso sem criação |
| `201 Created` | Recurso criado com sucesso |
| `400 Bad Request` | Dados inválidos ou regra de negócio violada |
| `404 Not Found` | Recurso não encontrado |

---

## 16. Tratamento de Erros de Domínio

### `TaskItemErrors`

| Erro | Quando ocorre |
|---|---|
| `InvalidTitle` | Título nulo ou vazio |
| `InvalidDescription` | Descrição nula ou vazia |
| `InvalidPriority` | Valor de prioridade inválido |
| `DueDateInPast` | Data de vencimento no passado |
| `NullLabel` | Label nula passada como argumento |
| `DuplicateLabel` | Label já associada à tarefa |
| `LabelNotFound` | Label não associada à tarefa |
| `NotFound(id)` | Tarefa não encontrada pelo ID |
| `CreateError` | Falha genérica na criação |
| `UpdateError` | Falha genérica na atualização |
| `DeleteError` | Falha genérica na deleção |

### `TaskLabelErrors`

| Erro | Quando ocorre |
|---|---|
| `InvalidName` | Nome nulo ou vazio |
| `InvalidPagination` | Parâmetros de paginação inválidos |
| `NotFound(id)` | Etiqueta não encontrada |
| `SomeNotFound(ids)` | Algumas etiquetas não encontradas |
| `AllNotFound` | Nenhuma etiqueta encontrada |
| `EmptyOrNullList` | Lista de IDs vazia ou nula |
| `InvalidIds` | IDs inválidos na lista |
| `CreateError` | Falha na criação |
| `UpdateError` | Falha na atualização |
| `DeleteError` | Falha na deleção |

---

## 17. Configuração e Variáveis de Ambiente

### `appsettings.json` (TaskManager.API)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=LifeSync;Include Error Detail=true;"
  },
  "RabbitMQSettings": {
    "Host": "rabbitmq",
    "User": "guest",
    "Password": "guest",
    "Port": 5672
  },
  "JwtSettings": {
    "Key": "SuperSecretKeyForJWTAuthentication2024!@#$%",
    "Issuer": "LifeSyncAPI",
    "Audience": "LifeSyncApp",
    "ExpiryMinutes": 60
  }
}
```

### Variáveis de Ambiente (Docker)

| Variável | Valor (desenvolvimento) | Descrição |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Development` | Ambiente de execução |
| `ASPNETCORE_URLS` | `http://+:8080` | Porta interna do serviço |
| `ConnectionStrings__DefaultConnection` | `Server=lifesyncdb;Port=5432;...` | String de conexão PostgreSQL |
| `RabbitMQSettings__Host` | `rabbitmq` | Host do RabbitMQ |
| `RabbitMQSettings__Port` | `5672` | Porta do RabbitMQ |
| `RabbitMQSettings__User` | `guest` | Usuário RabbitMQ |
| `RabbitMQSettings__Password` | `guest` | Senha RabbitMQ |

---

## 18. Autenticação e Segurança

### JWT Authentication

A autenticação JWT é configurada via `BuildingBlocks.Authentication`:

```csharp
builder.Services.AddJwtAuthentication(builder.Configuration);
```

**Configuração JWT:**

| Parâmetro | Valor |
|---|---|
| `Issuer` | `LifeSyncAPI` |
| `Audience` | `LifeSyncApp` |
| `ExpiryMinutes` | `60` |

### API Gateway — Autenticação

A rota `taskmanager-route` no YARP **não possui** política de autenticação obrigatória no momento (comentada no código):

```json
// "AuthorizationPolicy": "RequireAuthentication"
```

> **Nota:** A autenticação pode ser habilitada descomentando a linha `AuthorizationPolicy` no `appsettings.json` do YARP.

### Validação de Input

Todos os commands possuem validadores FluentValidation registrados automaticamente via reflection:

- `CreateTaskItemCommandValidator`
- `UpdateTaskItemCommandValidator`
- `AddLabelCommandValidator`
- `RemoveLabelCommandValidator`
- `CreateTaskLabelCommandValidator`
- `UpdateTaskLabelCommandValidator`

Erros de validação são interceptados pelo middleware `UseValidationExceptionHandling()` e retornados como `400 Bad Request`.

---

## 19. Infraestrutura Docker

### Serviços do Docker Compose

| Serviço | Imagem | Porta Externa | Porta Interna |
|---|---|---|---|
| `lifesyncdb` | `postgres:18` | `5433` | `5432` |
| `pgadmin` | `dpage/pgadmin4` | `5050` | `80` |
| `rabbitmq` | `rabbitmq:management` | `5672`, `15672` | `5672`, `15672` |
| `mailhog` | `mailhog/mailhog` | `1025`, `8025` | `1025`, `8025` |
| `taskmanager.api` | build local | — | `8080` |
| `yarpapigateway` | build local | `6006` | `8080` |
| `lifesyncapp.webapp` | build local | `6007` | `8080` |

### Dependências do TaskManager

```yaml
taskmanager.api:
  depends_on:
    lifesyncdb:
      condition: service_healthy
    rabbitmq:
      condition: service_healthy
  restart: on-failure
```

### Credenciais de Desenvolvimento

| Serviço | Usuário | Senha |
|---|---|---|
| PostgreSQL | `postgres` | `postgres` |
| PgAdmin | `admin@admin.com` | `admin` |
| RabbitMQ | `guest` | `guest` |
| MailHog | — | — |

---

## 20. Diagrama de Arquitetura

```
┌──────────────────────────────────────────────────────────────────────┐
│                        CLIENTE (WebApp / Browser)                    │
│                     http://localhost:6007                            │
└────────────────────────────┬─────────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────────┐
│                    YARP API GATEWAY                                  │
│                    http://localhost:6006                             │
│                                                                      │
│  /taskmanager-service/*  ──────────────────────►  taskmanager.api   │
│  /nutrition-service/*    ──────────────────────►  nutrition.api      │
│  /financial-service/*    ──────────────────────►  financial.api      │
│  /users-service/*        ──────────────────────►  users.api          │
│  /gym-service/*          ──────────────────────►  gym.api            │
│  /auth/*                 ──────────────────────►  users.api          │
└──────────────────────────────────────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────────┐
│                     TASKMANAGER MICROSERVICE                         │
│                     http://taskmanager.api:8080                      │
│                                                                      │
│  ┌─────────────────┐   ┌──────────────────────────────────────────┐ │
│  │   API Layer     │   │          Application Layer               │ │
│  │                 │   │                                          │ │
│  │ TaskItems       │──►│ CreateTaskItem  │ GetTaskItemById        │ │
│  │ Controller      │   │ UpdateTaskItem  │ GetByUser              │ │
│  │                 │   │ DeleteTaskItem  │ GetByFilter            │ │
│  │ TaskLabels      │──►│ AddLabel        │ GetAll                 │ │
│  │ Controller      │   │ RemoveLabel     │                        │ │
│  └─────────────────┘   │                                          │ │
│                        │ CreateTaskLabel │ GetTaskLabelById       │ │
│                        │ UpdateTaskLabel │ GetByUser              │ │
│                        │ DeleteTaskLabel │ GetByFilter            │ │
│                        └──────────────────────────────────────────┘ │
│                                         │                            │
│                        ┌────────────────▼─────────────────────────┐ │
│                        │           Domain Layer                    │ │
│                        │  TaskItem Entity  │  TaskLabel Entity     │ │
│                        │  Status Enum      │  Priority Enum        │ │
│                        │  LabelColor Enum  │  Domain Errors        │ │
│                        └────────────────┬─────────────────────────┘ │
│                                         │                            │
│                        ┌────────────────▼─────────────────────────┐ │
│                        │        Infrastructure Layer              │ │
│                        │  TaskItemRepository  (PostgreSQL)        │ │
│                        │  TaskLabelRepository (PostgreSQL)        │ │
│                        │  DueDateReminderService (Background)     │ │
│                        │  MigrationHostedService (Background)     │ │
│                        └────────────────┬─────────────────────────┘ │
└─────────────────────────────────────────┼────────────────────────────┘
                                          │
              ┌───────────────────────────┼───────────────────────┐
              │                           │                       │
              ▼                           ▼                       ▼
┌─────────────────────┐   ┌─────────────────────────┐   ┌────────────────────┐
│     PostgreSQL      │   │       RabbitMQ           │   │  Notification API  │
│  lifesyncdb:5432    │   │    rabbitmq:5672          │   │  (Consumer)        │
│  Database:LifeSyncDB│   │  Exchange: task_exchange  │   │                    │
│                     │   │  Key: task.due.reminder   │   │  Envia emails de   │
│  Tables:            │   │                           │   │  lembrete via      │
│  - TaskItems        │   │  Publica:                 │   │  MailHog           │
│  - TaskLabels       │   │  TaskDueReminderEvent     │   │                    │
└─────────────────────┘   └─────────────────────────┘   └────────────────────┘
```

---

## Referência Rápida de Endpoints

### Task Items

| Método | URL via Gateway | Descrição |
|---|---|---|
| `GET` | `/taskmanager-service/api/task-items` | Listar todas as tarefas |
| `GET` | `/taskmanager-service/api/task-items/{id}` | Buscar tarefa por ID |
| `GET` | `/taskmanager-service/api/task-items/user/{userId}` | Tarefas do usuário |
| `GET` | `/taskmanager-service/api/task-items/search` | Busca avançada com filtros |
| `POST` | `/taskmanager-service/api/task-items` | Criar tarefa |
| `POST` | `/taskmanager-service/api/task-items/batch` | Criar tarefas em lote |
| `POST` | `/taskmanager-service/api/task-items/{id}/addLabels` | Associar labels |
| `POST` | `/taskmanager-service/api/task-items/{id}/removeLabels` | Desassociar labels |
| `PUT` | `/taskmanager-service/api/task-items/{id}` | Atualizar tarefa |
| `DELETE` | `/taskmanager-service/api/task-items/{id}` | Deletar tarefa |

### Task Labels

| Método | URL via Gateway | Descrição |
|---|---|---|
| `GET` | `/taskmanager-service/api/task-labels` | Listar todas as etiquetas |
| `GET` | `/taskmanager-service/api/task-labels/{id}` | Buscar etiqueta por ID |
| `GET` | `/taskmanager-service/api/task-labels/user/{userId}` | Etiquetas do usuário |
| `GET` | `/taskmanager-service/api/task-labels/search` | Busca avançada com filtros |
| `POST` | `/taskmanager-service/api/task-labels` | Criar etiqueta |
| `POST` | `/taskmanager-service/api/task-labels/batch` | Criar etiquetas em lote |
| `PUT` | `/taskmanager-service/api/task-labels/{id}` | Atualizar etiqueta |
| `DELETE` | `/taskmanager-service/api/task-labels/{id}` | Deletar etiqueta |

### Utilitários

| Método | URL via Gateway | Descrição |
|---|---|---|
| `GET` | `/taskmanager-service/health` | Health check do serviço |

---

*Documentação gerada em 22/02/2026 — LifeSync TaskManager Microservice*
