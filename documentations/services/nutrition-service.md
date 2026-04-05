# 🥗 Nutrition Service

Microserviço responsável pelo **gerenciamento nutricional e acompanhamento alimentar** no LifeSync, incluindo diários alimentares, refeições, ingestão de líquidos e metas calóricas.

> **Stack:** ASP.NET Core · EF Core · PostgreSQL · CsvHelper  
> **Porta:** `5002` · **Schema:** `nutrition`
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

O Nutrition Service permite que usuários registrem seus diários alimentares diários, acompanhem refeições, alimentos e ingestão de líquidos, além de definirem metas de calorias e líquidos. A base de dados de alimentos é populada via seed com arquivos CSV.

### Responsabilidades

- Gerenciamento de diários nutricionais (`Diary`) por data
- CRUD de refeições (`Meal`) com alimentos vinculados
- CRUD de alimentos nas refeições (`MealFood`)
- Registro de ingestão de líquidos (`Liquid`) por tipo
- Acompanhamento de progresso diário (`DailyProgress`) com metas
- Seed da base de dados de alimentos via arquivo CSV

---

## Estrutura de Pastas

```
Nutrition/
├── Nutrition.API/
│   ├── Controllers/
│   │   ├── DailyProgressesController.cs
│   │   ├── DiariesController.cs
│   │   ├── FoodsController.cs
│   │   ├── LiquidsController.cs
│   │   ├── LiquidTypesController.cs
│   │   ├── MealFoodsController.cs
│   │   └── MealsController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── Nutrition.API.csproj
├── Nutrition.Application/
│   ├── Contracts/
│   │   ├── IDailyProgressService.cs
│   │   ├── IDiaryService.cs
│   │   ├── IFoodService.cs
│   │   ├── ILiquidService.cs
│   │   ├── ILiquidTypeService.cs
│   │   ├── IMealFoodService.cs
│   │   └── IMealService.cs
│   ├── DTOs/
│   │   ├── DailyProgress/
│   │   ├── Diary/
│   │   ├── Food/
│   │   ├── Liquid/
│   │   ├── LiquidType/
│   │   ├── Meal/
│   │   └── MealFood/
│   ├── EventHandlers/
│   │   ├── MealFoodAddedEventHandler.cs
│   │   └── MealFoodRemovedEventHandler.cs
│   ├── Features/
│   │   ├── DailyProgress/
│   │   ├── Diary/
│   │   ├── Food/
│   │   ├── Liquid/
│   │   ├── LiquidType/
│   │   ├── Meal/
│   │   └── MealFood/
│   ├── Mapping/
│   │   ├── DailyGoalMapper.cs
│   │   ├── DailyProgressMapper.cs
│   │   ├── DiaryMapper.cs
│   │   ├── FoodMapper.cs
│   │   ├── LiquidsMapper.cs
│   │   ├── LiquidTypesMapper.cs
│   │   ├── MealFoodMapper.cs
│   │   └── MealMapper.cs
│   └── Nutrition.Application.csproj
├── Nutrition.Domain/
│   ├── Entities/
│   │   ├── DailyProgress.cs
│   │   ├── Diary.cs
│   │   ├── Food.cs
│   │   ├── Liquid.cs
│   │   ├── LiquidType.cs
│   │   ├── Meal.cs
│   │   └── MealFood.cs
│   ├── Errors/
│   │   ├── DailyProgressErrors.cs
│   │   ├── DiaryErrors.cs
│   │   ├── FoodErrors.cs
│   │   ├── LiquidErrors.cs
│   │   ├── LiquidTypeErrors.cs
│   │   ├── MealErrors.cs
│   │   └── MealFoodErrors.cs
│   ├── Events/
│   │   ├── LiquidChangedEvent.cs
│   │   ├── MealAddedToDiaryEvent.cs
│   │   ├── MealFoodAddedEvent.cs
│   │   └── MealFoodRemovedEvent.cs
│   ├── Filters/
│   │   ├── DailyProgressQueryFilter.cs
│   │   ├── DiaryQueryFilter.cs
│   │   ├── FoodQueryFilter.cs
│   │   ├── LiquidQueryFilter.cs
│   │   ├── LiquidTypeQueryFilter.cs
│   │   ├── MealFoodQueryFilter.cs
│   │   ├── MealQueryFilter.cs
│   │   └── Specifications/
│   │       ├── DailyProgressSpecification.cs
│   │       ├── DiarySpecification.cs
│   │       ├── FoodSpecification.cs
│   │       ├── LiquidSpecification.cs
│   │       ├── LiquidTypeSpecification.cs
│   │       ├── MealFoodSpecification.cs
│   │       └── MealSpecification.cs
│   ├── Repositories/
│   │   ├── IDailyProgressRepository.cs
│   │   ├── IDiaryRepository.cs
│   │   ├── IFoodRepository.cs
│   │   ├── ILiquidRepository.cs
│   │   ├── ILiquidTypeRepository.cs
│   │   ├── IMealFoodRepository.cs
│   │   └── IMealRepository.cs
│   ├── ValueObjects/DailyGoal.cs
│   └── Nutrition.Domain.csproj
└── Nutrition.Infrastructure/
    ├── Persistence/
    │   ├── Data/
    │   │   ├── ApplicationDbContext.cs
    │   │   └── MigrationHostedService.cs
    │   └── Repositories/
    │       ├── DailyProgressRepository.cs
    │       ├── DiaryRepository.cs
    │       ├── FoodRepository.cs
    │       ├── LiquidRepository.cs
    │       ├── LiquidTypeRepository.cs
    │       ├── MealFoodRepository.cs
    │       └── MealRepository.cs
    ├── Services/
    │   ├── DailyProgressService.cs
    │   ├── DiaryService.cs
    │   ├── FoodService.cs
    │   ├── LiquidService.cs
    │   ├── LiquidTypeService.cs
    │   ├── MealFoodService.cs
    │   └── MealService.cs
    ├── DataSeeders/
    │   ├── Csv/CsvFiles/Food.csv
    │   ├── TablesCsvSeeder.cs
    │   └── SeederHostedService.cs
    ├── Migrations/
    └── Nutrition.Infrastructure.csproj
```

