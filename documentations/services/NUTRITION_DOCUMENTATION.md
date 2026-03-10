# Nutrition Microservice — Documentação Completa

**Projeto:** LifeSync
**Serviço:** Nutrition
**Versão:** 1.1
**Data:** 2026-02-24
**Arquitetura:** Clean Architecture + CQRS + Domain Events
**Runtime:** .NET (ASP.NET Core)
**Banco de Dados:** PostgreSQL

---

## Índice

1. [Visão Geral](#1-visão-geral)
2. [Estrutura do Projeto](#2-estrutura-do-projeto)
3. [Configuração e Startup](#3-configuração-e-startup)
4. [Autenticação e Autorização](#4-autenticação-e-autorização)
5. [Endpoints da API](#5-endpoints-da-api)
   - 5.1 [Diaries](#51-diaries)
   - 5.2 [Meals](#52-meals)
   - 5.3 [MealFoods](#53-mealfoods)
   - 5.4 [Liquids](#54-liquids)
   - 5.5 [DailyProgresses](#55-dailyprogresses)
   - 5.6 [Health Check](#56-health-check)
6. [DTOs de Requisição e Resposta](#6-dtos-de-requisição-e-resposta)
   - 6.1 [Diary DTOs](#61-diary-dtos)
   - 6.2 [Meal DTOs](#62-meal-dtos)
   - 6.3 [MealFood DTOs](#63-mealfood-dtos)
   - 6.4 [Liquid DTOs](#64-liquid-dtos)
   - 6.5 [DailyProgress DTOs](#65-dailyprogress-dtos)
   - 6.6 [Filtros de Query (Paginação e Ordenação)](#66-filtros-de-query-paginação-e-ordenação)
7. [Modelos de Domínio](#7-modelos-de-domínio)
8. [CQRS — Commands e Queries](#8-cqrs--commands-e-queries)
9. [Domain Events](#9-domain-events)
10. [Infraestrutura](#10-infraestrutura)
11. [Tratamento de Erros](#11-tratamento-de-erros)
12. [Padrões Arquiteturais](#12-padrões-arquiteturais)
13. [Schema do Banco de Dados](#13-schema-do-banco-de-dados)
14. [Catálogo de Alimentos — Data Seeder](#14-catálogo-de-alimentos--data-seeder)
15. [Histórico de Alterações](#15-histórico-de-alterações)

---

## 1. Visão Geral

O **Nutrition Microservice** é responsável por toda a gestão nutricional dos usuários na plataforma LifeSync. Ele gerencia:

- **Diários nutricionais** (`Diaries`) — registro diário de alimentação por usuário
- **Refeições** (`Meals`) — agrupamento de alimentos consumidos em uma refeição
- **Alimentos da refeição** (`MealFoods`) — vínculo entre uma refeição e um alimento do catálogo, com quantidade consumida
- **Catálogo de alimentos** (`Foods`) — tabela de alimentos com dados nutricionais completos, populada via CSV no startup
- **Tipos de líquidos** (`LiquidType`) — catálogo de tipos de bebidas (água, suco, etc.)
- **Líquidos** (`Liquids`) — controle de hidratação diária, associados a um tipo de líquido
- **Progresso diário** (`DailyProgresses`) — acompanhamento de metas calóricas e de hidratação

O serviço expõe uma API REST protegida por JWT e segue os princípios de Clean Architecture com CQRS (Command Query Responsibility Segregation) e Domain Events para manter a consistência entre agregados.

---

## 2. Estrutura do Projeto

```
Services/Nutrition/
├── Nutrition.API/                          # Camada de apresentação (Controllers, Program.cs)
│   ├── Controllers/
│   │   ├── DiariesController.cs
│   │   ├── MealsController.cs
│   │   ├── MealFoodsController.cs
│   │   ├── LiquidsController.cs
│   │   └── DailyProgressesController.cs
│   └── Program.cs
│
├── Nutrition.Application/                  # Camada de aplicação (CQRS, DTOs, Serviços)
│   ├── Commands/
│   │   ├── Diary/
│   │   ├── Meal/
│   │   ├── MealFood/
│   │   ├── Liquid/
│   │   └── DailyProgress/
│   ├── Queries/
│   │   ├── Diary/
│   │   ├── Meal/
│   │   ├── MealFood/
│   │   ├── Liquid/
│   │   └── DailyProgress/
│   ├── DTOs/
│   ├── EventHandlers/
│   │   ├── MealFoodAddedEventHandler.cs
│   │   └── MealFoodRemovedEventHandler.cs
│   ├── Interfaces/
│   └── DependencyInjection.cs
│
├── Nutrition.Domain/                       # Camada de domínio (Entidades, Eventos, Erros)
│   ├── Entities/
│   │   ├── Diary.cs
│   │   ├── Meal.cs
│   │   ├── MealFood.cs
│   │   ├── Food.cs                         # ← NOVO: catálogo de alimentos
│   │   ├── Liquid.cs
│   │   ├── LiquidType.cs                   # ← NOVO: tipos de líquidos
│   │   └── DailyProgress.cs
│   ├── ValueObjects/
│   │   └── DailyGoal.cs
│   ├── Events/
│   │   ├── MealFoodAddedEvent.cs
│   │   ├── MealFoodRemovedEvent.cs
│   │   └── MealAddedToDiaryEvent.cs
│   └── Errors/
│       ├── DiaryErrors.cs
│       ├── MealErrors.cs
│       ├── MealFoodErrors.cs
│       ├── LiquidErrors.cs
│       └── DailyProgressErrors.cs
│
└── Nutrition.Infrastructure/               # Camada de infraestrutura (EF Core, Repositórios)
    ├── Persistence/
    │   ├── Data/
    │   │   └── ApplicationDbContext.cs
    │   └── Repositories/
    │       ├── DiaryRepository.cs
    │       ├── MealRepository.cs
    │       ├── MealFoodRepository.cs
    │       ├── LiquidRepository.cs
    │       └── DailyProgressRepository.cs
    ├── DataSeeders/
    │   ├── Csv/
    │   │   ├── CsvFiles/
    │   │   │   └── Food.csv               # Dados nutricionais do catálogo de alimentos
    │   │   ├── FoodCsvRow.cs              # DTO de leitura do CSV
    │   │   └── FoodMap.cs                 # Mapeamento de colunas CSV (CsvHelper ClassMap)
    │   ├── TablesCsvSeeder.cs             # Semeia a tabela Foods a partir do CSV
    │   └── SeederHostedService.cs         # IHostedService: executa o seeder no startup
    ├── Migrations/
    └── DependencyInjection.cs
```

---

## 3. Configuração e Startup

### Program.cs

O ponto de entrada da aplicação registra os seguintes serviços e middlewares:

```csharp
// Serviços registrados
builder.Services.AddControllers()
builder.Services.AddEndpointsApiExplorer()
builder.Services.AddSwaggerGen()
builder.Services.AddHttpContextAccessor()
builder.Services.AddJwtAuthentication(builder.Configuration)   // JWT via BuildingBlocks
builder.Services.AddApplicationServices()                       // CQRS, Validators, Handlers
builder.Services.AddInfrastructureServices(builder.Configuration) // DB, Repos, Services

// Pipeline de middlewares
app.UseValidationExceptionHandling()  // Trata FluentValidation exceptions
app.UseAuthentication()
app.UseAuthorization()
app.MapControllers()
```

### appsettings.json — Configuração JWT

```json
{
  "JwtSettings": {
    "Key": "SuperSecretKeyForJWTAuthentication2024!@#$%",
    "Issuer": "LifeSyncAPI",
    "Audience": "LifeSyncApp",
    "ExpiryMinutes": 60
  }
}
```

### Configuração do Banco de Dados

- **SGBD:** PostgreSQL
- **Connection String padrão:** `Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=LifeSync`
- **ORM:** Entity Framework Core com Npgsql
- **Migrations:** Executadas automaticamente via `MigrationHostedService` na inicialização

### Data Seeding

Um `SeederHostedService` executa automaticamente na inicialização e popula a tabela de alimentos (`Foods`) a partir de um arquivo CSV usando `CsvHelper`. O seeder só executa se a tabela estiver vazia (operação idempotente). O arquivo CSV está em `DataSeeders/Csv/CsvFiles/Food.csv` com delimitador `;` e contém os campos: `Name`, `Calories`, `Protein`, `Lipids`, `Carbohydrates`, `Calcium`, `Magnesium`, `Iron`, `Sodium`, `Potassium`.

### Injeção de Dependência — Infrastructure

```
Scoped:
  IDiaryRepository          → DiaryRepository
  IMealRepository           → MealRepository
  IMealFoodRepository       → MealFoodRepository
  ILiquidRepository         → LiquidRepository
  IDailyProgressRepository  → DailyProgressRepository

  IDiaryService             → DiaryService
  IMealService              → MealService
  IMealFoodService          → MealFoodService
  ILiquidService            → LiquidService
  IDailyProgressService     → DailyProgressService

Hosted Services:
  MigrationHostedService
  SeederHostedService
```

---

## 4. Autenticação e Autorização

- **Tipo:** JWT Bearer Token
- **Todos os endpoints são protegidos por autenticação JWT**
- O token deve ser enviado no header `Authorization: Bearer <token>`
- O JWT é emitido pelo microserviço de Users e validado via configuração compartilhada (BuildingBlocks)

**Parâmetros de validação do token:**
| Parâmetro | Valor |
|-----------|-------|
| Issuer | `LifeSyncAPI` |
| Audience | `LifeSyncApp` |
| Expiração | 60 minutos |

---

## 5. Endpoints da API

> **Base URL:** `http://localhost:<port>/api`
> **Todos os endpoints requerem** `Authorization: Bearer <jwt_token>`

---

### 5.1 Diaries

**Route base:** `/api/diaries`

| Método | Rota | Parâmetros | Corpo da Requisição | Resposta | Descrição |
|--------|------|------------|---------------------|----------|-----------|
| `GET` | `/diaries/{id}` | `id: int` (path) | — | `DiaryDTO` | Busca um diário pelo ID |
| `GET` | `/diaries/user/{userId}` | `userId: int` (path) | — | `IList<DiaryDTO>` | Busca todos os diários de um usuário |
| `GET` | `/diaries/search` | `DiaryQueryFilterDTO` (query) | — | `(IList<DiaryDTO>, PaginationData)` | Busca diários com filtros e paginação |
| `GET` | `/diaries` | — | — | `IList<DiaryDTO>` | Retorna todos os diários |
| `POST` | `/diaries` | — | `CreateDiaryCommand` | `int` (Id criado) | Cria um novo diário |
| `PUT` | `/diaries/{id}` | `id: int` (path) | `UpdateDiaryCommand` | `bool` | Atualiza um diário existente |
| `DELETE` | `/diaries/{id}` | `id: int` (path) | — | `bool` | Deleta um diário pelo ID |

---

### 5.2 Meals

**Route base:** `/api/meals`

| Método | Rota | Parâmetros | Corpo da Requisição | Resposta | Descrição |
|--------|------|------------|---------------------|----------|-----------|
| `GET` | `/meals/{id}` | `id: int` (path) | — | `MealDTO` | Busca uma refeição pelo ID |
| `GET` | `/meals/diary/{id}` | `id: int` (path) — DiaryId | — | `IList<MealDTO>` | Busca todas as refeições de um diário |
| `GET` | `/meals/search` | `MealQueryFilterDTO` (query) | — | `(IList<MealDTO>, PaginationData)` | Busca refeições com filtros e paginação |
| `GET` | `/meals` | — | — | `IList<MealDTO>` | Retorna todas as refeições |
| `POST` | `/meals/{mealId}/foods` | `mealId: int` (path) | `CreateMealFoodDTO` | `bool` | Adiciona um alimento a uma refeição |
| `DELETE` | `/meals/{mealId}/foods/{foodId}` | `mealId: int`, `foodId: int` (path) | — | `bool` | Remove um alimento de uma refeição |
| `POST` | `/meals` | — | `CreateMealCommand` | `bool` | Cria uma nova refeição |
| `PUT` | `/meals/{id}` | `id: int` (path) | `UpdateMealCommand` | `bool` | Atualiza uma refeição |
| `DELETE` | `/meals/{id}` | `id: int` (path) | — | `bool` | Deleta uma refeição |

---

### 5.3 MealFoods

**Route base:** `/api/meal-foods`

| Método | Rota | Parâmetros | Corpo da Requisição | Resposta | Descrição |
|--------|------|------------|---------------------|----------|-----------|
| `GET` | `/meal-foods/{id}` | `id: int` (path) | — | `MealFoodDTO` | Busca um alimento pelo ID |
| `GET` | `/meal-foods/meal/{id}` | `id: int` (path) — MealId | — | `IList<MealFoodDTO>` | Busca todos os alimentos de uma refeição |
| `GET` | `/meal-foods/search` | `MealFoodQueryFilterDTO` (query) | — | `(IList<MealFoodDTO>, PaginationData)` | Busca alimentos com filtros e paginação |
| `GET` | `/meal-foods` | — | — | `IList<MealFoodDTO>` | Retorna todos os alimentos |
| `POST` | `/meal-foods` | — | `CreateMealFoodCommand` | `int` (Id criado) | Cria um novo alimento |
| `PUT` | `/meal-foods/{id}` | `id: int` (path) | `UpdateMealFoodCommand` | `bool` | Atualiza um alimento |
| `DELETE` | `/meal-foods/{id}` | `id: int` (path) | — | `bool` | Deleta um alimento |

---

### 5.4 Liquids

**Route base:** `/api/liquids`

| Método | Rota | Parâmetros | Corpo da Requisição | Resposta | Descrição |
|--------|------|------------|---------------------|----------|-----------|
| `GET` | `/liquids/{id}` | `id: int` (path) | — | `LiquidDTO` | Busca um líquido pelo ID |
| `GET` | `/liquids` | — | — | `IList<LiquidDTO>` | Retorna todos os líquidos |
| `GET` | `/liquids/diary/{diaryId}` | `diaryId: int` (path) | — | `IList<LiquidDTO>` | Busca todos os líquidos de um diário |
| `GET` | `/liquids/search` | `LiquidQueryFilterDTO` (query) | — | `(IList<LiquidDTO>, PaginationData)` | Busca líquidos com filtros e paginação |
| `POST` | `/liquids` | — | `CreateLiquidCommand` | `int` (Id criado) | Cria um novo líquido |
| `PUT` | `/liquids/{id}` | `id: int` (path) | `UpdateLiquidCommand` | `bool` | Atualiza um líquido |
| `DELETE` | `/liquids/{id}` | `id: int` (path) | — | `bool` | Deleta um líquido |

---

### 5.5 DailyProgresses

**Route base:** `/api/daily-progresses`

| Método | Rota | Parâmetros | Corpo da Requisição | Resposta | Descrição |
|--------|------|------------|---------------------|----------|-----------|
| `GET` | `/daily-progresses/{id}` | `id: int` (path) | — | `DailyProgressDTO` | Busca um progresso diário pelo ID |
| `GET` | `/daily-progresses/user/{userId}` | `userId: int` (path) | — | `IList<DailyProgressDTO>` | Busca todos os progressos de um usuário |
| `GET` | `/daily-progresses/search` | `DailyProgressQueryFilterDTO` (query) | — | `(IList<DailyProgressDTO>, PaginationData)` | Busca progressos com filtros e paginação |
| `GET` | `/daily-progresses` | — | — | `IList<DailyProgressDTO>` | Retorna todos os progressos diários |
| `POST` | `/daily-progresses` | — | `CreateDailyProgressCommand` | `int` (Id criado) | Cria um novo progresso diário |
| `PUT` | `/daily-progresses/{id}` | `id: int` (path) | `UpdateDailyProgressCommand` | `bool` | Atualiza um progresso diário |
| `DELETE` | `/daily-progresses/{id}` | `id: int` (path) | — | `bool` | Deleta um progresso diário |
| `POST` | `/daily-progresses/{id}/set-goal` | `id: int` (path) | `SetGoalCommand` | `bool` | Define/atualiza a meta do dia |

---

### 5.6 Health Check

| Método | Rota | Resposta |
|--------|------|----------|
| `GET` | `/health` | `{ status: "healthy", service: "Nutrition", timestamp, environment }` |

---

## 6. DTOs de Requisição e Resposta

### Base DTO

Todos os DTOs de resposta herdam de `DTOBase`:

```csharp
record DTOBase(int Id, DateTime CreatedAt, DateTime? UpdatedAt)
```

---

### 6.1 Diary DTOs

#### CreateDiaryCommand (Requisição — POST /diaries)

```csharp
record CreateDiaryCommand
{
    int UserId,        // ID do usuário dono do diário
    DateOnly Date      // Data do diário (formato: YYYY-MM-DD)
}
```

**Exemplo JSON:**
```json
{
  "userId": 1,
  "date": "2026-02-22"
}
```

**Resposta:** `int` — ID do diário criado

---

#### UpdateDiaryCommand (Requisição — PUT /diaries/{id})

```csharp
record UpdateDiaryCommand
{
    int Id,            // ID do diário a ser atualizado
    DateOnly Date      // Nova data do diário
}
```

**Exemplo JSON:**
```json
{
  "id": 5,
  "date": "2026-02-23"
}
```

**Resposta:** `bool` — `true` se atualizado com sucesso

---

#### DiaryDTO (Resposta)

```csharp
record DiaryDTO : DTOBase
{
    int Id,
    int UserId,
    DateOnly Date,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int TotalCalories,         // Calculado: soma calorias de todas as refeições + líquidos
    IList<MealDTO> Meals,      // Lista de refeições do dia
    IList<LiquidDTO> Liquids   // Lista de líquidos do dia
}
```

**Exemplo JSON:**
```json
{
  "id": 1,
  "userId": 42,
  "date": "2026-02-22",
  "createdAt": "2026-02-22T08:00:00Z",
  "updatedAt": null,
  "totalCalories": 1850,
  "meals": [...],
  "liquids": [...]
}
```

---

#### DiaryQueryFilterDTO (Query Params — GET /diaries/search)

```csharp
record DiaryQueryFilterDTO : DomainQueryFilterDTO
{
    int? Id,
    int? UserId,
    int? TotalCaloriesEquals,
    int? TotalCaloriesGreaterThan,
    int? TotalCaloriesLessThan,
    int? TotalLiquidsMlEquals,
    int? TotalLiquidsMlGreaterThan,
    int? TotalLiquidsMlLessThan,
    int? MealId,               // Filtra diários que contêm esse MealId
    int? LiquidId,             // Filtra diários que contêm esse LiquidId
    DateOnly? CreatedAt,
    DateOnly? UpdatedAt,
    bool? IsDeleted,
    string? SortBy,            // Campo para ordenação
    bool? SortDesc,            // true = decrescente
    int? Page,                 // Página (começa em 1)
    int? PageSize              // Itens por página
}
```

---

### 6.2 Meal DTOs

#### CreateMealCommand (Requisição — POST /meals)

```csharp
record CreateMealCommand
{
    int DiaryId,           // ID do diário ao qual a refeição pertence
    string Name,           // Nome da refeição (ex: "Café da manhã")
    string Description     // Descrição da refeição
}
```

**Exemplo JSON:**
```json
{
  "diaryId": 1,
  "name": "Almoço",
  "description": "Refeição completa com proteínas e vegetais"
}
```

**Resposta:** `bool` — `true` se criado com sucesso

---

#### UpdateMealCommand (Requisição — PUT /meals/{id})

```csharp
record UpdateMealCommand
{
    int Id,
    string Name,
    string Description
}
```

**Exemplo JSON:**
```json
{
  "id": 3,
  "name": "Almoço Atualizado",
  "description": "Nova descrição"
}
```

**Resposta:** `bool`

---

#### MealDTO (Resposta)

```csharp
record MealDTO : DTOBase
{
    int Id,
    int DiaryId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Name,
    string Description,
    int TotalCalories,                  // Calculado: soma de (Calories * Quantity) de cada MealFood
    IList<MealFoodDTO> MealFoods       // Lista de alimentos da refeição
}
```

**Exemplo JSON:**
```json
{
  "id": 3,
  "diaryId": 1,
  "createdAt": "2026-02-22T12:00:00Z",
  "updatedAt": null,
  "name": "Almoço",
  "description": "Refeição completa",
  "totalCalories": 650,
  "mealFoods": [...]
}
```

---

#### MealQueryFilterDTO (Query Params — GET /meals/search)

```csharp
record MealQueryFilterDTO : DomainQueryFilterDTO
{
    int? Id,
    string? NameContains,              // Filtro por nome (contains)
    string? DescriptionContains,       // Filtro por descrição (contains)
    int? DiaryId,
    int? TotalCaloriesEquals,
    int? TotalCaloriesGreaterThan,
    int? TotalCaloriesLessThan,
    int? MealFoodId,                   // Filtra refeições que contêm esse alimento
    DateOnly? CreatedAt,
    DateOnly? UpdatedAt,
    bool? IsDeleted,
    string? SortBy,
    bool? SortDesc,
    int? Page,
    int? PageSize
}
```

---

### 6.3 MealFood DTOs

> **Mudança arquitetural (v1.1):** `MealFood` não armazena mais dados nutricionais diretamente. Ele referencia um `Food` do catálogo centralizado via `FoodId`. Os dados nutricionais são retornados no DTO ao consultar, mas o cadastro só exige `MealId`, `FoodId` e `Quantity`.

#### CreateMealFoodCommand (Requisição — POST /meal-foods)

```csharp
record CreateMealFoodCommand
{
    int MealId,    // ID da refeição à qual o alimento pertence
    int FoodId,    // ID do alimento no catálogo (tabela Foods)
    int Quantity   // Quantidade consumida (unidades)
}
```

**Exemplo JSON:**
```json
{
  "mealId": 3,
  "foodId": 101,
  "quantity": 2
}
```

**Resposta:** `int` — ID do MealFood criado

---

#### CreateMealFoodDTO (Requisição — POST /meals/{mealId}/foods)

Usado para adicionar um alimento diretamente a uma refeição através do endpoint de Meals:

```csharp
record CreateMealFoodDTO
{
    int MealId,    // ID da refeição
    int FoodId,    // ID do alimento no catálogo
    int Quantity   // Quantidade consumida
}
```

**Exemplo JSON:**
```json
{
  "mealId": 3,
  "foodId": 101,
  "quantity": 2
}
```

**Resposta:** `bool`

---

#### UpdateMealFoodCommand (Requisição — PUT /meal-foods/{id})

```csharp
record UpdateMealFoodCommand
{
    int Id,        // ID do MealFood a atualizar
    int FoodId,    // Novo alimento do catálogo
    int Quantity   // Nova quantidade consumida
}
```

**Exemplo JSON:**
```json
{
  "foodId": 205,
  "quantity": 3
}
```

**Resposta:** `bool`

---

#### MealFoodDTO (Resposta)

O DTO de resposta é enriquecido com todos os dados nutricionais do `Food` relacionado (via join na camada de mapeamento):

```csharp
record MealFoodDTO : DTOBase
{
    int Id,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int MealId,                    // ID da refeição
    int FoodId,                    // ID do alimento no catálogo
    string Name,                   // Nome do alimento (de Food)
    int Calories,                  // Calorias por unidade (de Food, kcal)
    decimal? Protein,              // Proteínas (g) — de Food
    decimal? Lipids,               // Lipídios/Gorduras (g) — de Food
    decimal? Carbohydrates,        // Carboidratos (g) — de Food
    decimal? Calcium,              // Cálcio (mg) — de Food
    decimal? Magnesium,            // Magnésio (mg) — de Food
    decimal? Iron,                 // Ferro (mg) — de Food
    decimal? Sodium,               // Sódio (mg) — de Food
    decimal? Potassium,            // Potássio (mg) — de Food
    int? Quantity,                 // Quantidade consumida
    decimal? TotalCalories         // Calculado: Quantity * Food.Calories
}
```

**Exemplo JSON:**
```json
{
  "id": 10,
  "createdAt": "2026-02-24T12:05:00Z",
  "updatedAt": null,
  "mealId": 3,
  "foodId": 101,
  "name": "Arroz branco cozido",
  "calories": 128,
  "protein": 2.5,
  "lipids": 0.2,
  "carbohydrates": 28.1,
  "calcium": 4.0,
  "magnesium": 9.0,
  "iron": 0.1,
  "sodium": 1.0,
  "potassium": 37.0,
  "quantity": 2,
  "totalCalories": 256.0
}
```

---

#### MealFoodQueryFilterDTO (Query Params — GET /meal-foods/search)

```csharp
record MealFoodQueryFilterDTO : DomainQueryFilterDTO
{
    int? Id,
    string? NameContains,
    int? Quantity,
    int? CaloriesPerUnitEquals,
    int? CaloriesPerUnitGreaterThan,
    int? CaloriesPerUnitLessThan,
    int? MealId,                       // Filtra alimentos de uma refeição específica
    int? TotalCaloriesEquals,
    int? TotalCaloriesGreaterThan,
    int? TotalCaloriesLessThan,
    DateOnly? CreatedAt,
    DateOnly? UpdatedAt,
    bool? IsDeleted,
    string? SortBy,
    bool? SortDesc,
    int? Page,
    int? PageSize
}
```

---

### 6.4 Liquid DTOs

> **Mudança arquitetural (v1.1):** `Liquid` não armazena mais o nome diretamente nem tem campo `CaloriesPerMl`. Ele agora referencia um `LiquidType` via `LiquidTypeId`. O campo `QuantityMl` foi renomeado para `Quantity`.

#### CreateLiquidCommand (Requisição — POST /liquids)

```csharp
record CreateLiquidCommand
{
    int DiaryId,           // ID do diário ao qual o líquido pertence
    int LiquidTypeId,      // ID do tipo de líquido (catálogo LiquidType)
    int Quantity           // Quantidade consumida (ml)
}
```

**Exemplo JSON:**
```json
{
  "diaryId": 1,
  "liquidTypeId": 1,
  "quantity": 500
}
```

**Resposta:** `int` — ID do líquido criado

---

#### UpdateLiquidCommand (Requisição — PUT /liquids/{id})

```csharp
record UpdateLiquidCommand
{
    int Id,
    int LiquidTypeId,      // Novo tipo de líquido
    int Quantity           // Nova quantidade
}
```

**Exemplo JSON:**
```json
{
  "liquidTypeId": 2,
  "quantity": 350
}
```

**Resposta:** `bool`

---

#### LiquidDTO (Resposta)

```csharp
record LiquidDTO : DTOBase
{
    int Id,
    int DiaryId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Name,           // Nome do tipo de líquido (de LiquidType)
    int Quantity           // Quantidade consumida (ml)
}
```

**Exemplo JSON:**
```json
{
  "id": 7,
  "diaryId": 1,
  "createdAt": "2026-02-24T09:00:00Z",
  "updatedAt": null,
  "name": "Água",
  "quantity": 500
}
```

---

#### LiquidQueryFilterDTO (Query Params — GET /liquids/search)

```csharp
record LiquidQueryFilterDTO : DomainQueryFilterDTO
{
    int? Id,
    string? NameContains,
    int? QuantityMlEquals,
    int? QuantityMlGreaterThan,
    int? QuantityMlLessThan,
    int? CaloriesPerMlEquals,
    int? CaloriesPerMlGreaterThan,
    int? CaloriesPerMlLessThan,
    int? DiaryId,
    int? TotalCaloriesEquals,
    int? TotalCaloriesGreaterThan,
    int? TotalCaloriesLessThan,
    DateOnly? CreatedAt,
    DateOnly? UpdatedAt,
    bool? IsDeleted,
    string? SortBy,
    bool? SortDesc,
    int? Page,
    int? PageSize
}
```

---

### 6.5 DailyProgress DTOs

#### CreateDailyProgressCommand (Requisição — POST /daily-progresses)

```csharp
record CreateDailyProgressCommand
{
    int UserId,
    DateOnly Date,
    int? CaloriesConsumed,     // Opcional — pode iniciar em 0
    int? LiquidsConsumedMl     // Opcional — pode iniciar em 0
}
```

**Exemplo JSON:**
```json
{
  "userId": 42,
  "date": "2026-02-22",
  "caloriesConsumed": 0,
  "liquidsConsumedMl": 0
}
```

**Resposta:** `int` — ID do progresso diário criado

---

#### UpdateDailyProgressCommand (Requisição — PUT /daily-progresses/{id})

```csharp
record UpdateDailyProgressCommand
{
    int Id,
    int CaloriesConsumed,      // Total de calorias consumidas no dia
    int LiquidsConsumedMl,     // Total de líquidos consumidos (ml) no dia
    DailyGoalDTO? Goal         // Opcional — meta do dia
}
```

**Exemplo JSON:**
```json
{
  "id": 5,
  "caloriesConsumed": 1850,
  "liquidsConsumedMl": 2000,
  "goal": {
    "calories": 2000,
    "quantityMl": 2500
  }
}
```

**Resposta:** `bool`

---

#### SetGoalCommand (Requisição — POST /daily-progresses/{id}/set-goal)

```csharp
record SetGoalCommand
{
    int DailyProgressId,       // ID do progresso diário
    DailyGoalDTO Goal          // Meta a ser definida
}
```

**Exemplo JSON:**
```json
{
  "dailyProgressId": 5,
  "goal": {
    "calories": 2000,
    "quantityMl": 2500
  }
}
```

**Resposta:** `bool`

---

#### DailyProgressDTO (Resposta)

```csharp
record DailyProgressDTO : DTOBase
{
    int Id,
    int UserId,
    DateOnly Date,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int? CaloriesConsumed,         // Total de calorias consumidas no dia
    int? LiquidsConsumed,          // Total de líquidos consumidos (ml) no dia
    DailyGoalDTO Goal              // Meta do dia
}
```

**Exemplo JSON:**
```json
{
  "id": 5,
  "userId": 42,
  "date": "2026-02-22",
  "createdAt": "2026-02-22T07:00:00Z",
  "updatedAt": "2026-02-22T20:00:00Z",
  "caloriesConsumed": 1850,
  "liquidsConsumed": 2000,
  "goal": {
    "calories": 2000,
    "quantityMl": 2500
  }
}
```

---

#### DailyGoalDTO

```csharp
record DailyGoalDTO
{
    int Calories,      // Meta calórica diária (kcal)
    int QuantityMl     // Meta de hidratação diária (ml)
}
```

---

#### DailyProgressQueryFilterDTO (Query Params — GET /daily-progresses/search)

```csharp
record DailyProgressQueryFilterDTO : DomainQueryFilterDTO
{
    int? Id,
    int? UserId,
    DateOnly? Date,
    int? CaloriesConsumedEquals,
    int? CaloriesConsumedGreaterThan,
    int? CaloriesConsumedLessThan,
    int? LiquidsConsumedMlEquals,
    int? LiquidsConsumedMlGreaterThan,
    int? LiquidsConsumedMlLessThan,
    DateOnly? CreatedAt,
    DateOnly? UpdatedAt,
    bool? IsDeleted,
    string? SortBy,
    bool? SortDesc,
    int? Page,
    int? PageSize
}
```

---

### 6.6 Filtros de Query (Paginação e Ordenação)

Todos os filtros de busca herdam de `DomainQueryFilterDTO`:

```csharp
record DomainQueryFilterDTO
{
    DateOnly? CreatedAt,   // Filtra por data de criação
    DateOnly? UpdatedAt,   // Filtra por data de atualização
    bool? IsDeleted,       // Incluir registros deletados (soft delete)
    string? SortBy,        // Nome do campo para ordenação
    bool? SortDesc,        // true = ordem decrescente; false/null = crescente
    int? Page,             // Número da página (começa em 1)
    int? PageSize          // Itens por página (padrão definido no servidor)
}
```

**Resposta de busca com filtro:**

```json
{
  "items": [...],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 45,
    "totalPages": 5
  }
}
```

---

## 7. Modelos de Domínio

### Diary (Entidade Agregada)

```csharp
public class Diary : BaseEntity<int>
{
    public int UserId { get; private set; }
    public DateOnly Date { get; private set; }

    // Propriedades calculadas
    public int TotalCalories => _meals.Sum(m => m.TotalCalories)
    public int TotalLiquidsMl => _liquids.Sum(l => l.Quantity)

    // Coleções privadas (encapsulamento do domínio)
    private readonly List<Meal> _meals = new();
    public IReadOnlyCollection<Meal> Meals => _meals.AsReadOnly()

    private readonly List<Liquid> _liquids = new();
    public IReadOnlyCollection<Liquid> Liquids => _liquids.AsReadOnly()

    // Métodos de domínio
    void AddMeal(Meal meal)        // publica MealAddedToDiaryEvent
    void RemoveMeal(Meal meal)
    void AddLiquid(Liquid liquid)
    void RemoveLiquid(Liquid liquid)
    void UpdateDate(DateOnly newDate)
}
```

**Observação:** `TotalCalories` do `Diary` é calculado apenas a partir das refeições (`Meals`). Os líquidos não somam calorias ao total do diário (essa lógica foi removida com a refatoração de `Liquid`).

---

### Meal (Entidade)

```csharp
public class Meal : BaseEntity<int>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int DiaryId { get; private set; }

    // TotalCalories = soma de (Food.Calories * MealFood.Quantity) de cada MealFood
    public int TotalCalories => _mealFoods?.Sum(i => i.Food.Calories * i.Quantity) ?? 0

    private readonly List<MealFood> _mealFoods = new();
    public IReadOnlyCollection<MealFood> MealFoods => _mealFoods.AsReadOnly()

    // Métodos de domínio
    void SetDiaryId(int diaryId)
    void Update(string name, string description)
    void AddMealFood(MealFood mealFood)    // publica MealFoodAddedEvent
    void RemoveMealFood(int mealFoodId)    // publica MealFoodRemovedEvent
}
```

---

### Food (Entidade — Catálogo de Alimentos) ← NOVO

Representa um alimento do catálogo nutricional. Populado automaticamente via CSV no startup. Não é modificado pela API diretamente.

```csharp
public class Food : BaseEntity<int>
{
    public string Name { get; set; }
    public int Calories { get; set; }          // kcal por unidade
    public decimal? Protein { get; set; }      // g
    public decimal? Lipids { get; set; }       // g
    public decimal? Carbohydrates { get; set; }// g
    public decimal? Calcium { get; set; }      // mg
    public decimal? Magnesium { get; set; }    // mg
    public decimal? Iron { get; set; }         // mg
    public decimal? Sodium { get; set; }       // mg
    public decimal? Potassium { get; set; }    // mg
}
```

---

### MealFood (Entidade)

`MealFood` é a entidade de junção entre `Meal` e `Food`. Armazena apenas a quantidade consumida; os dados nutricionais vêm do `Food` relacionado via navigation property.

```csharp
public class MealFood : BaseEntity<int>
{
    public int MealId { get; private set; }
    public int FoodId { get; private set; }        // FK para Food
    public Food Food { get; private set; }          // Navigation property
    public int Quantity { get; private set; }       // Quantidade consumida (unidades)

    // TotalCalories = Quantity * Food.Calories (decimal)
    public decimal TotalCalories => Quantity * Food.Calories

    // Métodos de domínio
    void Update(int foodId, int quantity)
}
```

---

### LiquidType (Entidade — Catálogo de Tipos de Líquidos) ← NOVO

```csharp
public class LiquidType : BaseEntity<int>
{
    public string Name { get; private set; }   // Ex: "Água", "Suco de laranja", "Leite"
}
```

---

### Liquid (Entidade)

```csharp
public class Liquid : BaseEntity<int>
{
    public int DiaryId { get; private set; }
    public int LiquidTypeId { get; set; }          // FK para LiquidType
    public LiquidType LiquidType { get; set; }      // Navigation property
    public int Quantity { get; private set; }       // Quantidade em ml

    // Métodos de domínio
    void Update(int liquidTypeId, int quantity)
    void SetDiaryId(int diaryId)
    void SetLiquidTypeId(int liquidTypeId)
    void SetQuantity(int quantity)
}
```

---

### DailyProgress (Entidade)

```csharp
public class DailyProgress : BaseEntity<int>
{
    public int UserId { get; private set; }
    public DateOnly Date { get; private set; }
    public int CaloriesConsumed { get; private set; } = 0
    public int LiquidsConsumedMl { get; private set; } = 0
    public DailyGoal? Goal { get; private set; } = new(0, 0)    // Value Object

    // Métodos de domínio
    void SetGoal(DailyGoal goal)
    void ResetGoal()
    void SetConsumed(int caloriesConsumed, int liquidsConsumedMl)
    void AddCalories(int calories)
    void AddLiquidsQuantity(int liquidsMl)

    // Métodos de progresso
    int GetCaloriesProgressPercentage()    // % de progresso calórico
    int GetLiquidsProgressPercentage()     // % de progresso de hidratação
    bool IsGoalMet()                       // true se ambas as metas foram atingidas
    bool IsCaloriesGoalMet()
    bool IsLiquidsGoalMet()
}
```

---

### DailyGoal (Value Object)

```csharp
public class DailyGoal
{
    public int Calories { get; private set; } = 0      // Meta calórica diária (kcal)
    public int QuantityMl { get; private set; } = 0    // Meta de hidratação diária (ml)
}
```

Mapeado como **Owned Entity** no EF Core dentro de `DailyProgress`:
- `CaloriesGoal` → coluna da meta calórica
- `LiquidsGoalMl` → coluna da meta de hidratação

---

## 8. CQRS — Commands e Queries

O microserviço implementa o padrão CQRS com MediatR. Todos os comandos e queries passam por handlers registrados via reflection.

### Commands

#### Diary Commands
| Command | Parâmetros | Retorno |
|---------|------------|---------|
| `CreateDiaryCommand` | `UserId: int, Date: DateOnly` | `CreateDiaryResult(int Id)` |
| `UpdateDiaryCommand` | `Id: int, Date: DateOnly` | `UpdateDiaryResult(bool IsSuccess)` |
| `DeleteDiaryCommand` | `Id: int` | `DeleteDiaryResult(bool IsSuccess)` |

#### Meal Commands
| Command | Parâmetros | Retorno |
|---------|------------|---------|
| `CreateMealCommand` | `DiaryId: int, Name: string, Description: string` | `CreateMealResult(bool IsSuccess)` |
| `UpdateMealCommand` | `Id: int, Name: string, Description: string` | `UpdateMealResult(bool IsSuccess)` |
| `DeleteMealCommand` | `Id: int` | `DeleteMealResult(bool IsSuccess)` |
| `AddMealFoodCommand` | `MealId: int, MealFood: CreateMealFoodDTO` | `AddMealFoodResult(bool IsSuccess)` |
| `RemoveMealFoodCommand` | `MealId: int, FoodId: int` | `RemoveMealFoodResult(bool IsSuccess)` |

#### MealFood Commands
| Command | Parâmetros | Retorno |
|---------|------------|---------|
| `CreateMealFoodCommand` | `MealId: int, FoodId: int, Quantity: int` | `CreateMealFoodResult(int Id)` |
| `UpdateMealFoodCommand` | `Id: int, FoodId: int, Quantity: int` | `UpdateMealFoodResult(bool IsSuccess)` |
| `DeleteMealFoodCommand` | `Id: int` | `DeleteMealFoodResult(bool IsSuccess)` |

#### Liquid Commands
| Command | Parâmetros | Retorno |
|---------|------------|---------|
| `CreateLiquidCommand` | `DiaryId: int, LiquidTypeId: int, Quantity: int` | `CreateLiquidResult(int Id)` |
| `UpdateLiquidCommand` | `Id: int, LiquidTypeId: int, Quantity: int` | `UpdateLiquidResult(bool IsSuccess)` |
| `DeleteLiquidCommand` | `Id: int` | `DeleteLiquidResult(bool IsSuccess)` |

#### DailyProgress Commands
| Command | Parâmetros | Retorno |
|---------|------------|---------|
| `CreateDailyProgressCommand` | `UserId: int, Date: DateOnly, CaloriesConsumed?: int, LiquidsConsumedMl?: int` | `CreateDailyProgressResult(int Id)` |
| `UpdateDailyProgressCommand` | `Id: int, CaloriesConsumed: int, LiquidsConsumedMl: int, Goal?: DailyGoalDTO` | `UpdateDailyProgressResult(bool IsSuccess)` |
| `DeleteDailyProgressCommand` | `Id: int` | `DeleteDailyProgressResult(bool IsSuccess)` |
| `SetGoalCommand` | `DailyProgressId: int, Goal: DailyGoalDTO` | `SetGoalResult(bool IsSuccess)` |

---

### Queries

#### Diary Queries
| Query | Parâmetros | Retorno |
|-------|------------|---------|
| `GetDiaryQuery` | `Id: int` | `GetDiaryResult(DiaryDTO Diary)` |
| `GetDiariesQuery` | — | `GetDiariesResult(IList<DiaryDTO>)` |
| `GetAllDiariesByUserIdQuery` | `UserId: int` | `GetAllDiariesByUserIdResult(IList<DiaryDTO>)` |
| `GetDiaryByFilterQuery` | `DiaryQueryFilterDTO` | `GetDiaryByFilterResult(IList<DiaryDTO>, PaginationData)` |

#### Meal Queries
| Query | Parâmetros | Retorno |
|-------|------------|---------|
| `GetMealQuery` | `Id: int` | `GetMealResult(MealDTO)` |
| `GetMealsQuery` | — | `GetMealsResult(IList<MealDTO>)` |
| `GetByDiaryQuery` | `DiaryId: int` | `GetByDiaryResult(IList<MealDTO>)` |
| `GetMealByFilterQuery` | `MealQueryFilterDTO` | `GetMealByFilterResult(IList<MealDTO>, PaginationData)` |

#### MealFood Queries
| Query | Parâmetros | Retorno |
|-------|------------|---------|
| `GetMealFoodQuery` | `Id: int` | `GetMealFoodResult(MealFoodDTO)` |
| `GetMealFoodsQuery` | — | `GetMealFoodsResult(IList<MealFoodDTO>)` |
| `GetByMealQuery` | `MealId: int` | `GetByMealResult(IList<MealFoodDTO>)` |
| `GetMealFoodByFilterQuery` | `MealFoodQueryFilterDTO` | `GetMealFoodByFilterResult(IList<MealFoodDTO>, PaginationData)` |

#### Liquid Queries
| Query | Parâmetros | Retorno |
|-------|------------|---------|
| `GetLiquidQuery` | `Id: int` | `GetLiquidResult(LiquidDTO)` |
| `GetAllLiquidsQuery` | — | `GetAllLiquidsResult(IList<LiquidDTO>)` |
| `GetLiquidsByDiaryQuery` | `DiaryId: int` | `GetLiquidsByDiaryResult(IList<LiquidDTO>)` |
| `GetLiquidByFilterQuery` | `LiquidQueryFilterDTO` | `GetLiquidByFilterResult(IList<LiquidDTO>, PaginationData)` |

#### DailyProgress Queries
| Query | Parâmetros | Retorno |
|-------|------------|---------|
| `GetDailyProgressQuery` | `Id: int` | `GetDailyProgressResult(DailyProgressDTO)` |
| `GetDailyProgressesQuery` | — | `GetDailyProgressesResult(IList<DailyProgressDTO>)` |
| `GetAllDailyProgressesByUserIdQuery` | `UserId: int` | `GetAllDailyProgressesByUserIdResult(IList<DailyProgressDTO>)` |
| `GetDailyProgressByFilterQuery` | `DailyProgressQueryFilterDTO` | `GetDailyProgressByFilterResult(IList<DailyProgressDTO>, PaginationData)` |

---

## 9. Domain Events

O microserviço publica e consome eventos de domínio internos para manter a consistência entre os agregados `Diary`, `Meal` e `DailyProgress`.

### MealFoodAddedEvent

**Publicado quando:** Um alimento (`MealFood`) é adicionado a uma refeição.

```csharp
public class MealFoodAddedEvent : DomainEvent
{
    public int DiaryId { get; }          // ID do diário pai
    public decimal TotalCalories { get; } // Total de calorias adicionadas
}
```

**Handler (`MealFoodAddedEventHandler`):**
1. Recebe o evento com `DiaryId` e `TotalCalories`
2. Busca o diário pelo ID
3. Calcula o total calórico atualizado do diário
4. Busca o `DailyProgress` do usuário para a data do diário
5. Atualiza `CaloriesConsumed` no `DailyProgress`
6. Persiste a atualização no banco

---

### MealFoodRemovedEvent

**Publicado quando:** Um alimento (`MealFood`) é removido de uma refeição.

```csharp
public class MealFoodRemovedEvent : DomainEvent
{
    public int DiaryId { get; }
    public decimal TotalCalories { get; }
}
```

**Handler:** Lógica inversa ao `MealFoodAddedEventHandler` — subtrai as calorias do `DailyProgress`.

---

### MealAddedToDiaryEvent

**Publicado quando:** Uma refeição é adicionada a um diário.

```csharp
public class MealAddedToDiaryEvent : DomainEvent
{
    public int UserId { get; }
    public DateOnly Date { get; }
    public int MealId { get; }
}
```

---

## 10. Infraestrutura

### ApplicationDbContext

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Diary> Diaries { get; set; }
    public DbSet<Liquid> Liquids { get; set; }
    public DbSet<Meal> Meals { get; set; }
    public DbSet<MealFood> MealFoods { get; set; }
    public DbSet<Food> Foods { get; set; }           // ← NOVO: catálogo de alimentos
    public DbSet<DailyProgress> DailyProgresses { get; set; }
}
```

**Configurações especiais:**
- `DailyProgress.Goal` (Value Object `DailyGoal`) mapeado como **Owned Entity**
  - `CaloriesGoal` → coluna da meta calórica
  - `LiquidsGoalMl` → coluna da meta de hidratação
- `LiquidType` é mapeado implicitamente como tabela via navigation property em `Liquid`

---

### Repositórios

Todos implementam o padrão `IRepository<TEntity, TId, TFilter>`:

**IDiaryRepository:**
- `GetById(int id)` → `Diary?`
- `GetAll()` → `IEnumerable<Diary?>`
- `GetAllByUserId(int userId)` → `IEnumerable<Diary?>`
- `GetByDate(int userId, DateOnly date)` → `Diary?`
- `Find(Expression<Func<Diary, bool>> predicate)` → `IEnumerable<Diary?>`
- `FindByFilter(DiaryQueryFilter filter)` → `(IEnumerable<Diary>, int TotalCount)`
- `Create(Diary entity)`, `CreateRange(...)`, `Update(...)`, `Delete(...)`

**IMealRepository, IMealFoodRepository, ILiquidRepository:** Interface CRUD padrão + `FindByFilter`

**IDailyProgressRepository:**
- Adiciona: `GetAllByUserId(int userId)`, `GetByUserIdAndDateAsync(int userId, DateOnly date)`

---

### Migrations EF Core

As migrations são aplicadas automaticamente via `MigrationHostedService` na inicialização:

| Migration | Data | Descrição |
|-----------|------|-----------|
| `20250509212314_initialcreate2` | 2025-05-09 | Schema inicial |
| `20250522054150_update` | 2025-05-22 | Atualizações de schema |
| `20250522063142_update1` | 2025-05-22 | Atualizações complementares |
| `20250523010414_update22` | 2025-05-23 | Atualizações adicionais |
| `20250523010658_update222` | 2025-05-23 | Atualizações adicionais |
| `20250523010818_update2222` | 2025-05-23 | Atualizações adicionais |
| `20250523222623_updatedb` | 2025-05-23 | Atualizações de banco |
| `20250524234610_update333` | 2025-05-24 | Ajustes adicionais |
| `20260222035020_addFoodTable` | 2026-02-22 | Adiciona campos nutricionais e `Code` em `MealFoods` |
| `20260222052323_addFoodTable2` | 2026-02-22 | Refinamentos em `MealFoods` |
| `20260222052834_addFoodTable3` | 2026-02-22 | Ajustes finais em `MealFoods` |
| `20260224025721_addFood` | 2026-02-24 | **Refatoração:** cria tabela `Foods` e `LiquidType`; remove campos nutricionais de `MealFoods`; renomeia `Code` → `FoodId`; remove `Name`/`QuantityMl`/`CaloriesPerMl` de `Liquids`; renomeia para `LiquidTypeId`/`Quantity` |
| `20260224034026_addFood1` | 2026-02-24 | Adiciona `CreatedAt`, `UpdatedAt`, `IsDeleted` na tabela `Foods` |

---

## 11. Tratamento de Erros

### Padrão Result<T>

Todos os serviços de aplicação retornam `Result<T>` — nunca lançam exceções diretamente para a camada de API:

```csharp
Result<DiaryDTO>.Success(diaryDto)
Result<bool>.Failure(DiaryErrors.NotFound(id))
```

### Erros de Domínio (por entidade)

Cada entidade possui sua própria classe de erros estáticos:

**DiaryErrors:**
- `InvalidId` — ID inválido
- `InvalidUserId` — UserId inválido
- `InvalidDate` — Data inválida
- `NotFound(id)` — Diário não encontrado
- `CreateError` — Erro ao criar
- `UpdateError` — Erro ao atualizar
- `DeleteError` — Erro ao deletar

Padrão equivalente existe para: `MealErrors`, `MealFoodErrors`, `LiquidErrors`, `DailyProgressErrors`

### Validação de Comandos

- Validação via **FluentValidation** aplicada em todos os commands
- Middleware `app.UseValidationExceptionHandling()` captura exceções de validação e retorna respostas HTTP padronizadas com os erros de validação

### Exceções de Domínio

- `DomainException` lançada quando regras de negócio são violadas diretamente no domínio
- Capturadas e convertidas em falhas de `Result<T>` pelos handlers

---

## 12. Padrões Arquiteturais

### Clean Architecture

```
Nutrition.API
    ↓ (depends on)
Nutrition.Application
    ↓ (depends on)
Nutrition.Domain
    ↑ (implemented by)
Nutrition.Infrastructure
```

- **Domain** não tem dependências externas
- **Application** depende apenas de Domain (interfaces)
- **Infrastructure** implementa as interfaces do Domain e Application
- **API** orquestra tudo via DI

---

### CQRS (Command Query Responsibility Segregation)

- **Commands** → modificam estado → retornam resultado de sucesso/falha
- **Queries** → leem estado → retornam DTOs
- Handlers registrados via reflection com MediatR/similar

---

### Repository Pattern + Specification

- Todo acesso a dados é abstraído por interfaces de repositório
- Filtros de query encapsulados em objetos de especificação (`DiaryQueryFilter`, etc.)
- Repositórios retornam tupla `(Items, TotalCount)` para paginação

---

### Domain Events (Mediator interno)

- Eventos publicados dentro do domínio ao modificar estado
- Handlers na camada Application mantêm consistência entre agregados
- Fluxo: `Meal.AddMealFood()` → publica `MealFoodAddedEvent` → `MealFoodAddedEventHandler` atualiza `DailyProgress`

---

### Value Objects

- `DailyGoal` — imutável, sem identidade própria, pertence a `DailyProgress`
- Persistido como Owned Entity no EF Core (sem tabela separada)

---

### Result Pattern

- Todas as operações de Application retornam `Result<T>`
- Evita uso de exceções para controle de fluxo
- API converte `Result<T>` em respostas HTTP apropriadas

---

## 13. Schema do Banco de Dados

### Tabelas

```sql
-- Catálogo de alimentos (populado via CSV no startup)
Foods
  Id              SERIAL PRIMARY KEY
  Name            TEXT NOT NULL
  Calories        INT NOT NULL           -- kcal por unidade
  Protein         DECIMAL                -- g (nullable)
  Lipids          DECIMAL                -- g (nullable)
  Carbohydrates   DECIMAL                -- g (nullable)
  Calcium         DECIMAL                -- mg (nullable)
  Magnesium       DECIMAL                -- mg (nullable)
  Iron            DECIMAL                -- mg (nullable)
  Sodium          DECIMAL                -- mg (nullable)
  Potassium       DECIMAL                -- mg (nullable)
  CreatedAt       TIMESTAMP NOT NULL
  UpdatedAt       TIMESTAMP
  IsDeleted       BOOLEAN DEFAULT FALSE

-- Catálogo de tipos de líquidos
LiquidType
  Id          SERIAL PRIMARY KEY
  Name        TEXT NOT NULL
  CreatedAt   TIMESTAMP NOT NULL
  UpdatedAt   TIMESTAMP
  IsDeleted   BOOLEAN DEFAULT FALSE

-- Diários nutricionais
Diaries
  Id          SERIAL PRIMARY KEY
  UserId      INT NOT NULL
  Date        DATE NOT NULL
  CreatedAt   TIMESTAMP NOT NULL
  UpdatedAt   TIMESTAMP
  IsDeleted   BOOLEAN DEFAULT FALSE

-- Refeições (pertence a um Diary)
Meals
  Id          SERIAL PRIMARY KEY
  DiaryId     INT NOT NULL REFERENCES Diaries(Id)
  Name        VARCHAR NOT NULL
  Description VARCHAR
  CreatedAt   TIMESTAMP NOT NULL
  UpdatedAt   TIMESTAMP
  IsDeleted   BOOLEAN DEFAULT FALSE

-- Alimentos de refeição: vínculo Meal ↔ Food com quantidade
MealFoods
  Id          SERIAL PRIMARY KEY
  MealId      INT NOT NULL REFERENCES Meals(Id) ON DELETE CASCADE
  FoodId      INT NOT NULL REFERENCES Foods(Id) ON DELETE CASCADE
  Quantity    INT NOT NULL               -- Quantidade consumida (unidades)
  CreatedAt   TIMESTAMP NOT NULL
  UpdatedAt   TIMESTAMP
  IsDeleted   BOOLEAN DEFAULT FALSE

-- Líquidos (pertence a um Diary, referencia LiquidType)
Liquids
  Id              SERIAL PRIMARY KEY
  DiaryId         INT NOT NULL REFERENCES Diaries(Id)
  LiquidTypeId    INT NOT NULL REFERENCES LiquidType(Id) ON DELETE CASCADE
  Quantity        INT NOT NULL           -- Quantidade em ml
  CreatedAt       TIMESTAMP NOT NULL
  UpdatedAt       TIMESTAMP
  IsDeleted       BOOLEAN DEFAULT FALSE

-- Progresso diário
DailyProgresses
  Id                  SERIAL PRIMARY KEY
  UserId              INT NOT NULL
  Date                DATE NOT NULL
  CaloriesConsumed    INT NOT NULL DEFAULT 0
  LiquidsConsumedMl   INT NOT NULL DEFAULT 0
  CaloriesGoal        INT NOT NULL DEFAULT 0     -- DailyGoal.Calories (Owned Entity)
  LiquidsGoalMl       INT NOT NULL DEFAULT 0     -- DailyGoal.QuantityMl (Owned Entity)
  CreatedAt           TIMESTAMP NOT NULL
  UpdatedAt           TIMESTAMP
  IsDeleted           BOOLEAN DEFAULT FALSE
```

### Relacionamentos

```
Foods    1 ──────── N MealFood
Meals    1 ──────── N MealFood
Diary    1 ──────── N Meal
Diary    1 ──────── N Liquid
LiquidType 1 ─────  N Liquid
DailyProgress 1 ─── 1 DailyGoal (Owned Entity, sem tabela própria)
```

### Campos calculados (não persistidos)

| Entidade | Campo | Cálculo |
|----------|-------|---------|
| `MealFood` | `TotalCalories` | `Quantity * Food.Calories` (decimal) |
| `Meal` | `TotalCalories` | `SUM(MealFood.Food.Calories * MealFood.Quantity)` |
| `Diary` | `TotalCalories` | `SUM(Meal.TotalCalories)` |
| `Diary` | `TotalLiquidsMl` | `SUM(Liquid.Quantity)` |
| `DailyProgress` | `GetCaloriesProgressPercentage()` | `Min((CaloriesConsumed / Goal.Calories) * 100, 100)` |
| `DailyProgress` | `GetLiquidsProgressPercentage()` | `Min((LiquidsConsumedMl / Goal.QuantityMl) * 100, 100)` |
| `DailyProgress` | `IsGoalMet()` | `IsCaloriesGoalMet() && IsLiquidsGoalMet()` |

---

## 14. Catálogo de Alimentos — Data Seeder

### Visão Geral

O serviço utiliza um catálogo centralizado de alimentos (`Foods`) que é populado automaticamente no startup a partir de um arquivo CSV. Esse catálogo é a fonte de verdade para os dados nutricionais usados nos `MealFoods`.

### Fluxo de execução

```
Startup
  └── SeederHostedService.StartAsync()
        └── TablesCsvSeeder.SeedAsync()
              ├── Verifica se Foods já tem registros (idempotente)
              ├── Lê: DataSeeders/Csv/CsvFiles/Food.csv
              ├── Processa com CsvHelper (delimitador: ";")
              ├── Registra o ClassMap: FoodMap → FoodCsvRow
              └── Insere todos os registros em Foods via SaveChangesAsync()
```

### Estrutura do CSV (`Food.csv`)

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `Name` | string | Nome do alimento |
| `Calories` | int | Calorias por unidade (kcal) |
| `Protein` | decimal? | Proteínas (g) |
| `Lipids` | decimal? | Lipídios/Gorduras (g) |
| `Carbohydrates` | decimal? | Carboidratos (g) |
| `Calcium` | decimal? | Cálcio (mg) |
| `Magnesium` | decimal? | Magnésio (mg) |
| `Iron` | decimal? | Ferro (mg) |
| `Sodium` | decimal? | Sódio (mg) |
| `Potassium` | decimal? | Potássio (mg) |

### Classes envolvidas

| Classe | Localização | Responsabilidade |
|--------|-------------|-----------------|
| `FoodCsvRow` | `DataSeeders/Csv/FoodCsvRow.cs` | DTO que representa uma linha do CSV |
| `FoodMap` | `DataSeeders/Csv/FoodMap.cs` | `ClassMap<FoodCsvRow>` — mapeia colunas CSV para propriedades |
| `TablesCsvSeeder` | `DataSeeders/TablesCsvSeeder.cs` | Orquestra a leitura do CSV e persistência no banco |
| `SeederHostedService` | `DataSeeders/SeederHostedService.cs` | `IHostedService` que executa o seeder no startup |

### Registro no DI

```csharp
// Infrastructure/DependencyInjection.cs
services.AddScoped<TablesCsvSeeder>();
services.AddHostedService<SeederHostedService>();
```

---

## 15. Histórico de Alterações

### v1.1 — 2026-02-24

#### Refatoração: Separação do catálogo de alimentos (`Food`)

**Motivação:** Antes, cada `MealFood` armazenava todos os dados nutricionais diretamente (nome, calorias, proteínas, etc.), gerando duplicação massiva de dados. A refatoração centraliza esses dados em uma entidade `Food` separada.

**Mudanças no domínio:**
- **Nova entidade `Food`** — catálogo de alimentos com todos os atributos nutricionais
- **`MealFood` simplificado** — agora armazena apenas `MealId`, `FoodId` e `Quantity`; obtém dados nutricionais via navigation property `Food`
- **`TotalCalories` em `MealFood`** — mudou de `int` para `decimal` (resultado de `Quantity * Food.Calories`)

**Mudanças no banco de dados:**
- Tabela `MealFoods`: removidos campos `Name`, `Code`, `Calories`, `Protein`, `Lipids`, `Carbohydrates`, `Calcium`, `Magnesium`, `Iron`, `Sodium`, `Potassium`; adicionado `FoodId` (FK → `Foods`)
- Nova tabela `Foods` com todos os campos nutricionais + `CreatedAt`, `UpdatedAt`, `IsDeleted`
- Migrations: `20260224025721_addFood`, `20260224034026_addFood1`

**Mudanças nos DTOs:**
- `CreateMealFoodCommand`: era (dados nutricionais + Quantity) → agora `(MealId, FoodId, Quantity)`
- `CreateMealFoodDTO`: mesma mudança
- `UpdateMealFoodCommand`: era (Id + dados nutricionais) → agora `(Id, FoodId, Quantity)`
- `MealFoodDTO`: adicionado `MealId`; dados nutricionais agora vêm do `Food` relacionado

#### Refatoração: Tipos de líquidos (`LiquidType`)

**Motivação:** `Liquid` armazenava o nome do líquido diretamente como string. A refatoração cria um catálogo de tipos de líquidos reutilizável.

**Mudanças no domínio:**
- **Nova entidade `LiquidType`** — catálogo de tipos de líquidos (`Id`, `Name`)
- **`Liquid` refatorado** — campo `Name` removido; `CaloriesPerMl` renomeado para `LiquidTypeId` (FK); `QuantityMl` renomeado para `Quantity`

**Mudanças no banco de dados:**
- Tabela `Liquids`: removido `Name`; `CaloriesPerMl` → `LiquidTypeId` (FK → `LiquidType`); `QuantityMl` → `Quantity`
- Nova tabela `LiquidType`

**Mudanças nos DTOs:**
- `CreateLiquidCommand`: era `(DiaryId, Name, QuantityMl, CaloriesPerMl)` → agora `(DiaryId, LiquidTypeId, Quantity)`
- `UpdateLiquidCommand`: era `(Id, Name, QuantityMl, CaloriesPerMl)` → agora `(Id, LiquidTypeId, Quantity)`
- `LiquidDTO`: removido `QuantityMl` e `CaloriesPerMl`; adicionado `Quantity`; `Name` agora vem do `LiquidType`

#### Data Seeder refatorado

- **Antes:** seeder tentava popular `MealFoods` diretamente
- **Depois:** seeder popula a tabela `Foods` com dados do catálogo nutricional
- Novo arquivo CSV: `DataSeeders/Csv/CsvFiles/Food.csv`
- Novas classes: `FoodCsvRow`, `FoodMap` (em `DataSeeders/Csv/`)
- Classes removidas: `MealFoodCsvDTO`, `MealFoodMap`
