# Financial Service

Responsável pelo gerenciamento financeiro pessoal no LifeSync.

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

O Financial Service permite que usuários controlem suas finanças pessoais — registrando receitas, despesas e categorizando transações. Suporta múltiplos métodos de pagamento, transações recorrentes e mais de 140 moedas internacionais através do value object `Money`.

### Responsabilidades

- CRUD de transações financeiras (receitas e despesas)
- CRUD de categorias personalizadas por usuário
- Suporte a múltiplos métodos de pagamento e moedas
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
│   │   └── ReportsController.cs         # Placeholder (vazio)
│   ├── Program.cs
│   ├── appsettings.json
│   └── Financial.API.csproj
├── Financial.Application/
│   ├── Contracts/
│   │   ├── ICategoryService.cs
│   │   └── ITransactionService.cs
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
│   │   └── Report/
│   │       ├── UserBalanceSummaryDTO.cs
│   │       └── AccountBalanceDTO.cs
│   ├── Features/
│   │   ├── Categories/
│   │   │   ├── Commands/        # Create, Update, Delete
│   │   │   └── Queries/         # GetAll, GetById, GetByUser, GetByFilter
│   │   └── Transactions/
│   │       ├── Commands/        # Create, Update, Delete
│   │       └── Queries/         # GetAll, GetById, GetByUser, GetByFilter
│   ├── Mappings/
│   │   ├── CategoryMapper.cs
│   │   └── TransactionMapper.cs
│   └── Financial.Application.csproj
├── Financial.Domain/
│   ├── Entities/
│   │   ├── Category.cs
│   │   └── Transaction.cs
│   ├── Enums/
│   │   ├── TransactionType.cs
│   │   ├── PaymentMethod.cs
│   │   └── Currency.cs
│   ├── Errors/
│   │   ├── CategoryErrors.cs
│   │   └── TransactionErrors.cs
│   ├── Filters/
│   │   ├── CategoryQueryFilter.cs
│   │   ├── TransactionQueryFilter.cs
│   │   └── Specifications/
│   │       ├── CategorySpecification.cs
│   │       └── TransactionSpecification.cs
│   ├── Repositories/
│   │   ├── ICategoryRepository.cs
│   │   └── ITransactionRepository.cs
│   ├── ValueObjects/Money.cs
│   └── Financial.Domain.csproj
└── Financial.Infrastructure/
    ├── Configuration/TransactionConfiguration.cs
    ├── Persistence/
    │   ├── ApplicationDbContext.cs
    │   ├── MigrationHostedService.cs
    │   └── Repositories/
    │       ├── CategoryRepository.cs
    │       └── TransactionRepository.cs
    ├── Services/
    │   ├── CategoryService.cs
    │   └── TransactionService.cs
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

### Filtros e Especificações

#### `CategoryQueryFilter`

`Id`, `UserId`, `NameContains`, `DescriptionContains`, `CreatedAt`, `UpdatedAt`, `IsDeleted`, `SortBy`, `SortDesc`, `Page`, `PageSize`

#### `TransactionQueryFilter`

`Id`, `UserId`, `CategoryId`, `PaymentMethod`, `TransactionType`, `AmountEquals/GreaterThan/LessThan`, `CurrencyEquals`, `DescriptionContains`, `TransactionDate`, `TransactionDateFrom`, `TransactionDateTo`, paginação

---

## Aplicação

### Commands — Category

| Command | Retorno | Validação | Descrição |
|---|---|---|---|
| `CreateCategoryCommand(UserId, Name, Description?)` | `CreateCategoryResult(int Id)` | Nome: 2-50 chars; Descrição: max 200 | Cria categoria |
| `UpdateCategoryCommand(Id, Name, Description?)` | `UpdateCategoryResult(bool)` | Mesmo que Create | Atualiza |
| `DeleteCategoryCommand(Id)` | `DeleteCategoryResult(bool)` | — | Remove |

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

---

## Infraestrutura

### `ApplicationDbContext`

| DbSet | Tipo |
|---|---|
| `Categories` | `DbSet<Category>` |
| `Transactions` | `DbSet<Transaction>` |

### `TransactionConfiguration`

O value object `Money` é configurado como owned entity usando `OwnsOne`:
- `Amount.Amount` → coluna `Amount`
- `Amount.Currency` → coluna `Currency`

### Migrations

| Migration | Data |
|---|---|
| `20250609025157_initialCreate` | 2025-06-09 |
| `20250609025413_initialCreate321123` | 2025-06-09 |

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

### `ReportsController`

Placeholder para implementação futura de relatórios financeiros.

---

### Health Check

```
GET /health
→ { "status": "healthy", "service": "Financial", "timestamp": "...", "environment": "..." }
```

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
Categories              Transactions
──────────────────      ─────────────────────────────
Id (PK)                 Id (PK)
UserId                  UserId
Name                    CategoryId (FK, nullable) ──→ Categories
Description?            PaymentMethod (int enum)
CreatedAt               TransactionType (int enum)
UpdatedAt               Amount (int — centavos)
IsDeleted               Currency (int enum)
                        Description
                        TransactionDate
                        IsRecurring
                        CreatedAt
                        UpdatedAt
                        IsDeleted
```
