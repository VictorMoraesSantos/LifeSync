# 💰 Financial Service

Microserviço responsável pelo **controle financeiro pessoal** no LifeSync.

> **Stack:** ASP.NET Core · EF Core · PostgreSQL · Background Service  
> **Porta:** `5003` · **Schema:** `financial`
> **Padrões:** CQRS · DDD · Result Pattern · Specification · Clean Architecture

## Índice

- [Visão Geral](#visão-geral)
- [Estrutura de Pastas](#estrutura-de-pastas)
- [Domínio](#domínio)
- [Aplicação](#aplicação)
- [Infraestrutura](#infraestrutura)
- [API](#api)
- [Configuração](#configuração)
- [Dependências](#dependências)
- [📚 Documentação Relacionada](#-documentação-relacionada)

---

## Visão Geral

O Financial Service permite que usuários controlem suas finanças pessoais — registrando receitas, despesas e categorizando transações. Suporta múltiplos métodos de pagamento, transações recorrentes com agendamento automático e mais de 140 moedas internacionais através do value object `Money`.

### Responsabilidades

- CRUD de transações financeiras (receitas e despesas)
- CRUD de categorias personalizadas por usuário
- Suporte a múltiplos métodos de pagamento e moedas
- Agendamento de transações recorrentes (`RecurrenceSchedule`) com geração automática via Background Service
- Filtros avançados por tipo, categoria, período e valor
- Relatórios financeiros (placeholder para implementação futura)

---

## Estrutura de Pastas

```
Financial/
├── Financial.API/
│   ├── Controllers/
│   │   ├── CategoriesController.cs
│   │   ├── TransactionsController.cs
│   │   ├── RecurrenceScheduleController.cs
│   │   └── ReportsController.cs         # Placeholder (vazio)
│   ├── Program.cs
│   ├── appsettings.json
│   └── Financial.API.csproj
├── Financial.Application/
│   ├── Contracts/
│   │   ├── ICategoryService.cs
│   │   ├── ITransactionService.cs
│   │   └── IRecurrenceScheduleService.cs
│   ├── DTOs/
│   │   ├── Category/
│   │   │   ├── CategoryDTO.cs
│   │   │   ├── CategoryFilterDTO.cs
│   │   │   ├── CreateCategoryDTO.cs
│   │   │   └── UpdateCategoryDTO.cs
│   │   ├── Transaction/
│   │   │   ├── TransactionDTO.cs
│   │   │   ├── TransactionFilterDTO.cs
│   │   │   ├── CreateTransactionDTO.cs
│   │   │   └── UpdateTransactionDTO.cs
│   │   ├── RecurrenceSchedule/
│   │   │   ├── RecurrenceScheduleDTO.cs
│   │   │   ├── RecurrenceScheduleFilterDTO.cs
│   │   │   ├── CreateRecurrenceScheduleDTO.cs
│   │   │   └── UpdateRecurrenceScheduleDTO.cs
│   │   └── Report/
│   │       ├── UserBalanceSummaryDTO.cs
│   │       └── AccountBalanceDTO.cs
│   ├── Features/
│   │   ├── Categories/
│   │   │   ├── Commands/        # Create, Update, Delete
│   │   │   └── Queries/         # GetAll, GetById, GetByUser, GetByFilter
│   │   ├── Transactions/
│   │   │   ├── Commands/        # Create, Update, Delete
│   │   │   └── Queries/         # GetAll, GetById, GetByUser, GetByFilter
│   │   └── RecurrenceSchedules/
│   │       ├── Commands/        # Update, Delete, Deactivate
│   │       └── Queries/         # GetById, GetByUserId, GetByFilter
│   ├── Mappings/
│   │   ├── CategoryMapper.cs
│   │   ├── TransactionMapper.cs
│   │   └── RecurrencyScheduleMapper.cs
│   └── Financial.Application.csproj
├── Financial.Domain/
│   ├── Entities/
│   │   ├── Category.cs
│   │   ├── Transaction.cs
│   │   └── RecurrenceSchedule.cs
│   ├── Enums/
│   │   ├── TransactionType.cs
│   │   ├── PaymentMethod.cs
│   │   ├── Currency.cs
│   │   └── RecurrenceFrequency.cs
│   ├── Errors/
│   │   ├── CategoryErrors.cs
│   │   ├── TransactionErrors.cs
│   │   └── RecurrenceScheduleErrors.cs
│   ├── Filters/
│   │   ├── CategoryQueryFilter.cs
│   │   ├── TransactionQueryFilter.cs
│   │   ├── RecurrenceScheduleQueryFilter.cs
│   │   └── Specifications/
│   │       ├── CategorySpecification.cs
│   │       ├── TransactionSpecification.cs
│   │       └── RecurrenceScheduleSpecification.cs
│   ├── Repositories/
│   │   ├── ICategoryRepository.cs
│   │   ├── ITransactionRepository.cs
│   │   └── IRecurrenceScheduleRepository.cs
│   ├── ValueObjects/Money.cs
│   └── Financial.Domain.csproj
└── Financial.Infrastructure/
    ├── BackgroundServices/
    │   └── RecurrenceProcessorService.cs
    ├── Configuration/TransactionConfiguration.cs
    ├── Persistence/
    │   ├── ApplicationDbContext.cs
    │   ├── MigrationHostedService.cs
    │   └── Repositories/
    │       ├── CategoryRepository.cs
    │       ├── TransactionRepository.cs
    │       └── RecurrenceScheduleRepository.cs
    ├── Services/
    │   ├── CategoryService.cs
    │   ├── TransactionService.cs
    │   └── RecurrenceScheduleService.cs
    ├── Migrations/
    └── Financial.Infrastructure.csproj
```

---

## Domínio

### Entidade: `Category`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Regras |
|---|---|---|
| `UserId` | `int` | Obrigatório, > 0 |
| `Name` | `string` | Obrigatório, não pode ser vazio |
| `Description` | `string?` | Opcional |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `Update(name, description?)` | Atualiza nome e descrição, marca como atualizado |

---

### Entidade: `Transaction`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Regras |
|---|---|---|
| `UserId` | `int` | Obrigatório, > 0 |
| `CategoryId` | `int?` | Opcional, FK para Category, > 0 se informado |
| `Category` | `Category?` | Navigation property |
| `PaymentMethod` | `PaymentMethod` | Enum obrigatório |
| `TransactionType` | `TransactionType` | Enum obrigatório |
| `Amount` | `Money` | Value object, não pode ser nulo |
| `Description` | `string` | Obrigatório, não pode ser vazio |
| `TransactionDate` | `DateTime` | Convertida para UTC automaticamente |
| `IsRecurring` | `bool` | Default `false` |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `Update(categoryId?, paymentMethod, transactionType, amount, description, transactionDate, isRecurring)` | Atualiza todos os campos com validação |

---

### Entidade: `RecurrenceSchedule`

Herda de `BaseEntity<int>`. Representa o agendamento de uma transação recorrente — armazena a configuração de repetição e gera cópias da transação original automaticamente.

| Propriedade | Tipo | Regras |
|---|---|---|
| `TransactionId` | `int` | Obrigatório, > 0, FK para Transaction |
| `Transaction` | `Transaction` | Navigation property |
| `Frequency` | `RecurrenceFrequency` | Enum obrigatório |
| `StartDate` | `DateTime` | Obrigatório |
| `EndDate` | `DateTime?` | Opcional, deve ser posterior a StartDate |
| `NextOccurrence` | `DateTime` | Próxima data de geração |
| `MaxOccurrences` | `int?` | Opcional, > 0 |
| `OccurrencesGenerated` | `int` | Contador de transações geradas (default 0) |
| `IsActive` | `bool` | Indica se o agendamento está ativo |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `Update(frequency, endDate?, maxOccurrences?)` | Atualiza frequência e limites com validação |
| `GenerateTransaction()` | Gera cópia da transação original na data `NextOccurrence`, incrementa contador e calcula próxima ocorrência. Desativa automaticamente ao atingir `MaxOccurrences` ou `EndDate` |
| `CanGenerateNext()` | Verifica se pode gerar a próxima transação (ativo, dentro dos limites) |
| `Activate()` | Ativa o agendamento |
| `Deactivate()` | Desativa o agendamento |

**Regras de negócio:**
- Uma Transaction só pode ter um RecurrenceSchedule associado
- A Transaction deve ter `IsRecurring = true` para criar um schedule
- O schedule se desativa automaticamente quando `MaxOccurrences` é atingido ou `NextOccurrence` ultrapassa `EndDate`

---

### Value Object: `Money`

Record type imutável para representar valores monetários.

| Propriedade | Tipo | Regras |
|---|---|---|
| `Amount` | `int` | Valor em centavos, não negativo |
| `Currency` | `Currency` | Enum de moeda (140+ moedas) |

**Método factory:** `Money.Create(amount, currency)` — valida os parâmetros antes de criar.

---

### Enumerações

#### `TransactionType`

| Valor | Int | Nome PT |
|---|---|---|
| `Income` | 1 | Renda |
| `Expense` | 2 | Despesa |

#### `PaymentMethod`

| Valor | Int |
|---|---|
| `Cash` | 1 |
| `CreditCard` | 2 |
| `DebitCard` | 3 |
| `BankTransfer` | 4 |
| `Pix` | 5 |
| `Other` | 6 |

#### `RecurrenceFrequency`

| Valor | Int |
|---|---|
| `Daily` | 1 |
| `Weekly` | 2 |
| `Monthly` | 3 |
| `Yearly` | 4 |

Extensão `ToFriendlyString()` retorna o nome em inglês.

#### `Currency`

Suporte a 140+ moedas internacionais. Exemplos:

| Símbolo | Moeda |
|---|---|
| `BRL` | Real Brasileiro (R$) |
| `USD` | Dólar Americano ($) |
| `EUR` | Euro (€) |
| `GBP` | Libra Esterlina (£) |
| `JPY` | Iene Japonês (¥) |

Extensão `ToSymbol()` converte para símbolo da moeda.

---

### Erros de Domínio

#### `CategoryErrors`

| Erro | Mensagem |
|---|---|
| `InvalidId` | O ID da categoria deve ser maior que zero |
| `InvalidName` | O nome da categoria é obrigatório |
| `InvalidUserId` | O ID do usuário deve ser maior que zero |
| `NotFound(id)` | Categoria com ID {id} não encontrada |
| `DuplicateName` | Já existe uma categoria com este nome |
| `CategoryInUse` | Não é possível excluir categoria que está sendo utilizada |
| `CreateError`, `UpdateError`, `DeleteError` | Erros de operação |

#### `TransactionErrors`

| Erro | Mensagem |
|---|---|
| `InvalidId` | O ID da transação deve ser maior que zero |
| `InvalidUserId` | O ID do usuário deve ser maior que zero |
| `InvalidAmount` | O valor da transação é obrigatório |
| `InvalidDescription` | A descrição da transação é obrigatória |
| `FutureTransactionDate` | A data da transação não pode ser no futuro |
| `NegativeAmount` | O valor da transação deve ser positivo |
| `CategoryNotFound` | Categoria não encontrada |
| `InvalidPaymentMethod` | Método de pagamento inválido |
| `InvalidTransactionType` | Tipo de transação inválido |
| `NotFound(id)` | Transação com ID {id} não encontrada |
| `CreateError`, `UpdateError`, `DeleteError` | Erros de operação |

---

#### `RecurrenceScheduleErrors`

| Erro | Mensagem |
|---|---|
| `InvalidId` | O ID do agendamento deve ser maior que zero |
| `InvalidFrequency` | A frequência de recorrência é inválida |
| `InvalidStartDate` | A data de início é obrigatória |
| `EndDateBeforeStartDate` | A data final deve ser posterior à data de início |
| `InvalidMaxOccurrences` | O número máximo de ocorrências deve ser maior que zero |
| `InvalidTransaction` | A transação de origem é obrigatória |
| `NotFound(id)` | Agendamento com ID {id} não encontrado |
| `InactiveSchedule` | Este agendamento está inativo |
| `MaxOccurrencesReached` | Número máximo de ocorrências atingido |
| `TransactionNotLoaded` | A transação de origem não foi carregada |
| `TransactionNotRecurring` | A transação de origem não está marcada como recorrente |
| `ScheduleAlreadyExists` | Já existe um agendamento para esta transação |
| `CreateError`, `UpdateError`, `DeleteError` | Erros de operação |

---

### Filtros e Especificações

#### `CategoryQueryFilter`

`Id`, `UserId`, `NameContains`, `DescriptionContains`, `CreatedAt`, `UpdatedAt`, `IsDeleted`, `SortBy`, `SortDesc`, `Page`, `PageSize`

#### `TransactionQueryFilter`

`Id`, `UserId`, `CategoryId`, `PaymentMethod`, `TransactionType`, `AmountEquals/GreaterThan/LessThan`, `CurrencyEquals`, `DescriptionContains`, `TransactionDate`, `TransactionDateFrom`, `TransactionDateTo`, paginação

#### `RecurrenceScheduleQueryFilter`

`Id`, `TransactionId`, `UserId`, `Frequency`, `IsActive`, `StartDateFrom`, `StartDateTo`, `CreatedAt`, `UpdatedAt`, `IsDeleted`, `SortBy`, `SortDesc`, `Page`, `PageSize`

---

## Aplicação

### Commands — Category

| Command | Retorno | Validação | Descrição |
|---|---|---|---|
| `CreateCategoryCommand(UserId, Name, Description?)` | `CreateCategoryResult(int Id)` | Nome: 2-50 chars; Descrição: max 200 | Cria categoria |
| `UpdateCategoryCommand(Id, Name, Description?)` | `UpdateCategoryResult(bool)` | Mesmo que Create | Atualiza |
| `DeleteCategoryCommand(Id)` | `DeleteCategoryResult(bool)` | — | Remove |

### Commands — RecurrenceSchedule

| Command | Retorno | Descrição |
|---|---|---|
| `UpdateRecurrenceScheduleCommand(Id, Frequency, EndDate?, MaxOccurrences?)` | `UpdateRecurrenceScheduleResult(bool)` | Atualiza frequência e limites |
| `DeleteRecurrenceScheduleCommand(Id)` | `DeleteRecurrenceScheduleResult(bool)` | Remove agendamento |
| `DeactivateRecurrenceScheduleCommand(Id)` | `DeactivateRecurrenceScheduleResult(bool)` | Desativa agendamento |

> **Nota:** A criação do `RecurrenceSchedule` é feita automaticamente pelo `CreateTransactionCommandHandler` quando `IsRecurring = true` e os dados de recorrência são informados.

### Commands — Transaction

| Command | Retorno | Validação | Descrição |
|---|---|---|---|
| `CreateTransactionCommand(UserId, CategoryId?, PaymentMethod, TransactionType, Amount, Description, TransactionDate, IsRecurring)` | `CreateTransactionResult(int TransactionId)` | CategoryId > 0, Amount > 0, Descrição max 200, Data não futura | Cria transação |
| `UpdateTransactionCommand(Id, ...)` | `UpdateTransactionResult(bool)` | Mesmo que Create | Atualiza |
| `DeleteTransactionCommand(Id)` | `DeleteTransactionResult(bool)` | — | Remove |

---

### Queries — Category

| Query | Retorno | Descrição |
|---|---|---|
| `GetAllCategoriesQuery` | `IEnumerable<CategoryDTO>` | Todas as categorias |
| `GetCategoryByIdQuery(id)` | `CategoryDTO` | Categoria por ID |
| `GetCategoriesByUserIdQuery(userId)` | `IEnumerable<CategoryDTO>` | Categorias do usuário |
| `GetCategoriesByFilterQuery(filter)` | Paginado | Filtro avançado |

### Queries — Transaction

| Query | Retorno | Descrição |
|---|---|---|
| `GetAllTransactionsQuery` | `IEnumerable<TransactionDTO>` | Todas as transações |
| `GetTransactionByIdQuery(id)` | `TransactionDTO` | Transação por ID |
| `GetTransactionsByUserIdQuery(userId)` | `IEnumerable<TransactionDTO>` | Transações do usuário |
| `GetTransactionsByFilterQuery(filter)` | Paginado | Filtro avançado |

### Queries — RecurrenceSchedule

| Query | Retorno | Descrição |
|---|---|---|
| `GetRecurrenceScheduleByIdQuery(id)` | `RecurrenceScheduleDTO` | Agendamento por ID |
| `GetRecurrenceScheduleByUserIdQuery(userId)` | `IEnumerable<RecurrenceScheduleDTO>` | Agendamentos do usuário |
| `GetRecurrenceScheduleByFilterQuery(filter)` | Paginado | Filtro avançado |

---

### DTOs

#### `CategoryDTO`
```
CategoryDTO(Id, UserId, CreatedAt, UpdatedAt, Name, Description?)
```

#### `TransactionDTO`
```
TransactionDTO(Id, UserId, Category: CategoryDTO, CreatedAt, UpdatedAt, PaymentMethod, TransactionType, Amount: Money, Description, TransactionDate, IsRecurring)
```

#### `CreateTransactionDTO`
```
CreateTransactionDTO(UserId, CategoryId?, PaymentMethod, TransactionType, Amount: Money, Description, TransactionDate, IsRecurring = false)
```

#### `TransactionFilterDTO`
```
TransactionFilterDTO(Id?, UserId?, CategoryId?, PaymentMethod?, TransactionType?, AmountEquals?, AmountGreaterThan?, AmountLessThan?, CurrencyEquals?, DescriptionContains?, TransactionDate?, TransactionDateFrom?, TransactionDateTo?, ...paginação)
```

#### `RecurrenceScheduleDTO`
```
RecurrenceScheduleDTO(Id, TransactionId, Transaction: TransactionDTO, CreatedAt, UpdatedAt, Frequency, StartDate, EndDate, NextOccurrence, MaxOccurrences?, OccurrencesGenerated, IsActive)
```

#### `CreateRecurrenceScheduleDTO`
```
CreateRecurrenceScheduleDTO(TransactionId, Frequency, StartDate, EndDate?, MaxOccurrences?)
```

#### `UpdateRecurrenceScheduleDTO`
```
UpdateRecurrenceScheduleDTO(Id, Frequency, EndDate?, MaxOccurrences?)
```

#### `RecurrenceScheduleFilterDTO`
```
RecurrenceScheduleFilterDTO(Id?, TransactionId?, UserId?, Frequency?, IsActive?, StartDateFrom?, StartDateTo?, CreatedAt?, UpdatedAt?, IsDeleted?, SortBy?, SortDesc?, Page?, PageSize?)
```

#### DTOs de Relatório (estrutura futura)
```
UserBalanceSummaryDTO(TotalBalance, Currency, AccountBalances: IEnumerable<AccountBalanceDTO>)
AccountBalanceDTO(AccountId, AccountName, Balance, Currency)
```

---

### Contratos de Serviço

#### `ICategoryService`

```
GetByIdAsync(id)
GetAllAsync()
GetByUserIdAsync(userId)
GetByNameAsync(name, userId)
GetByFilterAsync(filter)
GetPagedAsync(page, pageSize)
FindAsync(predicate)
CountAsync(predicate?)
CreateAsync(dto)
CreateRangeAsync(dtos)
UpdateAsync(dto)
DeleteAsync(id)
DeleteRangeAsync(ids)
```

#### `ITransactionService`

```
GetByIdAsync(id)
GetAllAsync()
GetByUserIdAsync(userId)
GetByFilterAsync(filter)
GetPagedAsync(page, pageSize)
FindAsync(predicate)
CountAsync(predicate?)
CreateAsync(dto)
CreateRangeAsync(dtos)
UpdateAsync(dto)
DeleteAsync(id)
DeleteRangeAsync(ids)
```

#### `IRecurrenceScheduleService`

```
GetByIdAsync(id)
GetAllAsync()
GetByFilterAsync(filter)
GetPagedAsync(page, pageSize)
FindAsync(predicate)
CountAsync(predicate?)
CreateAsync(dto)
CreateRangeAsync(dtos)
UpdateAsync(dto)
DeleteAsync(id)
DeleteRangeAsync(ids)
GetByTransactionIdAsync(transactionId)
GetActiveByUserIdAsync(userId)
DeactiveScheduleAsync(id)
ActiveScheduleAsync(id)
ProcessDueSchedulesAsync()
```

---

## Infraestrutura

### `ApplicationDbContext`

| DbSet | Tipo |
|---|---|
| `Categories` | `DbSet<Category>` |
| `Transactions` | `DbSet<Transaction>` |
| `RecurrenceSchedules` | `DbSet<RecurrenceSchedule>` |

### `TransactionConfiguration`

O value object `Money` é configurado como owned entity usando `OwnsOne`:
- `Amount.Amount` → coluna `Amount`
- `Amount.Currency` → coluna `Currency`

### Migrations

| Migration | Data |
|---|---|
| `20250609025157_initialCreate` | 2025-06-09 |
| `20250609025413_initialCreate321123` | 2025-06-09 |
| `20260314214026_add_recurrence_schedule` | 2026-03-14 |

### `IRecurrenceScheduleRepository` — Métodos customizados

```
GetByTransactionId(transactionId, cancellationToken)   → RecurrenceSchedule
GetActiveByUserId(userId, cancellationToken)            → IEnumerable<RecurrenceSchedule>
GetDueSchedules(referenceDate, cancellationToken)       → IEnumerable<RecurrenceSchedule>
```

### Background Services

#### `RecurrenceProcessorService`

`BackgroundService` que executa a cada **1 hora** e processa agendamentos pendentes:

1. Busca schedules com `NextOccurrence <= DateTime.UtcNow` via `GetDueSchedules()`
2. Para cada schedule, chama `GenerateTransaction()` em loop enquanto `CanGenerateNext()` e `NextOccurrence <= now`
3. Persiste as transações geradas e atualiza o schedule (incrementa `OccurrencesGenerated`, recalcula `NextOccurrence`)

Registrado em `DependencyInjection.cs` via `services.AddHostedService<RecurrenceProcessorService>()`.

### `ICategoryRepository` — Métodos customizados

```
GetAllByUserId(userId, cancellationToken)    → IEnumerable<Category>
GetByNameContains(name, cancellationToken)   → IEnumerable<Category>
```

### `ITransactionRepository` — Métodos customizados

```
GetByUserIdAsync(userId, startDate, endDate, categoryId?, type?)  → IEnumerable<Transaction>
```

---

## API

### `CategoriesController` — `/api/categories`

| Método | Rota | Body / Params | Retorno |
|---|---|---|---|
| GET | `/{id}` | `id` | `CategoryDTO` |
| GET | `/user/{userId}` | `userId` | `IEnumerable<CategoryDTO>` |
| GET | `/search` | Filtros (query) | Paginado |
| GET | `/` | — | `IEnumerable<CategoryDTO>` |
| POST | `/` | `CreateCategoryCommand` | `{ id }` |
| PUT | `/{id}` | `UpdateCategoryCommand` | `{ isSuccess }` |
| DELETE | `/{id}` | `id` | `204` |

**Parâmetros de busca (`/search`):**
`id`, `userId`, `nameContains`, `descriptionContains`, `createdAt`, `updatedAt`, `isDeleted`, `sortBy`, `sortDesc`, `page`, `pageSize`

---

### `TransactionsController` — `/api/transactions`

| Método | Rota | Body / Params | Retorno |
|---|---|---|---|
| GET | `/{id}` | `id` | `TransactionDTO` |
| GET | `/user/{id}` | `userId` | `IEnumerable<TransactionDTO>` |
| GET | `/search` | Filtros (query) | Paginado |
| GET | `/` | — | `IEnumerable<TransactionDTO>` |
| POST | `/` | `CreateTransactionCommand` | `{ transactionId }` |
| PUT | `/{id}` | `UpdateTransactionCommand` | `{ isSuccess }` |
| DELETE | `/{id}` | `id` | `204` |

**Parâmetros de busca (`/search`):**
`id`, `userId`, `categoryId`, `paymentMethod`, `transactionType`, `amountEquals`, `amountGreaterThan`, `amountLessThan`, `currencyEquals`, `descriptionContains`, `transactionDate`, `transactionDateFrom`, `transactionDateTo`, `createdAt`, `updatedAt`, `isDeleted`, `sortBy`, `sortDesc`, `page`, `pageSize`

---

### `RecurrenceScheduleController` — `/api/recurrenceschedule`

| Método | Rota | Body / Params | Retorno |
|---|---|---|---|
| GET | `/{id}` | `id` | `RecurrenceScheduleDTO` |
| GET | `/user/{userId}` | `userId` | `IEnumerable<RecurrenceScheduleDTO>` |
| GET | `/search` | Filtros (query) | Paginado |
| PUT | `/{id}` | `UpdateRecurrenceScheduleCommand` | `{ isSuccess }` |
| PATCH | `/{id}/deactivate` | `id` | `{ isSuccess }` |
| DELETE | `/{id}` | `id` | `204` |

**Parâmetros de busca (`/search`):**
`id`, `transactionId`, `userId`, `frequency`, `isActive`, `startDateFrom`, `startDateTo`, `createdAt`, `updatedAt`, `isDeleted`, `sortBy`, `sortDesc`, `page`, `pageSize`

> **Nota:** A criação do RecurrenceSchedule não possui endpoint dedicado — é criado automaticamente pelo `CreateTransactionCommandHandler` quando a transação é marcada como recorrente.

---

### `ReportsController`

Placeholder para implementação futura de relatórios financeiros.

---

### Health Check

```
GET /health
→ { "status": "healthy", "service": "Financial", "timestamp": "...", "environment": "..." }
```

---

## Testes

### Projetos de Teste

| Projeto | Tipo | Status |
|---|---|---|
| `Financial.UnitTests` | Unitários | 12+ testes para Category, 20+ para Transaction |
| `Financial.IntegrationTests` | Integração | Stub vazio (não implementado) |

**Framework:** xUnit, assertions nativas (`Assert.*`)

### Cobertura

- **Category:** Criação válida/inválida, atualização, exceções de validação
- **Transaction:** Construtor, Update, validação de Money, regras de negócio

```bash
dotnet test tests/Financial.UnitTests
```

---

## Problemas Críticos

> **Fonte:** Code Review Completo (FINANCIAL_CODE_REVIEW.md)  
> **Data da Análise:** 03/03/2026  
> **Nota Geral:** 5.5/10

### Resumo de Issues por Severidade

| Severidade | Quantidade |
|------------|-----------|
| CRÍTICO | 7 |
| ALTO | 8 |
| MÉDIO | 5 |
| BAIXO | 0 |
| INFO | 1 |

### Tabela Completa de Issues

#### CRÍTICO

| # | Arquivo | Problema | Impacto |
|---|---------|----------|---------|
| 1 | `Transaction.cs` | Bug de dupla atribuição no `Update()`: `Amount` atribuído duas vezes + typo `SetAmout` | Setter privado perde propósito; código confuso |
| 2 | `Money.cs` | Valor negativo não permitido (`amount < 0` lança exceção) | Impossível representar despesas; contradição com `TransactionType.Expense` |
| 3 | `CategoryService.cs` | Paginação em memória - `GetPagedAsync` carrega TODOS os registros | Performance O(n) - 100k registros em memória |
| 4 | `CategoryService.cs` | `FindAsync` carrega tudo e filtra em memória sobre DTOs | Performance O(n) - carrega todos, converte, depois filtra |
| 5 | `CategoryService.cs` | N+1 em `DeleteRangeAsync` - loop com `GetById()` para cada ID | N queries ao banco para deletar N registros |
| 6 | `CategoriesController.cs` | Sem `[Authorize]` em nenhum controller | Todos os endpoints são públicos - sem autenticação |
| 7 | `appsettings.json` | JWT Secret hardcoded `"SuperSecretKeyForJWTAuthentication2024!@#$%"` | Segurança comprometida em produção |

#### ALTO

| # | Arquivo | Problema | Impacto |
|---|---------|----------|---------|
| 8 | `Category.cs`, `Transaction.cs` | Inconsistência de exceções - `DomainException` vs `ArgumentOutOfRangeException` | Handlers capturam `Exception` genérica e mascaram o tipo real |
| 9 | `CreateTransactionCommandValidator.cs` | Validador `.Must(amount => amount.Amount > 0 \|\| amount.Amount < 0)` SEMPRE retorna true | Validação ineficaz - qualquer valor é aceito |
| 10 | `CategoryService.cs` | `GetByNameAsync` valida `userId` mas IGNORA na query | Retorna categorias de TODOS os usuários - vazamento de dados |
| 11 | `ApplicationDbContext.cs` | Índices faltando: `Categories.UserId`, `Transactions.UserId`, `Transactions.TransactionDate`, `Transactions.PaymentMethod` | Queries lentas sem índices adequados |
| 12 | `ApplicationDbContext.cs` | Ausência de Global Query Filter para `IsDeleted` | Registros deletados aparecem em todas as queries |
| 13 | `TransactionMapper.cs` | `Category` mapeado como nullable no mapper, mas `TransactionDTO` declara como non-nullable | Potencial `NullReferenceException` |
| 14 | `CategoriesController.cs` | `GetAll()` retorna dados de TODOS os usuários | Sem filtro por usuário autenticado |

#### MÉDIO

| # | Arquivo | Problema | Impacto |
|---|---------|----------|---------|
| 15 | `CategoriesController.cs` | Comparação de erros por string (`result.Error.Description.Contains("NotFound")`) | Frágil - se mensagem mudar, mapeamento de status HTTP quebra |
| 16 | `CategoryService.cs` | `AccountBalanceDTO` e `UserBalanceSummaryDTO` definidos mas nunca utilizados | DTOs órfãos no código |
| 17 | `ReportsController.cs` | Controller vazio (stub sem implementação) | Confunde desenvolvedores; não cumpre promessa de funcionalidade |
| 18 | `TransactionConfiguration.cs` | Money `OwnsOne` sem `HasPrecision()` - usa default (18,2) | Para valores financeiros, deveria ser (18,4) ou mais |
| 19 | `Transaction.cs` | Documentação ausente da estratégia (Amount positivo + `TransactionType` indica direção) | Contradição entre `Money` não permitir negativo e domínio financeiro |

#### INFO

| # | Arquivo | Problema | Impacto |
|---|---------|----------|---------|
| 20 | `TransactionTests.cs`, `CategoryTests.cs` | Bons testes unitários (20+ casos) mas usando `Assert` nativo | Recomenda migração para FluentAssertions |

---

## Recomendações de Correção

### Prioridade 1 — Críticos (7 items)

| # | Ação | Esforço | Arquivo(s) |
|---|------|---------|------------|
| 1 | Adicionar `[Authorize]` em todos os controllers | 10 min | `*Controller.cs` |
| 2 | Remover JWT Key do `appsettings.json` e usar Secret Manager ou env vars | 30 min | `appsettings.json`, `Program.cs` |
| 3 | Corrigir paginação em memória — usar `IQueryable.Skip().Take()` no banco | 2h | `CategoryService.cs`, `TransactionService.cs` |
| 4 | Corrigir `FindAsync` para filtrar no banco — traduzir predicado de DTO para Entity | 2h | `CategoryService.cs`, `TransactionService.cs` |
| 5 | Corrigir N+1 no `DeleteRangeAsync` — criar `GetByIds(IEnumerable<int>)` no repository | 1h | `CategoryRepository.cs`, `CategoryService.cs` |
| 6 | Corrigir bug dupla atribuição no `Transaction.Update()` — remover `Amount = amount` duplicado e corrigir typo `SetAmout` → `SetAmount` | 15 min | `Transaction.cs` |
| 7 | Corrigir validador `.Must()` que sempre retorna true — usar `.Must(amount => amount.Amount != 0)` | 10 min | `CreateTransactionCommandValidator.cs` |

### Prioridade 2 — Altos (7 items)

| # | Ação | Esforço | Arquivo(s) |
|---|------|---------|------------|
| 8 | Filtrar `GetByNameAsync` por `userId` — corrigir query para usar parâmetro | 30 min | `CategoryService.cs`, `CategoryRepository.cs` |
| 9 | Adicionar índices no banco: `Categories.UserId`, `Transactions.UserId`, `Transactions.TransactionDate`, `Transactions.PaymentMethod` | 30 min | `ApplicationDbContext.cs` |
| 10 | Adicionar Global Query Filter para `IsDeleted` em todas as entidades | 30 min | `ApplicationDbContext.cs` |
| 11 | Corrigir `TransactionDTO` nullability — `Category` deve ser nullable ou mapper deve garantir valor | 30 min | `TransactionMapper.cs`, `TransactionDTO.cs` |
| 12 | Filtrar `GetAll` por usuário autenticado — extrair `UserId` do JWT | 1h | `CategoriesController.cs`, `TransactionsController.cs` |
| 13 | Padronizar exceções de domínio — usar `DomainException` em todas as entidades | 1h | `Category.cs`, `Transaction.cs` |
| 14 | Implementar testes de integração (~40 testes) | 8h | `Financial.IntegrationTests/*` |

### Prioridade 3 — Médios (5 items)

| # | Ação | Esforço | Arquivo(s) |
|---|------|---------|------------|
| 15 | Usar `ErrorType` ao invés de comparação por string (`result.Error.Type == ErrorType.NotFound`) | 1h | `CategoriesController.cs` |
| 16 | Remover DTOs não utilizados (`AccountBalanceDTO`, `UserBalanceSummaryDTO`) | 15 min | `Report/*.cs` |
| 17 | Implementar ou remover `ReportsController` — não deixar stub vazio | 2h | `ReportsController.cs` |
| 18 | Definir precisão decimal para Money `(18, 4)` no EF Core | 15 min | `TransactionConfiguration.cs` |
| 19 | Documentar estratégia de Amount positivo + `TransactionType` indica direção, ou permitir valores negativos no Money | 30 min | `Money.cs`, `Transaction.cs` |

---

## Score / Qualidade do Serviço

### Nota Geral: **5.5/10**

| Dimensão | Nota | Observações |
|----------|------|-------------|
| **Arquitetura** | 7/10 | Clean Architecture bem aplicada; CQRS implementado; Repository Pattern correto |
| **Domínio** | 6/10 | Entidades bem definidas; Value Objects corretos; bugs críticos de lógica presentes |
| **Infraestrutura** | 4/10 | Performance crítica (paginação em memória, N+1); ausência de índices; sem Global Query Filter |
| **API/Segurança** | 3/10 | Sem autenticação; JWT hardcoded; vazamento de dados entre usuários |
| **Testes** | 4/10 | Testes unitários bons (24 casos); integração ausente; cobertura insuficiente |
| **Código Limpo** | 6/10 | Typos, inconsistências de exceções, DTOs órfãos |

### Principais Pontos Fortes

- Estrutura de Clean Architecture bem definida e seguida
- CQRS implementado com Commands e Queries separados
- Value Objects (`Money`) e Domain Errors corretamente implementados
- Testes unitários de domínio com boa cobertura de casos
- Background Service para processamento de recorrências

### Principais Pontos Fracos

- **Segurança crítica:** endpoints públicos + JWT exposto + vazamento de dados
- **Performance crítica:** paginação e filtragem em memória com O(n) total
- **Bugs de lógica:** validador sempre verdadeiro + dupla atribuição + contradição de domínio
- **Testes incompletos:** sem integração e E2E
- **Código técnico:** inconsistências de padrão de exceções

### Plano de Ação Resumido

```
SEMANA 1 (Críticos):
├── Adicionar [Authorize] nos controllers
├── Remover JWT hardcoded
├── Corrigir paginação em memória
├── Corrigir N+1 em DeleteRange
└── Corrigir bug dupla atribuição

SEMANA 2 (Altos):
├── Filtrar queries por UserId
├── Adicionar índices no banco
├── Global Query Filter para IsDeleted
├── Corrigir nullability de Category no DTO
└── Iniciar testes de integração

SEMANA 3 (Melhorias):
├── Migrar testes para FluentAssertions
├── Implementar relatórios (ou remover stubs)
├── Padronizar exceções de domínio
└── Documentar estratégia de valores negativos
```

---

## Problemas Conhecidos

| Severidade | Problema | Descrição |
|---|---|---|
| CRÍTICO | Controllers sem `[Authorize]` | Todos os endpoints são públicos — sem autenticação |
| CRÍTICO | Paginação em memória | `GetPagedAsync` carrega todos os registros e pagina em memória |
| CRÍTICO | `FindAsync` carrega tudo | Busca todos os registros, filtra em memória |
| CRÍTICO | N+1 em `DeleteRangeAsync` | Loop chama `GetById()` para cada ID |
| CRÍTICO | Validator sempre true | `amount > 0 \|\| amount < 0` sempre é verdadeiro |
| ALTO | `GetByNameAsync` ignora UserId | Valida `userId` mas não usa na query — retorna dados de todos os usuários |
| ALTO | `GetAll()` sem filtro de usuário | Retorna dados de todos os usuários |
| ALTO | Índices faltando | `UserId`, `TransactionDate`, `PaymentMethod` sem índice |
| ALTO | Sem global query filter para soft delete | Registros deletados aparecem nas queries |
| ALTO | `TransactionDTO.Category` non-nullable | Pode receber `null` do mapper |
| MÉDIO | DTOs de Report não utilizados | `UserBalanceSummaryDTO`, `AccountBalanceDTO` definidos mas não usados |
| MÉDIO | `ReportsController` vazio | Placeholder sem implementação |
| MÉDIO | Money sem `HasPrecision()` | Usa precisão padrão (18,2) ao invés de (18,4) |
| MÉDIO | Typo `SetAmout` | Método privado com nome incorreto (deveria ser `SetAmount`) |

---

## Configuração

```json
{
  "ConnectionStrings": {
    "Database": "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=LifeSync;Include Error Detail=true;"
  },
  "JwtSettings": {
    "Key": "SuperSecretKeyForJWTAuthentication2024!@#$%",
    "Issuer": "LifeSyncAPI",
    "Audience": "LifeSyncApp",
    "ExpiryMinutes": 60
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
| `Swashbuckle.AspNetCore` | 10.1.0 | Swagger |

### Referências Internas

| Projeto | Uso |
|---|---|
| `BuildingBlocks` | CQRS, Result, Validation |
| `Core.Domain` | BaseEntity, ValueObject, Specification |
| `Core.API` | ApiController base |

### Schema do Banco

```
Categories              Transactions                        RecurrenceSchedules
──────────────────      ─────────────────────────────       ─────────────────────────────
Id (PK)                 Id (PK)                             Id (PK)
UserId                  UserId                              TransactionId (FK) ──→ Transactions
Name                    CategoryId (FK, nullable) ──→ Categories   Frequency (int enum)
Description?            PaymentMethod (int enum)            StartDate
CreatedAt               TransactionType (int enum)          EndDate?
UpdatedAt               Amount (int — centavos)             NextOccurrence
IsDeleted               Currency (int enum)                 MaxOccurrences?
                        Description                         OccurrencesGenerated
                        TransactionDate                     IsActive
                        IsRecurring                         CreatedAt
                        CreatedAt                           UpdatedAt
                        UpdatedAt                           IsDeleted
                        IsDeleted
```

---

## API Examples

### Create Transaction (Expense)

```bash
curl -X POST http://localhost:5003/api/transactions \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "categoryId": 5,
    "paymentMethod": 2,
    "transactionType": 2,
    "amount": {
      "amount": 5000,
      "currency": "BRL"
    },
    "description": "Monthly grocery shopping",
    "transactionDate": "2024-01-15T10:00:00Z",
    "isRecurring": false
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "statusCode": 200,
  "data": {
    "transactionId": 42
  },
  "errors": []
}
```

### Get Transactions by Type and Date Range

```bash
curl -X GET "http://localhost:5003/api/transactions?type=expense&from=2024-01-01&to=2024-01-31" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..."
```

**Response (200 OK):**
```json
{
  "success": true,
  "statusCode": 200,
  "data": [
    {
      "id": 42,
      "userId": 1,
      "category": {
        "id": 5,
        "userId": 1,
        "name": "Groceries",
        "description": "Food and household items",
        "createdAt": "2024-01-01T00:00:00Z",
        "updatedAt": null
      },
      "paymentMethod": "CreditCard",
      "transactionType": "Expense",
      "amount": {
        "amount": 5000,
        "currency": "BRL"
      },
      "description": "Monthly grocery shopping",
      "transactionDate": "2024-01-15T10:00:00Z",
      "isRecurring": false,
      "createdAt": "2024-01-15T10:00:00Z",
      "updatedAt": null
    }
  ],
  "errors": []
}
```

### Create Category

```bash
curl -X POST http://localhost:5003/api/categories \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "name": "Education",
    "description": "Courses, books and educational materials"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "statusCode": 200,
  "data": {
    "id": 12
  },
  "errors": []
}
```

### Create Recurrence Schedule

```bash
curl -X POST http://localhost:5003/api/recurrenceschedule \
  -H "Content-Type: application/json" \
  -d '{
    "transactionId": 42,
    "frequency": 3,
    "startDate": "2024-02-01T00:00:00Z",
    "endDate": "2024-12-01T00:00:00Z",
    "maxOccurrences": 11
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "statusCode": 200,
  "data": {
    "id": 7,
    "transactionId": 42,
    "frequency": "Monthly",
    "startDate": "2024-02-01T00:00:00Z",
    "endDate": "2024-12-01T00:00:00Z",
    "nextOccurrence": "2024-02-01T00:00:00Z",
    "maxOccurrences": 11,
    "occurrencesGenerated": 0,
    "isActive": true
  },
  "errors": []
}
```

---

## Erros

| Código | HTTP Status | Mensagem | Remediation |
|--------|-------------|----------|-------------|
| TRANSACTION_NOT_FOUND | 404 | Transaction not found | Verify transaction ID exists |
| CATEGORY_NOT_FOUND | 404 | Category not found | Verify category ID exists |
| INVALID_CURRENCY | 400 | Invalid or unsupported currency | Use a supported currency code (e.g., BRL, USD, EUR) |
| INVALID_AMOUNT | 400 | Invalid transaction amount | Ensure amount is a positive integer representing cents |

---

## 📚 Documentação Relacionada

| Tipo | Documento | Descrição |
|------|-----------|----------|
| 📋 Code Review | [FINANCIAL_CODE_REVIEW.md](../code-reviews/FINANCIAL_CODE_REVIEW.md) | Revisão detalhada de código com issues por severidade |
| 🧪 Test Plan | [Financial-Test-Plan.md](../test-plans/Financial-Test-Plan.md) | Plano de testes unitários, integração e E2E |
| 📖 Transações Recorrentes | [RecurringTransactions.md](RecurringTransactions.md) | Documentação detalhada do sistema de recorrências |
| 🔧 Building Blocks | [building-blocks.md](building-blocks.md) | Bibliotecas compartilhadas (CQRS, Result Pattern, Messaging) |
| 🏗️ Arquitetura | [API-GATEWAY.md](../architecture/API-GATEWAY.md) | Gateway que roteia chamadas para este serviço |
| 📊 Review Consolidado | [LIFESYNC_CODE_REVIEW_CONSOLIDADO.md](../code-reviews/LIFESYNC_CODE_REVIEW_CONSOLIDADO.md) | Visão consolidada de todos os serviços |

[← Voltar ao Índice de Documentação](../README.md)