---

## Domínio

### Entidade: `DailyProgress`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Regras |
|---|---|---|
| `UserId` | `int` | Obrigatório, > 0 |
| `Date` | `DateOnly` | Obrigatório, não pode ser futuro |
| `CaloriesConsumed` | `int` | Default 0, não negativo |
| `LiquidsConsumedMl` | `int` | Default 0, não negativo |
| `Goal` | `DailyGoal?` | Value object com metas diárias |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `SetGoal(goal)` | Define metas diárias |
| `ResetGoal()` | Reseta metas para (0, 0) |
| `SetConsumed(calories, liquids)` | Define ambos os valores |
| `AddCalories(calories)` | Incrementa calorias consumidas |
| `AddLiquidsQuantity(ml)` | Incrementa líquidos consumidos |
| `GetCaloriesProgressPercentage()` | Percentual 0-100 de calorias |
| `GetLiquidsProgressPercentage()` | Percentual 0-100 de líquidos |
| `IsGoalMet()` | Ambas as metas foram atingidas? |
| `IsCaloriesGoalMet()` | Meta de calorias atingida? |
| `IsLiquidsGoalMet()` | Meta de líquidos atingida? |

---

### Entidade: `Diary`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Regras |
|---|---|---|
| `UserId` | `int` | Obrigatório, > 0 |
| `Date` | `DateOnly` | Data do diário |
| `TotalCalories` | `int` | Computed: soma das calorias das refeições |
| `TotalLiquidsMl` | `int` | Computed: soma dos líquidos |
| `Meals` | `IReadOnlyCollection<Meal>` | Refeições do dia |
| `Liquids` | `IReadOnlyCollection<Liquid>` | Líquidos do dia |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `AddMeal(meal)` | Adiciona refeição e dispara `MealAddedToDiaryEvent` |
| `RemoveMeal(meal)` | Remove refeição |
| `AddLiquid(liquid)` | Adiciona líquido |
| `RemoveLiquid(liquid)` | Remove líquido |
| `UpdateDate(date)` | Atualiza a data |

---

### Entidade: `Meal`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Regras |
|---|---|---|
| `Name` | `string` | Obrigatório, não vazio |
| `Description` | `string` | Obrigatório, não vazio |
| `DiaryId` | `int` | FK para Diary, > 0 |
| `TotalCalories` | `int` | Computed: soma de `(Quantity * Food.Calories)` por MealFood |
| `MealFoods` | `IReadOnlyCollection<MealFood>` | Alimentos da refeição |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `Update(name, description)` | Atualiza nome e descrição |
| `SetDiaryId(diaryId)` | Define FK com validação |
| `AddMealFood(mealFood)` | Adiciona alimento (dispara `MealFoodAddedEvent`) |
| `RemoveMealFood(mealFoodId)` | Remove alimento (dispara `MealFoodRemovedEvent`) |

