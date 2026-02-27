# LifeSync

Plataforma de produtividade e bem-estar pessoal construída com arquitetura de microserviços. O LifeSync centraliza gerenciamento de tarefas, nutrição, finanças, treinos físicos e notificações em um único ecossistema integrado.

---

## Sumário

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Microserviços](#microserviços)
- [Bibliotecas Compartilhadas](#bibliotecas-compartilhadas)
- [Infraestrutura](#infraestrutura)
- [Documentação Detalhada](#documentação-detalhada)
- [Como Executar](#como-executar)
- [Testes](#testes)
- [Dependências Principais](#dependências-principais)

---

## Visão Geral

| Item | Detalhe |
|---|---|
| Plataforma | .NET 10 |
| SDK | 10.0.100 (definido em `global.json`) |
| Banco de Dados | PostgreSQL 18 |
| Mensageria | RabbitMQ |
| API Gateway | YARP 2.3.0 |
| ORM | Entity Framework Core 10.0.1 + Npgsql 10.0.0 |
| Autenticação | JWT Bearer + ASP.NET Core Identity |

---

## Arquitetura

```
Cliente (Blazor WebApp)
        │
        ▼
YARP API Gateway (:6006)
  ├── JWT Validation (centralizado)
  ├── Roteamento por prefixo de rota
  │
  ├──► Users API         (autenticação, perfil)
  ├──► TaskManager API   (tarefas, labels)
  ├──► Nutrition API     (diários, refeições, alimentos)
  ├──► Financial API     (transações, categorias, moedas)
  └──► Gym API           (exercícios, rotinas, sessões)
        │
        ▼ (eventos de integração via RabbitMQ)
  Notification API       (e-mails via SMTP/MailHog)
        │
        ▼
  PostgreSQL (banco único compartilhado)
```

### Padrões Arquiteturais

| Padrão | Aplicação |
|---|---|
| Clean Architecture | Separação em Domain / Application / Infrastructure / API |
| Domain-Driven Design (DDD) | Entidades ricas, Value Objects, Eventos de Domínio |
| CQRS | Commands e Queries com pipeline de behaviors customizado |
| Result Pattern | `Result<T>` / `HttpResult<T>` — sem exceções no fluxo normal |
| Repository + Specification | Acesso a dados com filtros compostos via `Specification<T>` |
| Strategy | Seleção de templates de e-mail no Notification Service |
| Pipeline Behaviors | `ValidationBehavior<TRequest, TResponse>` com FluentValidation |
| Soft Delete | Flag `IsDeleted` em todas as entidades via `BaseEntity<T>` |

---

## Microserviços

### Users Service
Responsável por autenticação, registro e gerenciamento de perfis de usuário.

- ASP.NET Core Identity + JWT Bearer
- Value Objects: `Name` (FirstName, LastName), `Contact` (Email, Phone)
- Publica `UserRegisteredIntegrationEvent` no RabbitMQ após registro
- Endpoints: `AuthController` (register, login, refresh-token) e `UsersController` (perfil, update)

### TaskManager Service
Gerenciamento de tarefas com suporte a labels, prioridades e lembretes de vencimento.

- Entidades: `TaskItem`, `TaskLabel` (relação many-to-many)
- Background Service `DueDateReminderService` que publica `TaskDueReminderIntegrationEvent`
- 7 enumerações com cores hex e nomes em português
- Endpoints: CRUD de tarefas + gestão de labels

### Nutrition Service
Acompanhamento de nutrição com diários, refeições, alimentos e ingestão de líquidos.

- 7 entidades de domínio: `DailyProgress`, `Diary`, `Meal`, `MealFood`, `Food`, `Liquid`, `LiquidType`
- Value Object `DailyGoal` (calorias, proteína, carboidrato, gordura, líquido)
- Seed de alimentos via CSV com CsvHelper 33.1.0
- Endpoints: 5 controllers cobrindo todos os agregados

### Financial Service
Controle financeiro com transações, categorias e suporte a múltiplas moedas.

- Entidades: `Transaction`, `Category`
- Value Object `Money` (Amount + Currency) com suporte a 140+ moedas e símbolo via `ToSymbol()`
- Enumerações: `TransactionType` (Income/Expense), `PaymentMethod` (6 métodos)
- `ReportsController` (placeholder para futuras funcionalidades)

### Gym Service
Gerenciamento de treinos com exercícios, rotinas e sessões de treino.

- 5 entidades: `Exercise`, `Routine`, `RoutineExercise`, `TrainingSession`, `CompletedExercise`
- 6 Value Objects: `Weight`, `SetCount`, `RepetitionCount`, `RestTime`, `Duration`, `ExerciseIntensity`
- 7 enumerações (MuscleGroup, ExerciseType, DifficultyLevel, etc.)
- Endpoints: 5 controllers com operações completas de CRUD

### Notification Service
Envio de e-mails transacionais a partir de eventos do RabbitMQ.

- Consome: `UserRegisteredIntegrationEvent` (user_exchange) e `TaskDueReminderIntegrationEvent` (task_exchange)
- Strategy Pattern para seleção de template por tipo de evento
- MailKit 4.14.1 + SMTP (MailHog em desenvolvimento)
- Sem endpoints REST — processamento 100% por eventos

### API Gateway (YARP)
Ponto de entrada único para todos os microserviços.

- YARP 2.3.0 como reverse proxy
- Validação centralizada de JWT
- Roteamento baseado em prefixo de rota por serviço
- Porta exposta: `:6006`

---

## Bibliotecas Compartilhadas

### BuildingBlocks
Implementações do pipeline CQRS customizado:

- `ISender` / `IPublisher` — despacho de commands, queries e eventos
- `IPipelineBehavior<TRequest, TResponse>` — middleware do pipeline
- `ValidationBehavior` — integração com FluentValidation
- Extensions de JWT e autorização
- `QueryFilterBuilder` — geração dinâmica de expressões LINQ
- `OrderByHelper` — ordenação dinâmica por string

### BuildingBlocks.Messaging
Infraestrutura de mensageria com RabbitMQ:

- `IEventBus` / `IEventConsumer` — publicação e consumo de eventos
- `RabbitMqPersistentConnection` — reconexão automática
- `IntegrationEvent` — base para eventos de integração
- `IConsumerDefinition` — configuração de exchange/queue/routing key

### Core.Domain
Primitivos de domínio reutilizáveis:

- `BaseEntity<TId>` — entidade base com `CreatedAt`, `UpdatedAt`, `IsDeleted` e eventos de domínio
- `ValueObject` — base para comparação por valor
- `Specification<T, TId>` — filtros compostos para repositórios
- `IRepository<T, TId, TFilter>` — contrato genérico de repositório

### Core.Application / Core.Infrastructure
Abstrações e implementações comuns de serviço e acesso a dados compartilhadas entre todos os microserviços.

---

## Infraestrutura

### Docker

Todos os serviços são orquestrados via Docker Compose.

```bash
docker-compose up -d
```

#### Serviços e Portas

| Serviço | Porta | Descrição |
|---|---|---|
| `lifesyncdb` | `5433:5432` | PostgreSQL 18 |
| `pgadmin` | `5050:80` | pgAdmin 4 (admin@admin.com / admin) |
| `rabbitmq` | `5672`, `15672` | RabbitMQ (guest/guest) + Management UI |
| `mailhog` | `1025`, `8025` | SMTP fake + UI de e-mails |
| `yarpapigateway` | `6006:8080` | API Gateway (YARP) |
| `lifesyncapp.webapp` | `6007:8080` | Frontend Blazor |

#### Variáveis de Ambiente (override)

| Variável | Valor padrão |
|---|---|
| `POSTGRES_USER` | `postgres` |
| `POSTGRES_PASSWORD` | `postgres` |
| `POSTGRES_DB` | `LifeSyncDB` |
| `RABBITMQ_DEFAULT_USER` | `guest` |
| `RABBITMQ_DEFAULT_PASS` | `guest` |
| `PGADMIN_DEFAULT_EMAIL` | `admin@admin.com` |
| `PGADMIN_DEFAULT_PASSWORD` | `admin` |

Cada API recebe sua string de conexão e configurações de RabbitMQ/SMTP via variáveis de ambiente no `docker-compose.override.yml`.

### Banco de Dados

Banco único compartilhado (`LifeSyncDB`) no PostgreSQL. Cada serviço gerencia suas próprias migrations via `MigrationHostedService` executado na inicialização.

Soft delete habilitado em todas as entidades através do `IsDeleted` configurado no `BaseEntity<T>`.

---

## Documentação Detalhada

A documentação técnica completa de cada microserviço — incluindo todas as entidades, endpoints, configurações e dependências — está disponível na pasta [`documentations/`](./documentations/):

| Arquivo | Conteúdo |
|---|---|
| [users-service.md](./documentations/users-service.md) | Autenticação, JWT, Identity, perfil de usuário |
| [taskmanager-service.md](./documentations/taskmanager-service.md) | Tarefas, labels, lembrete de vencimento |
| [nutrition-service.md](./documentations/nutrition-service.md) | Diários, refeições, alimentos, líquidos, metas |
| [financial-service.md](./documentations/financial-service.md) | Transações, categorias, moedas |
| [gym-service.md](./documentations/gym-service.md) | Exercícios, rotinas, sessões de treino |
| [notification-service.md](./documentations/notification-service.md) | E-mails via RabbitMQ + SMTP |
| [building-blocks.md](./documentations/building-blocks.md) | Building Blocks & Core — CQRS, Result Pattern, Messaging, Domain |

---

## Como Executar

### Pré-requisitos

- [.NET SDK 10.0.100](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Ambiente Completo (Docker)

```bash
# Subir todos os serviços
docker-compose up -d

# Acompanhar logs de um serviço específico
docker-compose logs -f taskmanager.api
```

### Desenvolvimento Local

```bash
# Subir apenas a infraestrutura (banco, fila, e-mail)
docker-compose up -d lifesyncdb rabbitmq mailhog

# Executar um serviço individualmente
cd Services/TaskManager/TaskManager.API
dotnet run
```

### Acessos após subida

- **API Gateway:** http://localhost:6006
- **Frontend:** http://localhost:6007
- **pgAdmin:** http://localhost:5050
- **RabbitMQ Management:** http://localhost:15672
- **MailHog UI:** http://localhost:8025

---

## Testes

Os testes estão localizados na pasta `tests/` e cobrem o TaskManager Service nos três níveis:

| Projeto | Tipo | Descrição |
|---|---|---|
| `TaskManager.UnitTests` | Unitário | Domínio, handlers, validadores |
| `TaskManager.IntegrationTests` | Integração | Repositórios com Testcontainers (PostgreSQL real) |
| `TaskManager.E2ETests` | E2E | Fluxos completos de API com Testcontainers |

```bash
# Executar todos os testes
dotnet test

# Executar apenas unitários
dotnet test tests/TaskManager.UnitTests
```

---

## Dependências Principais

| Pacote | Versão | Uso |
|---|---|---|
| `Microsoft.EntityFrameworkCore` | 10.0.1 | ORM |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.0 | Provider PostgreSQL |
| `FluentValidation` | 11.11.0 | Validação de commands/queries |
| `Yarp.ReverseProxy` | 2.3.0 | API Gateway |
| `RabbitMQ.Client` | 7.2.0 | Mensageria |
| `MailKit` | 4.14.1 | Envio de e-mails SMTP |
| `MimeKit` | 4.14.0 | Construção de mensagens MIME |
| `CsvHelper` | 33.1.0 | Seed de alimentos (Nutrition) |
| `Microsoft.AspNetCore.Identity` | 10.0.1 | Autenticação (Users) |
| `Testcontainers.PostgreSql` | — | Testes de integração/E2E |
