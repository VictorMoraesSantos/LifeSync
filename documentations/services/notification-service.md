# Notification Service

Responsável pelo envio de notificações e e-mails no LifeSync.

## Índice

- [Visão Geral](#visão-geral)
- [Estrutura de Pastas](#estrutura-de-pastas)
- [Domínio](#domínio)
- [Aplicação](#aplicação)
- [Infraestrutura](#infraestrutura)
- [API](#api)
- [Configuração](#configuração)
- [Dependências](#dependências)

---

## Visão Geral

O Notification Service é o serviço de mensageria do LifeSync. Ele consome eventos do RabbitMQ publicados por outros microserviços e os converte em e-mails enviados via SMTP. Utiliza o **padrão Strategy** para selecionar o template correto de e-mail com base no tipo de evento recebido.

### Responsabilidades

- Consumo de eventos de integração via RabbitMQ
- Envio de e-mails transacionais via SMTP (MailKit)
- Estratégias de e-mail por tipo de evento (Strategy Pattern)
- Persistência de mensagens de e-mail no banco de dados (parcial)

### Fluxo de Processamento

```
RabbitMQ Message
    ↓
RabbitMqEventConsumer (deserialização JSON → IntegrationEvent)
    ↓
IPublisher (dispatch CQRS)
    ↓
EventHandler (ex: UserRegisteredEventHandler)
    ↓
ProcessEmailEventUseCase
    ↓
EmailEventStrategyResolver (seleção de estratégia)
    ↓
Strategy (ex: UserRegisteredEmailStrategy)
    ↓
EmailMessageDTO
    ↓
EmailService.SendEmailAsync()
    ↓
MailKit SmtpClient → Servidor SMTP
```

---

## Estrutura de Pastas

```
Notification/
├── Notification.API/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Dockerfile
│   └── Notification.API.csproj
├── Notification.Application/
│   ├── Contracts/
│   │   ├── IEmailEventStrategy.cs
│   │   ├── IEmailEventStrategyResolver.cs
│   │   ├── IEmailService.cs
│   │   └── IProcessEmailEventUseCase.cs
│   ├── DTO/
│   │   └── EmailMessageDTO.cs
│   ├── EventHandlers/
│   │   └── UserRegisteredEventHandler.cs
│   ├── Factories/
│   │   ├── EmailEventStrategyFactory.cs
│   │   └── EmailEventStrategyResolver.cs
│   ├── Features/
│   │   └── ProcessEmailEventUseCase.cs
│   ├── Strategies/
│   │   ├── UserRegisteredEmailStrategy.cs
│   │   └── OrderPlacedEmailStrategy.cs
│   ├── DependencyInjection.cs
│   └── Notification.Application.csproj
├── Notification.Domain/
│   ├── Entities/EmailMessage.cs
│   ├── Enums/EmailEventTypes.cs
│   ├── Events/
│   │   ├── EmailSentEvent.cs
│   │   ├── UserRegisteredIntegrationEvent.cs
│   │   └── TaskDueReminderIntegrationEvent.cs
│   ├── Repositories/IEmailMessageRepository.cs
│   └── Notification.Domain.csproj
└── Notification.Infrastructure/
    ├── DependencyInjection.cs
    ├── Messaging/RabbitMqEventConsumer.cs
    ├── Migrations/
    │   └── 20250927183709_InitialCreate.cs
    ├── Persistence/
    │   ├── Data/
    │   │   ├── ApplicationDbContext.cs
    │   │   └── MigrationHostedService.cs
    │   └── Repositories/EmailMessageRepository.cs
    ├── Services/EmailService.cs
    ├── Smtp/SmtpSettings.cs
    └── Notification.Infrastructure.csproj
```

---

## Domínio

### Entidade: `EmailMessage`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Descrição |
|---|---|---|
| `From` | `string` | Remetente |
| `To` | `string` | Destinatário |
| `Subject` | `string` | Assunto |
| `Body` | `string` | Corpo do e-mail |

---

### Enum: `EmailEventTypes`

Constantes que identificam os tipos de evento de e-mail:

| Constante | Valor |
|---|---|
| `EmailEventTypes.UserRegistered` | `"UserRegistered"` |
| `EmailEventTypes.OrderPlaced` | `"OrderPlaced"` |

---

### Eventos de Domínio e Integração

#### `EmailSentEvent` (DomainEvent)

Disparado após o envio bem-sucedido de um e-mail.

| Propriedade | Tipo |
|---|---|
| `Email` | `string` |
| `SentAt` | `DateTime` |

#### `UserRegisteredIntegrationEvent` (IntegrationEvent)

Consumido do RabbitMQ quando um novo usuário se registra (publicado pelo Users Service).

| Propriedade | Tipo |
|---|---|
| `UserId` | `int` |
| `Email` | `string` |

**Origem:** Exchange `user_exchange`, Routing Key `user.registered`

#### `TaskDueReminderIntegrationEvent` (IntegrationEvent)

Consumido do RabbitMQ quando uma tarefa está próxima do vencimento (publicado pelo TaskManager Service).

| Propriedade | Tipo |
|---|---|
| `TaskId` | `int` |
| `UserId` | `int` |
| `TaskTitle` | `string` |
| `DueDate` | `DateOnly` |

**Origem:** Exchange `task_exchange`, Routing Key `task.due.reminder`

---

### `IEmailMessageRepository`

Herda de `IRepository<EmailMessage, int>`.
Métodos disponíveis: `Create`, `CreateRange`, `GetById`, `GetAll`, `Find`, `Delete`.

> **Nota:** O método `Update` ainda lança `NotImplementedException`.

---

## Aplicação

### Contratos de Serviço

#### `IEmailEventStrategy`

Interface do padrão Strategy para geração de e-mails por tipo de evento:

```csharp
string EventType { get; }
EmailMessageDTO CreateEmail(string eventData);
```

#### `IEmailEventStrategyResolver`

Resolve a estratégia correta para um tipo de evento:

```csharp
IEmailEventStrategy? Resolve(string eventType);
```

Utiliza matching case-insensitive.

#### `IEmailService`

```csharp
Task<EmailMessageDTO> GetEmailById(int id, CancellationToken)
Task<IEnumerable<EmailMessageDTO>> GetAllEmailMessages(CancellationToken)
Task<int> CreateEmail(EmailMessageDTO dto, CancellationToken)
Task<bool> UpdateEmail(EmailMessageDTO dto, CancellationToken)
Task<bool> DeleteEmail(int id, CancellationToken)
Task SendEmailAsync(EmailMessageDTO dto, CancellationToken)
```

> **Nota:** Apenas `SendEmailAsync` está implementado. Os demais métodos lançam `NotImplementedException`.

#### `IProcessEmailEventUseCase`

```csharp
Task HandleAsync(string eventType, string eventData, CancellationToken)
```

---

### `EmailMessageDTO`

```csharp
record EmailMessageDTO(string To, string Subject, string Body)
```

---

### Event Handlers

#### `UserRegisteredEventHandler`

Implementa `INotificationHandler<UserRegisteredIntegrationEvent>`.

Ao receber o evento, delega para `IProcessEmailEventUseCase.HandleAsync("UserRegistered", email)`.

---

### Estratégias de E-mail

#### `UserRegisteredEmailStrategy`

| Campo | Valor |
|---|---|
| `EventType` | `"UserRegistered"` |
| `Subject` | `"Welcome!"` |
| `Body` | `"Thanks for registering."` |

#### `OrderPlacedEmailStrategy`

| Campo | Valor |
|---|---|
| `EventType` | `"OrderPlaced"` |
| `Subject` | `"Order Confirmation"` |
| `Body` | `"Your order has been placed."` |

---

### `ProcessEmailEventUseCase`

Implementação do use case principal:

1. Chama `IEmailEventStrategyResolver.Resolve(eventType)`
2. Se não houver estratégia registrada, retorna silenciosamente
3. Gera `EmailMessageDTO` via estratégia
4. Envia o e-mail via `IEmailService.SendEmailAsync()`

---

## Infraestrutura

### `ApplicationDbContext`

| DbSet | Tipo |
|---|---|
| `EmailMessages` | `DbSet<EmailMessage>` |

### Migrations

| Migration | Data | Tabela criada |
|---|---|---|
| `20250927183709_InitialCreate` | 2025-09-27 | `EmailMessages` |

**Schema da tabela `EmailMessages`:**

| Coluna | Tipo | Nullable |
|---|---|---|
| `Id` | `int` (PK, auto) | Não |
| `From` | `text` | Não |
| `To` | `text` | Não |
| `Subject` | `text` | Não |
| `Body` | `text` | Não |
| `CreatedAt` | `timestamp with time zone` | Não |
| `UpdatedAt` | `timestamp with time zone` | Sim |
| `IsDeleted` | `boolean` (default: false) | Não |

---

### `EmailService`

Utiliza **MailKit** para envio de e-mails:

1. Cria `EmailMessage` em memória (não persiste atualmente)
2. Monta `MimeMessage` com remetente, destinatário, assunto e corpo
3. Se `SmtpSettings.From` estiver vazio, usa `"no-reply@test.local"`
4. Conecta ao servidor SMTP sem SSL (`SecureSocketOptions.None`)
5. Autentica apenas se `SmtpSettings.User` estiver preenchido
6. Envia e desconecta

---

### `RabbitMqEventConsumer` (BackgroundService)

Consome mensagens do RabbitMQ e as despacha para o pipeline CQRS:

1. Itera sobre todas as `IConsumerDefinition` registradas
2. Para cada definição, registra um callback via `IEventConsumer.StartConsuming()`
3. Ao receber mensagem, desserializa JSON → `IntegrationEvent`
4. Publica evento no pipeline via `IPublisher`

---

### Configuração de Filas RabbitMQ

| Exchange | Queue | Routing Key | Evento |
|---|---|---|---|
| `user_exchange` (Topic) | `email_events.user_registered` | `user.registered` | `UserRegisteredIntegrationEvent` |
| `task_exchange` (Topic) | `task_events.task_reminder` | `task.due.reminder` | `TaskDueReminderIntegrationEvent` |

Ambas as filas são configuradas com `Durable = true` e `AutoDelete = false`.

---

## API

O serviço não expõe endpoints REST de negócio. Apenas o health check está disponível.

### Health Check

```
GET /health
→ { "status": "healthy", "service": "Notification", "timestamp": "...", "environment": "..." }
```

---

## Limitações Conhecidas

| Severidade | Problema | Descrição |
|---|---|---|
| CRÍTICO | Consumer sem error handling | `RabbitMqEventConsumer.OnMessage()` não tem try-catch — erro de JSON causa crash |
| CRÍTICO | Blocking async | `.GetAwaiter().GetResult()` bloqueia thread do pool |
| CRÍTICO | SMTP sem TLS/SSL | `SecureSocketOptions.None` envia credenciais em texto puro |
| CRÍTICO | Sem error handling no EmailService | Exceções SMTP não são tratadas — podem derrubar o serviço |
| CRÍTICO | Credenciais hardcoded | RabbitMQ usa `guest/guest` no appsettings |
| ALTO | Sem Dead Letter Queue | Mensagens com falha são perdidas |
| ALTO | Templates hardcoded | Textos literais sem personalização (`"Thanks for registering."`) |
| ALTO | Sem validação de e-mail | Destinatário não é validado antes do envio |
| ALTO | Nova conexão SMTP por e-mail | Sem connection pooling — overhead em cada envio |
| ALTO | Zero logging | Nenhum `ILogger` em todo o serviço |
| ALTO | Zero testes | Nenhum projeto de teste |
| ALTO | Banco implementado mas não usado | `EmailMessageRepository` e `DbContext` nunca são chamados |
| ALTO | Handler para `TaskDueReminderIntegrationEvent` ausente | Evento consumido mas ignorado silenciosamente |
| MÉDIO | Métodos de CRUD não implementados | `CreateEmail`, `UpdateEmail`, `DeleteEmail`, `GetEmailById` lançam `NotImplementedException` |
| MÉDIO | Namespace misto (~70% ainda `EmailSender.*`) | Refatoração incompleta do nome do serviço |
| MÉDIO | Dead code | `EmailMessage` criado em memória mas nunca usado |
| MÉDIO | Typo no parâmetro | `cancellationTokeno` ao invés de `cancellationToken` |
| MÉDIO | `EnableSsl` configurado mas ignorado | Propriedade existe no `SmtpSettings` mas não é usada no `EmailService` |

---

## Configuração

```json
{
  "ConnectionStrings": {
    "Database": "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=LifeSync;Include Error Detail=true;"
  },
  "SmtpSettings": {
    "Host": "localhost",
    "Port": 1025,
    "User": "",
    "Password": "",
    "From": "no-reply@test.local",
    "EnableSsl": false
  },
  "RabbitMQSettings": {
    "Host": "rabbitmq",
    "User": "guest",
    "Password": "guest",
    "Port": 5672
  }
}
```

Em desenvolvimento (`appsettings.Development.json`), o host do RabbitMQ é sobrescrito para `localhost`.

---

## Dependências

### Pacotes NuGet

| Pacote | Versão | Uso |
|---|---|---|
| `MailKit` | 4.14.1 | Envio de e-mails via SMTP |
| `MimeKit` | 4.14.0 | Construção de mensagens MIME |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.0 | Provider PostgreSQL |
| `Microsoft.EntityFrameworkCore` | 10.0.1 | ORM |
| `Microsoft.Extensions.Options.ConfigurationExtensions` | 10.0.1 | Binding de configuração |
| `Microsoft.AspNetCore.OpenApi` | 10.0.1 | Swagger |

### Referências Internas

| Projeto | Uso |
|---|---|
| `BuildingBlocks` | CQRS, IPublisher, INotificationHandler |
| `BuildingBlocks.Messaging` | IntegrationEvent, IEventConsumer, IEventBus |
| `Core.Domain` | BaseEntity |