---

### Entidade: `MealFood`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Regras |
|---|---|---|
| `MealId` | `int` | FK para Meal, > 0 |
| `FoodId` | `int` | FK para Food, > 0 |
| `Food` | `Food` | Navigation property |
| `Quantity` | `int` | Quantidade, > 0 |
| `TotalCalories` | `decimal` | Computed: `Quantity * Food.Calories` |

---

### Entidade: `Food`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Descrição |
|---|---|---|
| `Name` | `string` | Nome do alimento |
| `Calories` | `int` | Calorias por unidade |
| `Protein` | `decimal?` | Proteína (g) |
| `Lipids` | `decimal?` | Lipídios/Gorduras (g) |
| `Carbohydrates` | `decimal?` | Carboidratos (g) |
| `Calcium` | `decimal?` | Cálcio (mg) |
| `Magnesium` | `decimal?` | Magnésio (mg) |
| `Iron` | `decimal?` | Ferro (mg) |
| `Sodium` | `decimal?` | Sódio (mg) |
| `Potassium` | `decimal?` | Potássio (mg) |

> A tabela de alimentos é populada via `Food.csv` usando **CsvHelper** durante o seed.

---

### Entidade: `Liquid`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Regras |
|---|---|---|
| `DiaryId` | `int` | FK para Diary, > 0 |
| `LiquidTypeId` | `int` | FK para LiquidType, > 0 |
| `LiquidType` | `LiquidType` | Navigation property |
| `Quantity` | `int` | Quantidade em ml, > 0 |

---

### Entidade: `LiquidType`

| Propriedade | Tipo | Descrição |
|---|---|---|
| `Id` | `int` | PK |
| `Name` | `string` | Nome (ex: "Água", "Suco") |

---

### Value Object: `DailyGoal`

```
DailyGoal
├── Calories: int   (meta diária de calorias, >= 0)
└── QuantityMl: int (meta diária de líquidos em ml, >= 0)
```

---

### Eventos de Domínio

| Evento | Disparado por | Propriedades |
|---|---|---|
| `MealAddedToDiaryEvent` | `Diary.AddMeal()` | `UserId`, `Date`, `MealId` |
| `MealFoodAddedEvent` | `Meal.AddMealFood()` | `DiaryId`, `TotalCalories` |
| `MealFoodRemovedEvent` | `Meal.RemoveMealFood()` | `DiaryId`, `TotalCalories` |
| `LiquidChangedEvent` | `Diary.AddLiquid()` / `Diary.RemoveLiquid()` | `DiaryId` |

---

## Aplicação

### Commands — DailyProgress

| Command | Retorno | Descrição |
|---|---|---|
| `CreateDailyProgressCommand(UserId, Date, CaloriesConsumed?, LiquidsConsumedMl?)` | `CreateDailyProgressResult(int Id)` | Cria progresso diário |
| `UpdateDailyProgressCommand(Id, CaloriesConsumed, LiquidsConsumedMl, Goal?)` | `UpdateDailyProgressResult(bool)` | Atualiza consumo |
| `DeleteDailyProgressCommand(Id)` | `DeleteDailyProgressResult(bool)` | Remove progresso |
| `SetGoalCommand(DailyProgressId, Goal)` | `SetGoalResult(bool)` | Define meta diária |

### Commands — Diary

| Command | Retorno | Descrição |
|---|---|---|
| `CreateDiaryCommand(UserId, Date)` | `CreateDiaryResult(int Id)` | Cria diário |
| `UpdateDiaryCommand(Id, Date)` | `UpdateDiaryResult(bool)` | Atualiza data |
| `DeleteDiaryCommand(Id)` | `DeleteDiaryResult(bool)` | Remove diário |

### Commands — Meal

