# Building Blocks & Core

Bibliotecas internas compartilhadas por todos os microserviços do LifeSync.

## Índice

- [Visão Geral](#visão-geral)
- [BuildingBlocks](#buildingblocks)
  - [CQRS](#cqrs)
  - [Result Pattern](#result-pattern)
  - [Validação](#validação)
  - [Autenticação e Autorização](#autenticação-e-autorização)
  - [Helpers de Query](#helpers-de-query)
- [BuildingBlocks.Messaging](#buildingblocksmessaging)
  - [Event Bus](#event-bus)
  - [Event Consumer](#event-consumer)
  - [Conexão Persistente](#conexão-persistente)
- [Core.Domain](#coredomain)
  - [BaseEntity](#baseentity)
  - [Value Object](#value-object)
  - [Specification Pattern](#specification-pattern)
  - [Repository Pattern](#repository-pattern)
  - [Domain Events](#domain-events)
- [Core.Application](#coreapplication)
- [Core.Infrastructure](#coreinfrastructure)
- [Core.API](#coreapi)
- [Dependências](#dependências)

---

## Visão Geral

O LifeSync é construído sobre uma base de bibliotecas internas que provêm padrões consistentes em todos os microserviços:

| Biblioteca | Responsabilidade |
|---|---|
| `BuildingBlocks` | CQRS, Result Pattern, Validação, JWT, Autorização |
| `BuildingBlocks.Messaging` | RabbitMQ — publicação e consumo de eventos |
| `Core.Domain` | BaseEntity, ValueObject, Specification, IRepository, DomainEvent |
| `Core.Application` | DTOBase, interfaces de serviço (IReadService, ICreateService, etc.) |
| `Core.Infrastructure` | SpecificationEvaluator para EF Core |
| `Core.API` | ApiController base para todos os controllers |

---

## BuildingBlocks

### CQRS

Implementação própria do padrão CQRS com suporte a pipeline behaviors.

#### Interfaces Principais

```csharp
// Comandos retornam Result<T>
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
public interface ICommand : IRequest<Result> { }

// Queries retornam Result<T>
public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }

// Handlers
public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>> where TCommand : ICommand<TResponse> { }

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse> { }
```

#### `ISender`

Dispatcher de comandos e queries. Resolve o handler correto via DI com suporte a pipeline behaviors.

```csharp
Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
```

**Ordem de execução:**
1. Todos os `IPipelineBehavior<TRequest, TResponse>` registrados são executados em ordem
2. O handler final processa a requisição

#### `IPublisher`

Pub/sub para notificações. Invoca todos os handlers registrados para o tipo de notificação.

```csharp
Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken)
    where TNotification : INotification
```

#### `IPipelineBehavior<TRequest, TResponse>`

Middleware para requisições CQRS:

```csharp
Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken)
```

#### Registro (DI)

```csharp
services.AddBuildingBlocks(Assembly.GetExecutingAssembly());
// Registra: ISender, IPublisher, todos os handlers e validadores do assembly
```

---

### Result Pattern

Sistema de tratamento de erros sem exceções para fluxo normal.

#### `ErrorType`

```csharp
public enum ErrorType
{
    Failure = 0,    // Falha genérica
    Validation = 1, // Erro de validação
    Problem = 2,    // Problema de negócio
    NotFound = 3,   // Recurso não encontrado
    Conflict = 4    // Conflito de dados
}
```

#### `Error`

```csharp
public record Error
{
    public string Description { get; }
    public ErrorType Type { get; }

    public static Error Failure(string description)
    public static Error NotFound(string description)
    public static Error Problem(string description)
    public static Error Conflict(string description)
}
```

#### `Result<T>` e `Result`

```csharp
// Criação
Result<int>.Success(42)
Result<int>.Failure(Error.NotFound("Item não encontrado"))

// Verificação
result.IsSuccess   // bool
result.IsFailure   // bool
result.Value       // T (lança exceção se falha)
result.Error       // Error (lança exceção se sucesso)
```

#### `HttpResult<T>`

Wrapper de resposta HTTP — converte `Result<T>` em respostas padronizadas:

| Método | Status HTTP | Uso |
|---|---|---|
| `Ok(data)` | 200 | Sucesso com dados |
| `Created(data)` | 201 | Recurso criado |
| `Updated()` | 204 | Atualizado sem corpo |
| `Deleted()` | 204 | Removido sem corpo |
| `BadRequest(errors)` | 400 | Erro de validação |
| `NotFound(errors)` | 404 | Não encontrado |
| `Unauthorized(errors)` | 401 | Não autenticado |
| `Forbidden(errors)` | 403 | Sem permissão |
| `InternalError(errors)` | 500 | Erro interno |
| `FromResult(result)` | — | Conversão automática de `Result<(Items, Pagination)>` |

**Estrutura da resposta:**
```json
{
  "success": true,
  "statusCode": 200,
  "data": { ... },
  "errors": [],
  "pagination": { "currentPage": 1, "pageSize": 50, "totalItems": 100, "totalPages": 2 }
}
```

#### `PaginationData`

```csharp
public class PaginationData
{
    public int? CurrentPage { get; }   // Default: 1
    public int? PageSize { get; }      // Default: 50
    public int? TotalItems { get; }    // Default: 0
    public int? TotalPages { get; }    // Default: 0
}
```

---

### Validação

Pipeline de validação automática usando **FluentValidation** integrado ao CQRS.

#### `ValidationBehavior<TRequest, TResponse>`

`IPipelineBehavior` que:
1. Localiza todos os `IValidator<TRequest>` registrados
2. Executa validações assíncronas
3. Coleta todas as falhas
4. Lança `ValidationException` se houver erros (antes do handler)

#### `ValidationExceptionHandlingMiddleware`

Captura `ValidationException` e retorna resposta no formato **RFC 7231**:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Failed",
  "status": 400,
  "errors": {
    "PropertyName": ["Error message 1", "Error message 2"]
  }
}
```

**Uso no `Program.cs`:**
```csharp
builder.Services.AddValidationExceptionHandling();
// ...
app.UseValidationExceptionHandling();
```

---

### Autenticação e Autorização

#### `JwtAuthenticationExtensions`

```csharp
services.AddJwtAuthentication(configuration)
```

Configura `JwtBearer` com validação de:
- Issuer (`JwtSettings:Issuer`)
- Audience (`JwtSettings:Audience`)
- Assinatura (`JwtSettings:Key`)
- Lifetime
- `ClockSkew = TimeSpan.Zero` (sem tolerância de tempo)

#### `AuthorizationExtensions`

Extensões para `ClaimsPrincipal`:

```csharp
// Retorna o UserId do usuário autenticado
user.GetUserId()       → int?

// Verifica se é administrador
user.IsAdmin()         → bool

// Verifica se pode acessar recurso de outro usuário (admins podem tudo)
user.CanAccess(resourceUserId)  → bool
```

---

### Helpers de Query

#### `OrderByHelper`

Ordenação dinâmica via LINQ com reflection:

```csharp
IQueryable<T> query = query.ApplyOrderBy("Name", descending: false);
```

- Case-insensitive na busca de propriedade
- Retorna query original se propriedade não existir

#### `QueryFilterBuilder`

Construção dinâmica de filtros LINQ:

```csharp
// Filtro por valor nullable
query = QueryFilterBuilder.AddFilter(query, x => x.Status, status);

// Filtro por string contém
query = QueryFilterBuilder.AddContainsFilter(query, x => x.Name, nameContains);

// Filtro em coleção aninhada
query = QueryFilterBuilder.AddCollectionFilter(query, x => x.Labels, x => x.Id, labelId);
```

Todos os filtros são traduzidos para SQL pelo EF Core.

---

## BuildingBlocks.Messaging

Abstração sobre o RabbitMQ para publicação e consumo de eventos de integração.

### `IntegrationEvent`

Classe base para todos os eventos de integração entre microserviços:

```csharp
public abstract class IntegrationEvent : INotification
{
    public Guid Id { get; }          // Auto-gerado
    public DateTime CreationDate { get; }  // UTC
}
```

### Event Bus

#### `IEventBus`

```csharp
void PublishAsync<TEvent>(TEvent @event, PublishOptions options)
    where TEvent : IntegrationEvent;
```

#### `PublishOptions`

| Propriedade | Padrão | Descrição |
|---|---|---|
| `ExchangeName` | `"default.exchange"` | Nome do exchange |
| `TypeExchange` | `Fanout` | Tipo do exchange |
| `RoutingKey` | `""` | Routing key |
| `Durable` | `true` | Exchange durável |
| `AutoDelete` | `false` | Auto-deletar |

**Comportamento do `EventBus`:**
- Declara exchange no RabbitMQ
- Serializa evento para JSON
- Publica com `DeliveryMode.Persistent` (mensagens persistidas em disco)
- Usa `Event.Id` como Message ID

---

### Event Consumer

#### `IEventConsumer`

```csharp
void StartConsuming(Action<string> onMessageReceived, ConsumerOptions? options = null);
```

#### `ConsumerOptions`

| Propriedade | Descrição |
|---|---|
| `QueueName` | Nome da fila |
| `ExchangeName` | Exchange de origem |
| `TypeExchange` | Tipo do exchange |
| `RoutingKey` | Routing key para binding |
| `Durable` | Fila durável |
| `AutoDelete` | Auto-deletar |

**Comportamento do `EventConsumer`:**
- Declara a fila e realiza o binding com o exchange
- Usa `AsyncEventingBasicConsumer`
- `BasicQos(prefetchCount: 1)` para fair dispatch
- Acknowledgment manual após processamento (`BasicAck`)

#### `AddEventConsumer<TEvent>` (extensão de DI)

```csharp
services.AddEventConsumer<UserRegisteredIntegrationEvent>(opts =>
{
    opts.ExchangeName = "user_exchange";
    opts.QueueName = "email_events.user_registered";
    opts.RoutingKey = "user.registered";
    opts.TypeExchange = ExchangeType.Topic;
    opts.Durable = true;
});
```

---

### Conexão Persistente

#### `PersistentConnection`

Gerencia a conexão com o RabbitMQ com reconexão automática:

| Configuração | Valor |
|---|---|
| Tentativas máximas | 5 |
| Backoff exponencial | `2^attempt` segundos, máximo 30s |
| `AutomaticRecoveryEnabled` | `true` |
| `NetworkRecoveryInterval` | 10 segundos |
| `TopologyRecoveryEnabled` | `true` |
| `RequestedHeartbeat` | 10 segundos |
| `SocketReadWriteTimeout` | 30 segundos |
| Thread-safe | Sim (via `lock`) |

#### `RabbitMqSettings`

```json
{
  "RabbitMQSettings": {
    "Host": "rabbitmq",
    "User": "guest",
    "Password": "guest",
    "Port": 5672,
    "VirtualHost": "/",
    "ClientProvidedName": "LifeSync"
  }
}
```

#### Registro (DI)

```csharp
services.AddMessaging(configuration);
// Registra: RabbitMqSettings, PersistentConnection (singleton), IEventBus (singleton), IEventConsumer (singleton)
```

---

## Core.Domain

Fundação do domínio compartilhada por todos os serviços.

### `BaseEntity<T>`

Classe base para todas as entidades:

```csharp
public abstract class BaseEntity<T> : IBaseEntity<T>
{
    public T Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    public void MarkAsUpdated()        // Atualiza UpdatedAt
    public void MarkAsDeleted()        // Sets IsDeleted = true
    public void AddDomainEvent(IDomainEvent) // Acumula evento
    public void ClearDomainEvents()    // Limpa eventos após dispatch
}
```

**Igualdade:** Baseada no `Id` (não na referência).

### `IAggregateRoot`

Interface marcadora para raízes de agregados (sem membros). Usada para identificar os agregados que coordenam mudanças de estado no domínio.

---

### Value Object

```csharp
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    // Igualdade baseada nos componentes, não na referência
    public override bool Equals(object? obj)
    public override int GetHashCode()
    public static bool operator ==(ValueObject a, ValueObject b)
    public static bool operator !=(ValueObject a, ValueObject b)
}
```

**Uso:** Estenda `ValueObject` e implemente `GetEqualityComponents()` retornando os campos que definem a igualdade.

---

### Domain Events

```csharp
public interface IDomainEvent : INotification
{
    Guid Id { get; set; }
    DateTime OccuredOn { get; }
}

public abstract class DomainEvent : IDomainEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime OccuredOn { get; } = DateTime.UtcNow;
}
```

- Herda de `INotification` para integração com o `IPublisher` do CQRS
- Acumulados na entidade via `AddDomainEvent()` e despachados após a persistência

---

### `DomainException`

```csharp
public class DomainException : Exception
{
    public string Description { get; }
}
```

Construtores:
- `(string message)` — Description = "Domain.Error"
- `(Error error)` — Usa o tipo `Error` do Result Pattern

---

### Specification Pattern

#### `IDomainQueryFilter`

Interface base para todos os filtros de query:

```csharp
public interface IDomainQueryFilter
{
    DateOnly? CreatedAt { get; }
    DateOnly? UpdatedAt { get; }
    bool? IsDeleted { get; }
    string? SortBy { get; }
    bool? SortDesc { get; }
    int? Page { get; }
    int? PageSize { get; }   // Default: 50
}
```

#### `Specification<T, TId>`

Classe base para todas as especificações de query:

| Membro | Descrição |
|---|---|
| `Criteria` | Expressão WHERE |
| `Includes` | Navigation properties para eager loading |
| `OrderBy` / `OrderByDescending` | Ordenação |
| `Skip` / `Take` | Paginação |
| `IsPagingEnabled` | Flag de paginação |

**Métodos protegidos:**

```csharp
protected void AddCriteria(Expression<Func<T, bool>> criteria)
protected void AddIf(bool condition, Expression<Func<T, bool>> predicate)
protected void ApplyBaseFilters(IDomainQueryFilter filter)  // Aplica IsDeleted, Sort, Pagination
protected void ApplyOrderBy(Expression<Func<T, object>> expr)
protected void ApplyPaging(int skip, int take)
protected void AddInclude(Expression<Func<T, object>> include)
```

**Exemplo de uso:**
```csharp
public class TaskItemSpecification : Specification<TaskItem, int>
{
    public TaskItemSpecification(TaskItemQueryFilter filter)
    {
        AddIf(filter.UserId.HasValue, x => x.UserId == filter.UserId);
        AddIf(!string.IsNullOrEmpty(filter.TitleContains), x => x.Title.Contains(filter.TitleContains!));
        AddIf(filter.Status.HasValue, x => x.Status == filter.Status);
        AddInclude(x => x.Labels);
        ApplyBaseFilters(filter);
    }
}
```

---

### Repository Pattern

#### `IRepository<T, TId>`

Interface base com CRUD:

```csharp
Task<T?> GetById(TId id, CancellationToken)
Task<IEnumerable<T?>> GetAll(CancellationToken)
Task<IEnumerable<T?>> Find(Expression<Func<T, bool>> predicate, CancellationToken)
Task Create(T entity, CancellationToken)
Task CreateRange(IEnumerable<T> entities, CancellationToken)
Task Update(T entity, CancellationToken)
Task Delete(T entity, CancellationToken)
```

#### `IRepository<T, TId, TFilter>`

Estende o anterior com suporte a filtros especificados:

```csharp
Task<(IEnumerable<T> Items, int TotalCount)> FindByFilter(TFilter filter, CancellationToken)
```

---

## Core.Application

### `DTOBase`

Base para todos os DTOs de leitura:

```csharp
public record DTOBase(
    [JsonPropertyOrder(-3)] int Id,
    [JsonPropertyOrder(-2)] DateTime CreatedAt,
    [JsonPropertyOrder(-1)] DateTime? UpdatedAt);
```

### `DomainQueryFilterDTO`

Base para todos os filtros de query em DTOs:

```csharp
public record DomainQueryFilterDTO(
    DateOnly? CreatedAt, DateOnly? UpdatedAt, bool? IsDeleted,
    string? SortBy, bool? SortDesc, int? Page, int? PageSize);
```

### Interfaces de Serviço

| Interface | Métodos |
|---|---|
| `ICreateService<TCreate>` | `CreateAsync(dto)`, `CreateRangeAsync(dtos)` |
| `IReadService<TRead, TId>` | `GetByIdAsync`, `GetAllAsync`, `GetPagedAsync`, `FindAsync`, `CountAsync` |
| `IReadService<TRead, TId, TFilter>` | Herda anterior + `GetByFilterAsync(filter)` |
| `IUpdateService<TUpdate>` | `UpdateAsync(dto)` |
| `IDeleteService<TDelete>` | `DeleteAsync(id)`, `DeleteRangeAsync(ids)` |

Todos retornam `Result<T>` para tratamento funcional de erros.

---

## Core.Infrastructure

### `SpecificationEvaluator`

Converte uma `Specification<T>` em `IQueryable<T>` para o EF Core:

```csharp
public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, Specification<T> spec)
```

**Ordem de aplicação:**
1. `WHERE` (Criteria)
2. `INCLUDE` (Includes)
3. `ORDER BY` (OrderBy / OrderByDescending)
4. `SKIP` / `TAKE` (Paginação)

---

## Core.API

### `ApiController`

Classe base para todos os controllers:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase { }
```

> **Nota:** O atributo `[Authorize]` está comentado — a autenticação é tratada no nível do API Gateway (YARP).

---

## Problemas Críticos

Issues identificadas nas bibliotecas BuildingBlocks que requerem atenção imediata.

### Tabela de Issues

| ID | Severidade | Biblioteca | Componente | Descrição | Impacto |
|----|------------|------------|------------|-----------|---------|
| **BB-001** | 🔴 Crítica | BuildingBlocks | Result Pattern | `Result<T>.Value` lança exceção `InvalidOperationException` quando `IsFailure == true` | Runtime exceptions em fluxo normal de execução; violação do princípio de tratamento funcional |
| **BB-002** | 🔴 Crítica | BuildingBlocks | ValidationBehavior | `ValidationBehavior` lança `ValidationException` em vez de retornar `Result<Unit>.Failure()` | Interrompe o pipeline CQRS abruptamente; não permite tratamento uniforme de erros |
| **BB-003** | 🟠 Alta | BuildingBlocks.Messaging | EventBus | `EventBus.PublishAsync` não trata exceções de serialização JSON | Mensagens podem ser perdidas sem notificação ao publisher |
| **BB-004** | 🟠 Alta | Core.Domain | Domain Events | Eventos de domínio são acumulados mas **não há mecanismo automático de dispatch** | Eventos nunca são publicados/consumidos; perda de consistência eventual |
| **BB-005** | 🟠 Alta | BuildingBlocks | Repository Pattern | `IRepository` não expõe `IUnitOfWork` | Transações distribuídas não são suportadas; múltiplos repositórios não podem participar da mesma transação |
| **BB-006** | 🟡 Média | BuildingBlocks | HttpResult | `HttpResult.BadRequest(errors)` recebe `string[]` mas o formato RFC 7231 espera `Dictionary<string, string[]>` | Incompatibilidade de formato na resposta de validação |
| **BB-007** | 🟡 Média | BuildingBlocks.Messaging | PersistentConnection | Reconexão usa `Thread.Sleep` bloqueante (backoff exponencial) | Bloqueia threads durante reconexão; degrada performance em cenários de falha |
| **BB-008** | 🟡 Média | Core.Infrastructure | SpecificationEvaluator | `GetQuery` não valida se `spec.Criteria` é null antes de aplicar `Where` | Pode resultar em query inválida ou exceção em runtime |
| **BB-009** | 🟢 Baixa | Core.Application | DTOs | DTOs não possuem validação integrada ( FluentValidation não é aplicado a DTOs de entrada ) | Validação depende exclusivamente dos validators do CQRS |
| **BB-010** | 🟢 Baixa | BuildingBlocks | JwtAuthentication | `ClockSkew = TimeSpan.Zero` pode causar falha em tokens legítimos devido a diferenças de relógio | Autenticação pode falhar em ambientes com clocks dessincronizados |

### Detalhamento das Issues Críticas

#### BB-001: Acesso a `Result<T>.Value` em estado de falha

```csharp
// Código que causa problema
var result = Result<int>.Failure(Error.NotFound("Item não encontrado"));
var value = result.Value; // Lança InvalidOperationException
```

**Recomendação:** Sempre verificar `IsSuccess` ou usar match/pattern matching:

```csharp
if (result.IsSuccess)
    return result.Value;
// Tratar erro adequadamente
```

#### BB-002: Validação lança exceção em vez de retornar Failure

O `ValidationBehavior` interrompe o pipeline lançando exceção, impedindo que outros behaviors (como logging) processem a requisição.

**Recomendação:** Retornar `Result<Unit>.Failure(validationErrors)` para permitir tratamento uniforme.

#### BB-004: Domain Events sem Dispatch

Os eventos são acumulados em `BaseEntity` via `AddDomainEvent()`, mas não existe mecanismo no `Core.Infrastructure` para consumi-los e publicá-los no Event Bus após o `SaveChanges`.

**Recomendação:** Implementar `IDomainEventDispatcher` integrado ao EF Core `SaveChangesInterceptor`.

---

## Recomendações de Correção

### Curto Prazo (1-2 semanas)

1. **Adicionar null-check em `SpecificationEvaluator.GetQuery`**
   ```csharp
   if (spec.Criteria != null)
       query = query.Where(spec.Criteria);
   ```

2. **Corrigir formato de erros em `HttpResult.BadRequest`**
   ```csharp
   // Alterar de string[] para Dictionary<string, string[]>
   public static HttpResult BadRequest(Dictionary<string, string[]> errors)
   ```

3. **Adicionar logging no `EventBus.PublishAsync`**
   ```csharp
   try {
       // publishing logic
   } catch (Exception ex) {
       _logger.LogError(ex, "Failed to publish event {EventType}", typeof(TEvent).Name);
       throw;
   }
   ```

### Médio Prazo (1 mês)

4. **Implementar Unit of Work Pattern**
   ```csharp
   public interface IUnitOfWork
   {
       Task<Result> SaveChangesAsync(CancellationToken ct = default);
       Task<Result> SaveChangesAsync<TDomainEvent>(
           TDomainEvent domainEvent,
           CancellationToken ct = default) where TDomainEvent : IDomainEvent;
   }
   ```

5. **Implementar Domain Event Dispatcher**
   - Criar `IDomainEventDispatcher` interface
   - Implementar `DomainEventDispatcher` que usa `IPublisher`
   - Integrar via `SaveChangesInterceptor` no EF Core

6. **Substituir `Thread.Sleep` por `Task.Delay` no PersistentConnection**
   ```csharp
   await Task.Delay(backoffTime, cancellationToken);
   ```

### Longo Prazo (2-3 meses)

7. **Refatorar ValidationBehavior para retornar Result**
   - Alterar pipeline para retornar `Result<Unit>.Failure()` em vez de lançar
   - Permitir que outros behaviors processem erros de validação

8. **Adicionar FluentValidation aos DTOs de entrada**
   - Criar validators base para `CreateDTO` e `UpdateDTO`
   - Integrar ao pipeline CQRS automaticamente

9. **Configurar tolerância de clock skew no JWT**
   ```csharp
   .Net:
   options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(5);
   ```

10. **Adicionar health checks para RabbitMQ**
    ```csharp
    services.AddHealthChecks()
        .AddRabbitMQ(rabbitConnectionString, name: "rabbitmq");
    ```

---

## Score / Qualidade das Bibliotecas

### Avaliação Geral

| Biblioteca | Completude | Testabilidade | Manutenibilidade | Documentação | **Score Final** |
|------------|------------|---------------|------------------|--------------|-----------------|
| BuildingBlocks | ⭐⭐⭐⭐ (80%) | ⭐⭐⭐⭐ (75%) | ⭐⭐⭐⭐ (80%) | ⭐⭐⭐⭐⭐ (90%) | **81% (B+)** |
| BuildingBlocks.Messaging | ⭐⭐⭐ (65%) | ⭐⭐⭐ (60%) | ⭐⭐⭐⭐ (75%) | ⭐⭐⭐⭐ (70%) | **68% (C)** |
| Core.Domain | ⭐⭐⭐⭐⭐ (95%) | ⭐⭐⭐⭐⭐ (90%) | ⭐⭐⭐⭐⭐ (95%) | ⭐⭐⭐⭐ (80%) | **92% (A-)** |
| Core.Application | ⭐⭐⭐⭐ (85%) | ⭐⭐⭐⭐ (85%) | ⭐⭐⭐⭐⭐ (90%) | ⭐⭐⭐⭐⭐ (90%) | **88% (A-)** |
| Core.Infrastructure | ⭐⭐⭐⭐ (75%) | ⭐⭐⭐⭐ (70%) | ⭐⭐⭐⭐ (80%) | ⭐⭐⭐⭐ (75%) | **75% (B)** |
| Core.API | ⭐⭐⭐ (60%) | ⭐⭐⭐ (70%) | ⭐⭐⭐⭐⭐ (90%) | ⭐⭐⭐ (60%) | **70% (C+)** |

### Detalhamento por Critério

#### Completude
- **Core.Domain (95%):**patterns completos (Entity, Value Object, Specification, Repository, Domain Events)
- **BuildingBlocks (80%):** CQRS, Result Pattern, Validação implementados; falta Unit of Work
- **BuildingBlocks.Messaging (65%):** Pub/Sub implementado; falta retry policy, dead letter queue

#### Testabilidade
- **Core.Domain (90%):** entidades POCO sem dependências externas; fácil de testar
- **Core.Application (85%):** interfaces bem definidas; mocks trivias
- **BuildingBlocks.Messaging (60%):** acoplamento forte com RabbitMQ.Client dificulta unit tests

#### Manutenibilidade
- **Core.Domain (95%):** código limpo, sem dependências externas, princípios SOLID seguidos
- **Core.API (90%):** controllers minimalistas, lógica nos serviços
- **BuildingBlocks.Messaging (75%):** código procedural; refatoração para async/await beneficiaria

#### Documentação
- **BuildingBlocks (90%):** interfaces documentadas, exemplos de uso presentes
- **Core.Application (90%):** XML documentation completa
- **Core.API (60%):** falta documentação sobre convenções de API e códigos de erro

### Indicadores de Qualidade

| Métrica | Status | Observação |
|---------|--------|------------|
| Cobertura de testes | ⚠️ Não mensurado | Recomenda-se >70% para bibliotecas compartilhadas |
| SonarQube Issues | ✅ Aprovado | `SonarAnalyzer.CSharp` ativo em todos os projetos |
| Dependency Vulnerability | ✅ Nenhuma | Dependências atualizadas (Outdated: none) |
| Breaking Changes | ⚠️ Risco | Métodos como `HttpResult.BadRequest(string[])` podem quebrar consumidores |

### Recomendações de Melhoria Prioritárias

1. **Alta Prioridade**
   - Implementar Unit of Work para suportar transações distribuídas
   - Criar mecanismo de dispatch de Domain Events

2. **Média Prioridade**
   - Adicionar health checks para RabbitMQ
   - Implementar retry policy com Polly no EventBus
   - Configurar tolerância de ClockSkew no JWT

3. **Baixa Prioridade**
   - Adicionar mais exemplos de uso na documentação
   - Criar templates de测试 para novos microsserviços

---

## Dependências

### BuildingBlocks.csproj

| Pacote | Versão | Uso |
|---|---|---|
| `FluentValidation` | 12.1.1 | Validação de commands/queries |
| `FluentValidation.DependencyInjectionExtensions` | 12.1.1 | DI automático de validadores |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.3 | JWT |
| `Microsoft.AspNetCore.Http.Abstractions` | 2.3.9 | Primitivas HTTP |
| `Microsoft.Extensions.Configuration.Abstractions` | 10.0.3 | Configuração |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 10.0.3 | Injeção de dependência |
| `SonarAnalyzer.CSharp` | 10.20.0.135146 | Análise estática de qualidade |

### BuildingBlocks.Messaging.csproj

| Pacote | Versão | Uso |
|---|---|---|
| `RabbitMQ.Client` | 7.2.1 | Message broker |
| `Microsoft.Extensions.Configuration.Binder` | 10.0.3 | Bind de configuração |
| `Microsoft.Extensions.Options` | 10.0.3 | IOptions<T> |
| `Microsoft.Extensions.Logging.Abstractions` | 10.0.3 | ILogger |

### Core.Domain.csproj

| Pacote / Referência | Uso |
|---|---|
| `BuildingBlocks` | INotification para DomainEvent |
| `SonarAnalyzer.CSharp` 10.20.0.135146 | Análise estática |

### Core.Infrastructure.csproj

| Pacote | Versão | Uso |
|---|---|---|
| `Microsoft.EntityFrameworkCore` | 10.0.3 | ORM |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 10.0.3 | Identity |
| `Newtonsoft.Json` | 13.0.4 | Serialização JSON |