| Command | Retorno | Descrição |
|---|---|---|
| `CreateMealCommand(DiaryId, Name, Description)` | `CreateMealResult(bool)` | Cria refeição no diário |
| `UpdateMealCommand(Id, Name, Description)` | `UpdateMealResult(bool)` | Atualiza refeição |
| `DeleteMealCommand(Id)` | `DeleteMealResult(bool)` | Remove refeição |
| `AddMealFoodCommand(MealId, CreateMealFoodDTO)` | `AddMealFoodResult(bool)` | Adiciona alimento à refeição |
| `RemoveMealFoodCommand(MealId, FoodId)` | `RemoveMealFoodResult(bool)` | Remove alimento da refeição |

### Commands — Liquid / MealFood

| Command | Retorno | Descrição |
|---|---|---|
| `CreateLiquidCommand(DiaryId, LiquidTypeId, Quantity)` | `CreateLiquidResult(int Id)` | Registra líquido |
| `UpdateLiquidCommand(Id, LiquidTypeId, Quantity)` | — | Atualiza líquido |
| `DeleteLiquidCommand(Id)` | — | Remove líquido |
| `CreateMealFoodCommand(MealId, FoodId, Quantity)` | `CreateMealFoodResult(int Id)` | Cria alimento na refeição |
| `UpdateMealFoodCommand(Id, FoodId, Quantity)` | — | Atualiza alimento |
| `DeleteMealFoodCommand(Id)` | — | Remove alimento da refeição |
| `CreateLiquidTypeCommand(Name)` | `CreateLiquidTypeResult(int Id)` | Cria tipo de líquido |
| `UpdateLiquidTypeCommand(Id, Name)` | — | Atualiza tipo |
| `DeleteLiquidTypeCommand(Id)` | — | Remove tipo |

---

### Queries

| Query | Retorno | Descrição |
|---|---|---|
| `GetDailyProgressQuery(id)` | `DailyProgressDTO` | Progresso por ID |
| `GetDailyProgressesQuery()` | `IEnumerable<DailyProgressDTO>` | Todos os progressos |
| `GetAllDailyProgressesByUserIdQuery(userId)` | `IEnumerable<DailyProgressDTO>` | Progressos do usuário |
| `GetDailyProgressByFilterQuery(filter)` | Paginado | Filtro avançado |
| `GetDiaryQuery(id)` | `DiaryDTO` | Diário por ID (com refeições e líquidos) |
| `GetDiariesQuery()` | `IEnumerable<DiaryDTO>` | Todos os diários |
| `GetAllDiariesByUserIdQuery(userId)` | `IEnumerable<DiaryDTO>` | Diários do usuário |
| `GetDiaryByFilterQuery(filter)` | Paginado | Filtro avançado |
| `GetMealQuery(id)` | `MealDTO` | Refeição por ID (com alimentos) |
| `GetMealsQuery()` | `IEnumerable<MealDTO>` | Todas as refeições |
| `GetByDiaryQuery(diaryId)` | `IEnumerable<MealDTO>` | Refeições do diário |
| `GetMealByFilterQuery(filter)` | Paginado | Filtro avançado |
| `GetLiquidQuery(id)` | `LiquidDTO` | Líquido por ID |
| `GetAllLiquidsQuery()` | `IEnumerable<LiquidDTO>` | Todos os líquidos |
| `GetLiquidsByDiaryQuery(diaryId)` | `IEnumerable<LiquidDTO>` | Líquidos do diário |
| `GetMealFoodQuery(id)` | `MealFoodDTO` | Alimento de refeição por ID |
| `GetMealFoodsQuery()` | `IEnumerable<MealFoodDTO>` | Todos |
| `GetByMealQuery(mealId)` | `IEnumerable<MealFoodDTO>` | Alimentos de uma refeição |
| `GetMealFoodByFilterQuery(filter)` | Paginado | Filtro avançado de alimentos de refeição |
| `GetFoodByIdQuery(id)` | `FoodDTO` | Alimento por ID |
| `GetAllFoodsQuery()` | `IEnumerable<FoodDTO>` | Todos os alimentos |
| `GetFoodByFilterQuery(filter)` | Paginado | Filtro avançado de alimentos |
| `GetLiquidByFilterQuery(filter)` | Paginado | Filtro avançado de líquidos |
| `GetLiquidTypeQuery(id)` | `LiquidTypeDTO` | Tipo de líquido por ID |
| `GetAllLiquidTypesQuery()` | `IEnumerable<LiquidTypeDTO>` | Todos os tipos de líquidos |
| `GetLiquidTypeByFilterQuery(filter)` | Paginado | Filtro avançado de tipos |

---

### DTOs Principais

#### `DailyProgressDTO`
```
DailyProgressDTO(Id, UserId, Date, CreatedAt, UpdatedAt, CaloriesConsumed?, LiquidsConsumed?, Goal: DailyGoalDTO)
```

#### `DailyGoalDTO`
```
DailyGoalDTO(Calories: int, QuantityMl: int)
```

#### `DiaryDTO`
```
DiaryDTO(Id, UserId, Date, CreatedAt, UpdatedAt, TotalCalories, Meals: IList<MealDTO>, Liquids: IList<LiquidDTO>)
```

#### `MealDTO`
```
MealDTO(Id, DiaryId, CreatedAt, UpdatedAt, Name, Description, TotalCalories, MealFoods: IList<MealFoodDTO>)
```

#### `MealFoodDTO`
```
MealFoodDTO(Id, MealId, FoodId, Name, Calories, Protein?, Lipids?, Carbohydrates?, Calcium?, Magnesium?, Iron?, Sodium?, Potassium?, Quantity?, TotalCalories?)
```

#### `LiquidDTO`
```
LiquidDTO(Id, DiaryId, CreatedAt, UpdatedAt, Name, Quantity)
```

---

### Filtros Disponíveis

#### `DailyProgressQueryFilter`
`Id`, `UserId`, `Date`, `CaloriesConsumedEquals/GreaterThan/LessThan`, `LiquidsConsumedMlEquals/GreaterThan/LessThan`, paginação

#### `DiaryQueryFilter`
`Id`, `UserId`, `TotalCaloriesEquals/GreaterThan/LessThan`, `TotalLiquidsMlEquals/GreaterThan/LessThan`, `MealId`, `LiquidId`, paginação

#### `MealQueryFilter`
`Id`, `NameContains`, `DescriptionContains`, `DiaryId`, `TotalCaloriesEqual/GreaterThen/LessThen`, `MealFoodId`, paginação

#### `MealFoodQueryFilter`
`Id`, `NameContains`, `Quantity`, `CaloriesPerUnitEquals/GreaterThan/LessThan`, `MealId`, `TotalCaloriesEquals/GreaterThan/LessThan`, paginação

#### `LiquidQueryFilter`
`Id`, `DiaryId`, `NameContains`, `QuantityEquals/GreaterThan/LessThan`, paginação

#### `FoodQueryFilter`
`Id`, `NameContains`, paginação

#### `LiquidTypeQueryFilter`
`Id`, `NameContains`, paginação

---

## Infraestrutura

### `ApplicationDbContext`

| DbSet | Tipo |
|---|---|
| `Diaries` | `DbSet<Diary>` |
| `Meals` | `DbSet<Meal>` |
| `MealFoods` | `DbSet<MealFood>` |
| `Foods` | `DbSet<Food>` |
| `Liquids` | `DbSet<Liquid>` |
| `LiquidTypes` | `DbSet<LiquidType>` |
| `DailyProgresses` | `DbSet<DailyProgress>` |

**Configuração especial:** `DailyProgress.Goal` é configurado como owned entity:
- `Goal.Calories` → coluna `CaloriesGoal`
- `Goal.QuantityMl` → coluna `LiquidsGoalMl`

### Migrations

| Migration | Data |
|---|---|
| `20250509212314_initialcreate2` | 2025-05-09 |
| `20250522054150_update` | 2025-05-22 |
| `20250522063142_update1` | 2025-05-22 |
| `20250523010414_update22` | 2025-05-23 |
| `20250523010658_update222` | 2025-05-23 |
| `20250523010818_update2222` | 2025-05-23 |
| `20250523222623_updatedb` | 2025-05-23 |
| `20250524234610_update333` | 2025-05-24 |

### Seed de Dados

A tabela `Foods` é populada via arquivo `Food.csv` usando **CsvHelper 33.1.0**. O arquivo fica em `Nutrition.Infrastructure/DataSeeders/Csv/CsvFiles/Food.csv` e é incluído na saída do projeto.

---

## API

### `DailyProgressesController` — `/api/daily-progresses`

| Método | Rota | Body / Params | Descrição |
|---|---|---|---|
| GET | `/{id:int}` | `id` | Progresso por ID |
| GET | `/user/{userId:int}` | `userId` | Progressos do usuário |
| GET | `/search` | Filtros (query) | Busca paginada |
| GET | `/` | — | Todos os progressos |
| POST | `/` | `CreateDailyProgressCommand` | Criar progresso |
| PUT | `/{id:int}` | `UpdateDailyProgressCommand` | Atualizar |
| DELETE | `/{id:int}` | `id` | Remover |
| POST | `/{id:int}/set-goal` | `SetGoalCommand` | Definir meta |

**Exemplo:**
```json
POST /api/daily-progresses/1/set-goal
{
  "calories": 2000,
  "quantityMl": 2500
}
→ { "isSuccess": true }
```

---

### `DiariesController` — `/api/diaries`

| Método | Rota | Body / Params | Descrição |
|---|---|---|---|
| GET | `/{id:int}` | `id` | Diário com refeições e líquidos |
| GET | `/user/{userId:int}` | `userId` | Diários do usuário |
| GET | `/search` | Filtros (query) | Busca paginada |
| GET | `/` | — | Todos os diários |
| POST | `/` | `CreateDiaryCommand` | Criar diário |
| PUT | `/{id:int}` | `UpdateDiaryCommand` | Atualizar data |
| DELETE | `/{id:int}` | `id` | Remover |

**Exemplo:**
```json
POST /api/diaries
{ "userId": 1, "date": "2025-02-27" }
→ { "id": 1 }

GET /api/diaries/1
→ { "id": 1, "totalCalories": 2500, "meals": [...], "liquids": [...] }
```

---

### `MealsController` — `/api/meals`

| Método | Rota | Body / Params | Descrição |
|---|---|---|---|
| GET | `/{id:int}` | `id` | Refeição por ID (com alimentos) |
| GET | `/diary/{id:int}` | `diaryId` | Refeições do diário |
| GET | `/search` | Filtros (query) | Busca paginada |
| GET | `/` | — | Todas as refeições |
| POST | `/` | `CreateMealCommand` | Criar refeição |
| POST | `/{mealId}/foods` | `AddMealFoodCommand` | Adicionar alimento |
| DELETE | `/{mealId}/foods/{foodId}` | — | Remover alimento |
| PUT | `/{id:int}` | `UpdateMealCommand` | Atualizar refeição |
| DELETE | `/{id:int}` | `id` | Remover refeição |

**Exemplo:**
```json
POST /api/meals/1/foods
{ "mealId": 1, "foodId": 5, "quantity": 2 }
→ { "isSuccess": true }
```

---

### `FoodsController` — `/api/foods`

| Método | Rota | Body / Params | Descrição |
|---|---|---|---|
| GET | `/{id:int}` | `id` | Alimento por ID |
| GET | `/search` | Filtros (query) | Busca paginada |
| GET | `/` | — | Todos os alimentos |

---

### `LiquidsController` — `/api/liquids`

| Método | Rota | Body / Params | Descrição |
|---|---|---|---|
| GET | `/{id:int}` | `id` | Líquido por ID |
| GET | `/` | — | Todos os líquidos |
| GET | `/diary/{diaryId:int}` | `diaryId` | Líquidos do diário |
| GET | `/search` | Filtros (query) | Busca paginada |
| POST | `/` | `CreateLiquidCommand` | Registrar líquido |
| PUT | `/{id:int}` | `UpdateLiquidCommand` | Atualizar |
| DELETE | `/{id:int}` | `id` | Remover |

---

### `LiquidTypesController` — `/api/liquid-types`

| Método | Rota | Body / Params | Descrição |
|---|---|---|---|
| GET | `/{id:int}` | `id` | Tipo por ID |
| GET | `/` | — | Todos os tipos |
| GET | `/search` | Filtros (query) | Busca paginada |
| POST | `/` | `CreateLiquidTypeCommand` | Criar tipo |
| PUT | `/{id:int}` | `UpdateLiquidTypeCommand` | Atualizar |
| DELETE | `/{id:int}` | `id` | Remover |

---

### `MealFoodsController` — `/api/meal-foods`

| Método | Rota | Body / Params | Descrição |
|---|---|---|---|
| GET | `/{id:int}` | `id` | Alimento na refeição por ID |
| GET | `/meal/{id:int}` | `mealId` | Alimentos de uma refeição |
| GET | `/search` | Filtros (query) | Busca paginada |
| GET | `/` | — | Todos |
| POST | `/` | `CreateMealFoodCommand` | Criar |
| PUT | `/{id:int}` | `UpdateMealFoodCommand` | Atualizar |
| DELETE | `/{id:int}` | `id` | Remover |

---

### Health Check

```
GET /health
→ { "status": "healthy", "service": "Nutrition", "timestamp": "...", "environment": "..." }
```

---

## Problemas Críticos

> **Fonte:** Code Review - NUTRITION_CODE_REVIEW.md  
> **Data:** 03/03/2026  
> **Nota Geral:** 5.5/10

### Resumo por Severidade

| Severidade | Quantidade |
|------------|-----------|
| CRÍTICO | 6 |
| ALTO | 7 |
| MÉDIO | 5 |
| BAIXO | 1 |
| INFO | 1 |

---

## 📚 Documentação Relacionada

| Tipo | Documento | Descrição |
|------|-----------|----------|
| 📋 Code Review | [NUTRITION_CODE_REVIEW.md](../code-reviews/NUTRITION_CODE_REVIEW.md) | Revisão detalhada de código com issues por severidade |
| 🧪 Test Plan | [Nutrition-Test-Plan.md](../test-plans/Nutrition-Test-Plan.md) | Plano de testes unitários, integração e E2E |
| 📖 Documentação Estendida | [NUTRITION_DOCUMENTATION.md](NUTRITION_DOCUMENTATION.md) | Documentação complementar do serviço |
| 🔧 Building Blocks | [building-blocks.md](building-blocks.md) | Bibliotecas compartilhadas (CQRS, Result Pattern, Messaging) |
| 🏗️ Arquitetura | [API-GATEWAY.md](../architecture/API-GATEWAY.md) | Gateway que roteia chamadas para este serviço |
| 📊 Review Consolidado | [LIFESYNC_CODE_REVIEW_CONSOLIDADO.md](../code-reviews/LIFESYNC_CODE_REVIEW_CONSOLIDADO.md) | Visão consolidada de todos os serviços |

[← Voltar ao Índice de Documentação](../README.md)

### Issues Críticos

| # | Camada | Arquivo | Problema | Impacto |
|---|--------|---------|----------|---------|
| 1 | Domain | `Food.cs` | Entidade totalmente anêmica com public setters | Zero encapsulamento, sem validação |
| 2 | Domain | `Meal.cs` | Bug NullReference em `RemoveMealFood()` | `mealFood.TotalCalories` lança exceção se não encontrado |
| 3 | Infrastructure | `DailyProgressService.cs`, `DiaryService.cs`, `MealService.cs` | Paginação em memória | Carrega todos os registros antes de paginar |
| 4 | Infrastructure | Repository | `FindAsync` carrega tudo em memória | Filtragem sobre DTOs em memória |
| 5 | API | Controllers | Sem `[Authorize]` | Endpoints públicos sem autenticação |
| 6 | API | `appsettings.json` | JWT Secret hardcoded | Segurança comprometida |

### Issues de Alta Prioridade

| # | Camada | Arquivo | Problema |
|---|--------|---------|----------|
| 7 | Application | Commands | Ausência de FluentValidation |
| 8 | Application | `MealFoodAddedEventHandler.cs` | Event handlers com falha silenciosa |
| 9 | Infrastructure | Repository | Specification pagination perdida |
| 10 | Infrastructure | Database | Ausência de indexes (UserId, Date, DiaryId, MealId) |
| 11 | Infrastructure | API | Sem isolamento de dados por usuário |
| 12 | Infrastructure | DbContext | Sem Global Query Filter para soft delete |
| 13 | API | — | Sem Global Exception Handler |

---

## Recomendações de Correção

### Prioridade 1 — Críticos

| # | Item | Esforço Estimado | Arquivo |
|---|------|------------------|---------|
| 1 | Encapsular entidade Food (private setters + validação) | 2h | `Food.cs` |
| 2 | Corrigir NullReference no `Meal.RemoveMealFood()` | 15 min | `Meal.cs` |
| 3 | Adicionar `[Authorize]` nos controllers | 10 min | `*Controller.cs` |
| 4 | Remover JWT Key do `appsettings.json` | 30 min | `appsettings.json` |
| 5 | Corrigir paginação em memória (3 services) | 3h | `*Service.cs` |
| 6 | Corrigir `FindAsync` para filtrar no banco | 2h | `*Repository.cs` |

### Prioridade 2 — Altos

| # | Item | Esforço Estimado |
|---|------|------------------|
| 7 | Adicionar FluentValidation nos commands | 4h |
| 8 | Corrigir event handlers com falha silenciosa | 1h |
| 9 | Adicionar indexes no banco | 30 min |
| 10 | Implementar isolamento de dados por usuário | 2h |
| 11 | Adicionar Global Query Filter | 30 min |
| 12 | Corrigir specification pagination | 1h |
| 13 | Adicionar Global Exception Handler | 2h |

### Prioridade 3 — Médios

| # | Item | Esforço Estimado |
|---|------|------------------|
| 14 | Cachear `TotalCalories` computado | 2h |
| 15 | Implementar Unit of Work | 4h |
| 16 | Consolidar namespaces (Contracts vs Interfaces) | 30 min |
| 17 | Corrigir `LiquidType` falha silenciosa | 15 min |
| 18 | Corrigir typo no nome do arquivo (`DiaryService..cs`) | 5 min |

---

## Score / Qualidade

### Avaliação Geral

| Dimensão | Nota | Observação |
|----------|------|------------|
| **Nota Geral** | 5.5/10 | Requer atenção imediata |
| Domain Model | 6/10 | Bom uso de DDD e Events, mas Food anêmico |
| Application Layer | 5/10 | Falta validação e error handling |
| Infrastructure | 4/10 | Performance crítica (paginação, N+1) |
| API/Security | 3/10 | Sem autenticação, JWT exposto |
| Testabilidade | 0/10 | Nenhum projeto de teste existente |

### Pontos Positivos

- Uso efetivo de Domain Events (`MealAddedToDiaryEvent`, `MealFoodAddedEvent`, `MealFoodRemovedEvent`)
- Value Object `DailyGoal` bem implementado
- Estrutura de erros através de classes `*Errors` centralizadas
- Specification Pattern implementado para filtros
- Separação clara das camadas (Domain, Application, Infrastructure, API)

### Pontos de Atenção

- **Performance:** Paginação em memória e N+1 queries
- **Segurança:** Ausência de autenticação nos endpoints
- **Testes:** Cobertura zero — projeto de testes não existe
- **Validação:** Food entity sem encapsulamento permite dados inválidos
- **Bug:** `Meal.RemoveMealFood()` pode lançar NullReferenceException

### Plano de Testes Recomendado

| Tipo | Qtd. Testes | Meta Cobertura | Status |
|------|-------------|----------------|--------|
| Unit Tests | ~230 | 95% Domain, 90% Application | A criar |
| Integration Tests | ~65 | 80% Infrastructure | A criar |
| E2E Tests | ~40 | 75% API | A criar |
| **TOTAL** | **~335** | **85%** | — |

> **Nota:** O teste mais crítico é o de `Meal.RemoveMealFood()` para garantir que o bug de NullReference seja coberto e corrigido.

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
| `CsvHelper` | 33.1.0 | Seed de alimentos via CSV |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.0 | Provider PostgreSQL |
| `Microsoft.EntityFrameworkCore` | 10.0.1 | ORM |
| `Swashbuckle.AspNetCore` | 10.1.0 | Swagger |

### Referências Internas

| Projeto | Uso |
|---|---|
| `BuildingBlocks` | CQRS, Result, Validation |
| `Core.Domain` | BaseEntity, ValueObject, Specification |
| `Core.API` | ApiController base |

### Schema do Banco (Conceitual)

```
DailyProgresses     Diaries            Meals              MealFoods       Foods
────────────────    ─────────────      ─────────────────  ─────────────   ──────────────
Id (PK)             Id (PK)            Id (PK)            Id (PK)         Id (PK)
UserId              UserId             DiaryId (FK) ──→   MealId (FK) ─→  Name
Date                Date               Name               FoodId (FK) ─→  Calories
CaloriesConsumed    TotalCalories      Description        Quantity        Protein
LiquidsConsumed     TotalLiquidsMl     TotalCalories      TotalCalories   Lipids
CaloriesGoal                                                              Carbohydrates
LiquidsGoalMl       Liquids
                    ─────────────      LiquidTypes
                    Id (PK)            ─────────────
                    DiaryId (FK)       Id (PK)
                    LiquidTypeId (FK)  Name
                    Quantity
```
