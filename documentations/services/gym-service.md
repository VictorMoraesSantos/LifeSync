# 🏋️ Gym Service

Microserviço responsável pelo **gerenciamento de treinos, exercícios e sessões de academia** no LifeSync.

> **Stack:** ASP.NET Core · EF Core · PostgreSQL  
> **Porta:** `5004` · **Schema:** `gym`  
> **Padrões:** CQRS · DDD (6 Value Objects) · Result Pattern · Specification · Clean Architecture

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

O Gym Service é o microserviço mais rico em termos de modelagem de domínio do LifeSync. Utiliza uma coleção de value objects para encapsular conceitos como `SetCount`, `RepetitionCount`, `Weight` e `RestTime`, garantindo que regras de negócio sejam aplicadas desde o domínio.

### Responsabilidades

- Catálogo de exercícios com tipo, grupo muscular e equipamento
- Criação de rotinas de treino com exercícios configurados
- Registro de sessões de treino com exercícios completados
- Filtros avançados com paginação em todos os agregados

---

## Estrutura de Pastas

```
Gym/
├── Gym.API/
│   ├── Controllers/
│   │   ├── ExercisesController.cs
│   │   ├── RoutinesController.cs
│   │   ├── RoutineExercisesController.cs
│   │   ├── TrainingSessionsController.cs
│   │   └── CompletedExercisesController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── Gym.API.csproj
├── Gym.Application/
│   ├── Contracts/
│   │   ├── IExerciseService.cs
│   │   ├── IRoutineService.cs
│   │   ├── IRoutineExerciseService.cs
│   │   ├── ITrainingSessionService.cs
│   │   └── ICompletedExerciseService.cs
│   ├── DTOs/
│   │   ├── Exercise/
│   │   ├── Routine/
│   │   ├── RoutineExercise/
│   │   ├── TrainingSession/
│   │   └── CompletedExercise/
│   ├── Features/
│   │   ├── Exercise/
│   │   ├── Routine/
│   │   ├── RoutineExercise/
│   │   ├── TrainingSession/
│   │   └── CompletedExercise/
│   ├── Mapping/
│   └── Gym.Application.csproj
├── Gym.Domain/
│   ├── Entities/
│   │   ├── Exercise.cs
│   │   ├── Routine.cs
│   │   ├── RoutineExercise.cs
│   │   ├── TrainingSession.cs
│   │   └── CompletedExercise.cs
│   ├── ValueObjects/
│   │   ├── Weight.cs
│   │   ├── RepetitionCount.cs
│   │   ├── SetCount.cs
│   │   ├── RestTime.cs
│   │   ├── Duration.cs
│   │   └── ExerciseIntensity.cs
│   ├── Enums/
│   │   ├── MuscleGroup.cs
│   │   ├── ExerciseType.cs
│   │   ├── EquipmentType.cs
│   │   ├── MeasurementUnit.cs
│   │   ├── ActivityLevel.cs
│   │   ├── ActivityStatus.cs
│   │   └── WorkoutStatus.cs
│   ├── Repositories/
│   ├── Filters/
│   ├── Errors/
│   └── Gym.Domain.csproj
└── Gym.Infrastructure/
    ├── Persistence/
    │   ├── Data/
    │   │   ├── ApplicationDbContext.cs
    │   │   └── MigrationHostedService.cs
    │   ├── Repositories/
    │   └── Configuration/
    ├── Services/
    ├── DependencyInjection.cs
    ├── Migrations/
    └── Gym.Infrastructure.csproj
```

---

## Domínio

### Entidade: `Exercise`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Regras |
|---|---|---|
| `Name` | `string` | Obrigatório, não vazio |
| `Description` | `string` | Obrigatório, não vazio |
| `MuscleGroup` | `MuscleGroup` | Enum obrigatório |
| `Type` | `ExerciseType` | Enum obrigatório |
| `EquipmentType` | `EquipmentType?` | Opcional |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `Update(name, description, muscleGroup, type, equipmentType?)` | Atualiza todos os campos |
| `SetEquipmentType(equipmentType?)` | Atualiza equipamento |

---

### Entidade: `Routine`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Regras |
|---|---|---|
| `Name` | `string` | Obrigatório, não vazio |
| `Description` | `string` | Obrigatório, não vazio |
| `RoutineExercises` | `IReadOnlyCollection<RoutineExercise>` | Exercícios da rotina |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `Update(name, description)` | Atualiza nome e descrição |
| `AddExercise(routineExercise)` | Adiciona exercício à rotina |
| `RemoveExercise(routineExercise)` | Remove exercício da rotina |

---

### Entidade: `RoutineExercise`

Herda de `BaseEntity<int>`. Representa um exercício configurado dentro de uma rotina.

| Propriedade | Tipo | Regras |
|---|---|---|
| `RoutineId` | `int` | FK para Routine, > 0 |
| `Routine` | `Routine` | Navigation property |
| `ExerciseId` | `int` | FK para Exercise, > 0 |
| `Exercise` | `Exercise` | Navigation property |
| `Sets` | `SetCount` | Value object, mínimo 1 |
| `Repetitions` | `RepetitionCount` | Value object, mínimo 1 |
| `RestBetweenSets` | `RestTime` | Value object, >= 0 segundos |
| `RecommendedWeight` | `Weight?` | Value object opcional |
| `Instructions` | `string?` | Instruções especiais |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `Update(sets, repetitions, restBetweenSets, recommendedWeight?, instructions?)` | Atualiza configurações |

---

### Entidade: `TrainingSession`

Herda de `BaseEntity<int>`.

| Propriedade | Tipo | Regras |
|---|---|---|
| `UserId` | `int` | Obrigatório, > 0 |
| `RoutineId` | `int` | FK para Routine, > 0 |
| `Routine` | `Routine` | Navigation property |
| `StartTime` | `DateTime` | Obrigatório |
| `EndTime` | `DateTime?` | Opcional |
| `Notes` | `string?` | Observações da sessão |
| `CompletedExercises` | `IReadOnlyCollection<CompletedExercise?>` | Exercícios realizados |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `Update(routineId, startTime, endTime?, notes)` | Atualiza dados da sessão |
| `AddCompletedExercise(completedExercise)` | Registra exercício completado |
| `Complete(notes?)` | Finaliza a sessão |
| `GetDuration()` | Retorna `TimeSpan` da sessão |

---

### Entidade: `CompletedExercise`

Herda de `BaseEntity<int>`. Representa um exercício realizado em uma sessão.

| Propriedade | Tipo | Regras |
|---|---|---|
| `TrainingSessionId` | `int` | FK para TrainingSession, > 0 |
| `RoutineExerciseId` | `int` | FK para RoutineExercise, > 0 |
| `SetsCompleted` | `SetCount` | Value object, mínimo 1 |
| `RepetitionsCompleted` | `RepetitionCount` | Value object, mínimo 1 |
| `WeightUsed` | `Weight?` | Value object opcional |
| `CompletedAt` | `DateTime` | Timestamp de conclusão |
| `Notes` | `string?` | Observações |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `Update(setsCompleted, repetitionsCompleted, weightUsed?, notes?)` | Atualiza dados |
| `MarkCompleted()` | Define `CompletedAt` como UTC atual |

---

### Value Objects

| Value Object | Propriedade | Regras | Factory |
|---|---|---|---|
| `Weight` | `Value: int`, `Unit: MeasurementUnit` | Valor >= 0 | `Weight.Create(value, unit)` |
| `SetCount` | `Value: int` | Mínimo 1 | `SetCount.Create(count)` |
| `RepetitionCount` | `Value: int` | Mínimo 1 | `RepetitionCount.Create(count)` |
| `RestTime` | `Value: int` (segundos) | >= 0 | `RestTime.Create(seconds)` |
| `Duration` | `Value: TimeSpan` | Deve ser positivo | `Duration.Create(timespan)` |
| `ExerciseIntensity` | `Value: int` | Entre 1 e 10 | `ExerciseIntensity.Create(intensity)` |

---

### Enumerações

#### `MuscleGroup`

`Chest`, `Back`, `Shoulders`, `Biceps`, `Triceps`, `Forearms`, `Abs`, `Quadriceps`, `Hamstrings`, `Calves`, `Glutes`, `LowerBack`, `Traps`, `Neck`, `FullBody`, `Core`

#### `ExerciseType`

`Strength`, `Hypertrophy`, `Endurance`, `Power`, `Flexibility`, `Cardio`, `HIIT`, `Recovery`

#### `EquipmentType`

`Barbell`, `Dumbbell`, `Machine`, `Cable`, `Bodyweight`, `ResistanceBand`, `Kettlebell`, `MedicineBall`, `FoamRoller`, `BattleRope`, `PullUpBar`, `Bench`, `Other`

#### `MeasurementUnit`

| Categoria | Valores |
|---|---|
| Peso | `Kilogram`, `Pound` |
| Distância | `Meter`, `Kilometer`, `Mile` |
| Tempo | `Second`, `Minute`, `Hour` |

#### `ActivityLevel`

`Beginner (1)`, `Intermediate (2)`, `Advanced (3)`, `Expert (4)`

#### `ActivityStatus`

`Planned`, `Completed`, `Skipped`

#### `WorkoutStatus`

`Planned`, `InProgress`, `Completed`, `Cancelled`

---

### Erros de Domínio

| Classe | Erros principais |
|---|---|
| `ExerciseErrors` | `InvalidName`, `InvalidDescription`, `InvalidMuscleGroup`, `InvalidExerciseType`, `InvalidEquipmentType`, `NotFound(id)`, `InUseError` |
| `RoutineErrors` | `InvalidName`, `InvalidDescription`, `NameTooLong`, `DuplicateName`, `ExerciseAlreadyInRoutine`, `ExerciseNotFoundInRoutine`, `NotFound(id)` |
| `TrainingSessionErrors` | `InvalidUserId`, `InvalidRoutineId`, `InvalidStartTime`, `EndTimeNotAfterStart`, `NullCompletedExercise`, `NotFound(id)` |
| `RoutineExerciseErrors` | `InvalidRoutineId`, `InvalidExerciseId`, `InvalidSets`, `InvalidRepetitions`, `InvalidRestBetweenSets`, `NotFound(id)` |
| `CompletedExerciseErrors` | `InvalidTrainingSessionId`, `InvalidRoutineExerciseId`, `InvalidSetsCompleted`, `InvalidRepetitionsCompleted`, `CompletionDateInFuture`, `NotFound(id)` |

---

## Aplicação

### Commands — Exercise

| Command | Retorno | Validação | Descrição |
|---|---|---|---|
| `CreateExerciseCommand(Name, Description, MuscleGroup, ExerciseType, EquipmentType?)` | `CreateExerciseResult(int Id)` | Name max 100, Description max 500 | Cria exercício |
| `UpdateExerciseCommand(Id, ...)` | `UpdateExerciseResult(bool)` | Mesmo que Create | Atualiza |
| `DeleteExerciseCommand(Id)` | — | — | Remove |

### Commands — Routine

| Command | Retorno | Validação | Descrição |
|---|---|---|---|
| `CreateRoutineCommand(Name, Description)` | `CreateRoutineResult(int Id)` | Name 3-100, Description 5-500 | Cria rotina |
| `UpdateRoutineCommand(Id, Name, Description)` | `UpdateRoutineResult(bool)` | Mesmo que Create | Atualiza |
| `DeleteRoutineCommand(Id)` | — | — | Remove |

### Commands — RoutineExercise

| Command | Retorno | Descrição |
|---|---|---|
| `CreateRoutineExerciseCommand(RoutineId, ExerciseId, Sets, Repetitions, RestBetweenSets, RecommendedWeight?, Instructions?)` | `CreateRoutineExerciseResult(int Id)` | Vincula exercício à rotina |
| `UpdateRoutineExerciseCommand(Id, Sets, Repetitions, RestBetweenSets, RecommendedWeight?, Instructions?)` | `UpdateRoutineExerciseResult(bool)` | Atualiza configuração |
| `DeleteRoutineExerciseCommand(Id)` | — | Remove vínculo |

### Commands — TrainingSession

| Command | Retorno | Validação | Descrição |
|---|---|---|---|
| `CreateTrainingSessionCommand(UserId, RoutineId, StartTime, EndTime)` | `CreateTrainingSessionResult(int Id)` | UserId > 0, RoutineId > 0 | Registra sessão |
| `UpdateTrainingSessionCommand(Id, RoutineId, StartTime, EndTime, Notes)` | `UpdateTrainingSessionResult(bool)` | — | Atualiza |
| `DeleteTrainingSessionsCommand(Id)` | — | — | Remove |

### Commands — CompletedExercise

| Command | Retorno | Descrição |
|---|---|---|
| `CreateCompletedExerciseCommand(TrainingSessionId, ExerciseId, RoutineExerciseId, SetsCompleted, RepetitionsCompleted, WeightUsed?, Notes?)` | `CreateCompletedExerciseResult(int Id)` | Registra exercício realizado |
| `UpdateCompletedExerciseCommand(Id, SetsCompleted, RepetitionsCompleted, RestBetweenSets, WeightUsed?, Notes?)` | — | Atualiza |
| `DeleteCompletedExerciseCommand(Id)` | — | Remove |

---

### Queries

| Query | Retorno | Descrição |
|---|---|---|
| `GetAllExercisesQuery` | `IEnumerable<ExerciseDTO>` | Todos os exercícios |
| `GetExerciseByIdQuery(id)` | `ExerciseDTO` | Exercício por ID |
| `GetExerciseByFilterQuery(filter)` | Paginado | Filtro avançado |
| `GetAllRoutinesQuery` | `IEnumerable<RoutineDTO>` | Todas as rotinas |
| `GetRoutineByIdQuery(id)` | `RoutineDTO` | Rotina por ID (com exercícios) |
| `GetRoutineByFilterQuery(filter)` | Paginado | Filtro avançado |
| `GetAllRoutineExercisesQuery` | `IEnumerable<RoutineExerciseDTO>` | Todos os vínculos |
| `GetRoutineExerciseByIdQuery(id)` | `RoutineExerciseDTO` | Por ID |
| `GetRoutineExerciseByFilterQuery(filter)` | Paginado | Filtro avançado |
| `GetAllTrainingSessionsQuery` | `IEnumerable<TrainingSessionDTO>` | Todas as sessões |
| `GetTrainingSessionByIdQuery(id)` | `TrainingSessionDTO` | Sessão por ID |
| `GetTrainingSessionsByUserIdQuery(userId)` | `IEnumerable<TrainingSessionDTO>` | Sessões do usuário |
| `GetTrainingSessionByFilterQuery(filter)` | Paginado | Filtro avançado |
| `GetAllCompletedExercisesQuery` | `IEnumerable<CompletedExerciseDTO>` | Todos os exercícios completados |
| `GetCompletedExerciseByIdQuery(id)` | `CompletedExerciseDTO` | Por ID |
| `GetCompletedExerciseByFilterQuery(filter)` | Paginado | Filtro avançado |

---

### DTOs Principais

```csharp
ExerciseDTO(Id, CreatedAt, UpdatedAt, Name, Description, MuscleGroup, Type, EquipmentType?)
RoutineDTO(Id, CreatedAt, UpdatedAt, Name, Description)
RoutineExerciseDTO(Id, RoutineId, ExerciseId, Sets, Repetitions, RestBetweenSets, RecommendedWeight?, Instructions?)
TrainingSessionDTO(Id, UserId, RoutineId, CreatedAt, UpdatedAt, StartTime, EndTime?, Notes?)
CompletedExerciseDTO(Id, TrainingSessionId, RoutineExerciseId, SetsCompleted, RepetitionsCompleted, WeightUsed?, Notes?)
```

---

### Filtros Disponíveis

#### `ExerciseFilterDTO`
`Id?`, `NameContains?`, `DescriptionContains?`, `MuscleGroupContains?`, `ExerciseTypeContains?`, `EquipmentTypeContains?`, paginação

#### `RoutineFilterDTO`
`Id?`, `NameContains?`, `DescriptionContains?`, `RoutineExerciseId?`, paginação

#### `RoutineExerciseFilterDTO`
`Id?`, `RoutineId?`, `ExerciseId?`, `SetsEquals/LessThan/GreaterThan`, `RepetitionsEquals/LessThan/GreaterThan`, `RestTimeEquals/LessThan/GreaterThan`, `RecommendedWeightEquals/LessThan/GreaterThan`, `InstructionsContains?`, paginação

#### `TrainingSessionFilterDTO`
`Id?`, `UserId?`, `RoutineId?`, `StartTime?`, `EndTime?`, `NotesContains?`, `CompletedExerciseId?`, paginação

#### `CompletedExerciseFilterDTO`
`Id?`, `TrainingSessionId?`, `RoutineExerciseId?`, `SetsCompletedEquals/LessThan/GreaterThan`, `RepetitionsCompletedEquals/LessThan/GreaterThan`, `WeightUsedEquals/LessThan/GreaterThan`, `CompletedAt?`, `NotesContains?`, paginação

---

## Infraestrutura

### `ApplicationDbContext`

| DbSet | Tipo |
|---|---|
| `Exercises` | `DbSet<Exercise>` |
| `Routines` | `DbSet<Routine>` |
| `RoutineExercises` | `DbSet<RoutineExercise>` |
| `TrainingSessions` | `DbSet<TrainingSession>` |
| `CompletedExercises` | `DbSet<CompletedExercise>` |

**Configuração especial:** Value objects (`Sets`, `Repetitions`, `RestBetweenSets`, `RecommendedWeight`) em `RoutineExercise` e `CompletedExercise` são configurados como owned entities via `OwnsOne`.

### Migrations

| Migration | Data |
|---|---|
| `20250716045350_initialMigration` | 2025-07-16 |
| `20251103215926_mudancas` | 2025-11-03 |

### Registros de DI (Infrastructure)

```csharp
// Repositórios (Scoped)
services.AddScoped<IExerciseRepository, ExerciseRepository>();
services.AddScoped<IRoutineRepository, RoutineRepository>();
services.AddScoped<IRoutineExerciseRepository, RoutineExerciseRepository>();
services.AddScoped<ITrainingSessionRepository, TrainingSessionRepository>();
services.AddScoped<ICompletedExerciseRepository, CompletedExerciseRepository>();

// Serviços (Scoped)
services.AddScoped<IExerciseService, ExerciseService>();
services.AddScoped<IRoutineService, RoutineService>();
services.AddScoped<IRoutineExerciseService, RoutineExerciseService>();
services.AddScoped<ITrainingSessionService, TrainingSessionService>();
services.AddScoped<ICompletedExerciseService, CompletedExerciseService>();
```

### `ITrainingSessionService` — Método customizado

```
GetByUserIdAsync(userId, cancellationToken)  → Result<IEnumerable<TrainingSessionDTO?>>
```

---

## API

### `ExercisesController` — `/api/exercises`

| Método | Rota | Body / Params | Retorno |
|---|---|---|---|
| GET | `/{id}` | `id` | `ExerciseDTO` |
| GET | `/` | — | `IEnumerable<ExerciseDTO>` |
| GET | `/search` | Filtros (query) | Paginado |
| POST | `/` | `CreateExerciseCommand` | `201 { Id }` |
| PUT | `/{id}` | `UpdateExerciseCommand` | `204` |
| DELETE | `/{id}` | `id` | `204` |

---

### `RoutinesController` — `/api/routines`

| Método | Rota | Body / Params | Retorno |
|---|---|---|---|
| GET | `/{id}` | `id` | `RoutineDTO` |
| GET | `/` | — | `IEnumerable<RoutineDTO>` |
| GET | `/search` | Filtros (query) | Paginado |
| POST | `/` | `CreateRoutineCommand` | `201 { Id }` |
| PUT | `/{id}` | `UpdateRoutineCommand` | `204` |
| DELETE | `/{id}` | `id` | `204` |

---

### `RoutineExercisesController` — `/api/routine-exercises`

| Método | Rota | Body / Params | Retorno |
|---|---|---|---|
| GET | `/{id}` | `id` | `RoutineExerciseDTO` |
| GET | `/` | — | `IEnumerable<RoutineExerciseDTO>` |
| GET | `/search` | Filtros (query) | Paginado |
| POST | `/` | `CreateRoutineExerciseCommand` | `201 { Id }` |
| PUT | `/{id}` | `UpdateRoutineExerciseCommand` | `204` |
| DELETE | `/{id}` | `id` | `204` |

---

### `TrainingSessionsController` — `/api/training-sessions`

| Método | Rota | Body / Params | Retorno |
|---|---|---|---|
| GET | `/{id}` | `id` | `TrainingSessionDTO` |
| GET | `/` | — | `IEnumerable<TrainingSessionDTO>` |
| GET | `/search` | Filtros (query) | Paginado |
| POST | `/` | `CreateTrainingSessionCommand` | `201 { Id }` |
| PUT | `/{id}` | `UpdateTrainingSessionCommand` | `204` |
| DELETE | `/{id}` | `id` | `204` |

---

### `CompletedExercisesController` — `/api/completed-exercises`

| Método | Rota | Body / Params | Retorno |
|---|---|---|---|
| GET | `/{id}` | `id` | `CompletedExerciseDTO` |
| GET | `/` | — | `IEnumerable<CompletedExerciseDTO>` |
| GET | `/search` | Filtros (query) | Paginado |
| POST | `/` | `CreateCompletedExerciseCommand` | `201 { Id }` |
| PUT | `/{id}` | `UpdateCompletedExerciseCommand` | `204` |
| DELETE | `/{id}` | `id` | `204` |

---

### Health Check

```
GET /health
→ { "status": "healthy", "service": "Gym", "timestamp": "...", "environment": "..." }
```

---

## Testes

### Projetos de Teste

| Projeto | Tipo | Status |
|---|---|---|
| `Gym.UnitTests` | Unitários | Stub vazio (não implementado) |
| `Gym.IntegrationTests` | Integração | Existente |
| `Gym.E2ETests` | E2E | Existente |

---

## Problemas Críticos (Code Review 03/03/2026)

> **Fonte:** Análise Code Review Completo — Todas as camadas (Domain, Application, Infrastructure, API)

### PROBLEMAS CRÍTICOS

| # | Severidade | Camada | Problema | Impacto | Arquivo |
|---|---|---|---|---|---|
| 1 | CRÍTICO | Application | **RoutineMapper - Parâmetros invertidos** | Nome e descrição de TODAS as rotinas estão trocados na API | `RoutineMapper.cs:101-102` |
| 2 | CRÍTICO | Infrastructure | **Paginação em memória** | Performance O(n) em todas as listagens — carrega TODOS os registros para内存 | `ExerciseService.cs`, `RoutineService.cs`, `TrainingSessionService.cs`, `CompletedExerciseService.cs` |
| 3 | CRÍTICO | Infrastructure | **RoutineExerciseSpecification - Semântica incorreta** | Filtro `SetsEquals` usa `<=` ao invés de `==` — usuários buscando "5 sets" recebem 1,2,3,4,5 | `RoutineExerciseSpecification.cs:155` |
| 4 | CRÍTICO | API | **Controllers sem `[Authorize]`** | Todos os endpoints são públicos — sem autenticação | Todos os Controllers |
| 5 | CRÍTICO | API | **JWT Secret hardcoded** | Chave de autenticação exposta no appsettings.json | `appsettings.json:253` |

### PROBLEMAS DE ALTA PRIORIDADE

| # | Severidade | Camada | Problema | Impacto | Arquivo |
|---|---|---|---|---|---|
| 6 | ALTO | Domain | **`CompletedAt` nunca inicializado** | `CompletedAt` fica com `default(DateTime)` (01/01/0001) — dados inválidos no banco | `CompletedExercise.cs` |
| 7 | ALTO | Domain | **Inconsistência de exceções** | Exercise usa `ArgumentException`, Routine usa `DomainException` — padrão不一致 | `Exercise.cs`, `Routine.cs` |
| 8 | ALTO | Infrastructure | **NullReferenceException potencial** | `Notes.Contains()` lança exceção se `Notes` for null no banco | `TrainingSessionSpecification.cs:169-170` |
| 9 | ALTO | Infrastructure | **Configuração duplicada** | `OwnsOne(ce => ce.SetsCompleted)` registrado duas vezes | `CompletedExerciseConfiguration.cs:185-187` |
| 10 | ALTO | Infrastructure | **Ausência de índices** | Consultas por `Exercise.Name`, `TrainingSession.UserId`, etc. sem index — performance degradada | `ApplicationDbContext.cs` |
| 11 | ALTO | Infrastructure | **Sem isolamento de dados por usuário** | Qualquer usuário pode consultar treinos de outros | `TrainingSessionService.cs:208` |
| 12 | ALTO | Infrastructure | **FindAsync carrega tudo em memória** | Mesmo padrão de paginação — O(n) desnecessário | `ExerciseService.cs`, etc. |
| 13 | ALTO | API | **Validação ID route vs body ausente** | `command.Id` diferente do route `id` é sobrescrito silenciosamente | Controllers |

### PROBLEMAS DE MÉDIA PRIORIDADE

| # | Severidade | Camada | Problema | Impacto |
|---|---|---|---|---|
| 14 | MÉDIO | Application | **Camada Service redundante** | Fluxo: Controller → Handler → Service → Repository — uma camada poderia ser eliminada |
| 15 | MÉDIO | Infrastructure | **Eager Loading inconsistente** | ExerciseRepository: nenhum include; RoutineRepository: inclui `RoutineExercises`; TrainingSessionRepository: inclui `Routine` e `CompletedExercises` |
| 16 | MÉDIO | Infrastructure | **Sem Unit of Work** | Operações como "criar rotina + adicionar exercícios" não são atômicas |
| 17 | MÉDIO | Domain | **Coleção nullable desnecessária** | `IReadOnlyCollection<CompletedExercise?>` — `?` no tipo cria confusão |
| 18 | MÉDIO | Domain | **Domain events comentados** | Eventos de domínio declarados mas desativados — infraestrutura ociosa |
| 19 | MÉDIO | Application | **Arquivos com extensão duplicada** | `UpdateRoutineExerciseDTO.cs.cs`, `UpdateTrainingSessionDTO.cs.cs` |

### PROBLEMAS DE BAIXA PRIORIDADE

| # | Severidade | Problema |
|---|---|---|
| 20 | BAIXO | Mensagens de erro em idiomas misturados (inglês e português) |

---

## Recomendações de Correção

### Prioridade 1 — Críticos (Esforço: ~5h)

| # | Ação | Tempo Estimado | Dependência |
|---|---|---|---|
| 1 | Corrigir `RoutineMapper` — inverter ordem de `Name` e `Description` no `ToDTO()` | 10 min | — |
| 2 | Adicionar `[Authorize]` em todos os Controllers | 10 min | — |
| 3 | Remover JWT Key do `appsettings.json` — usar variáveis de ambiente | 30 min | — |
| 4 | Corrigir paginação em memória — mover `Skip/Take` para query do banco (4 services) | 4h | Repositórios |
| 5 | Corrigir `SetsEquals` na specification — mudar `<=` para `==` | 15 min | — |

### Prioridade 2 — Altos (Esforço: ~6h)

| # | Ação | Tempo Estimado | Dependência |
|---|---|---|---|
| 6 | Corrigir NullReference na `TrainingSessionSpecification` — adicionar null check | 15 min | — |
| 7 | Remover duplicata `CompletedExerciseConfiguration` | 10 min | — |
| 8 | Adicionar índices no banco via Fluent API | 30 min | — |
| 9 | Implementar isolamento de dados por usuário — validar `UserId` autenticado | 2h | Auth |
| 10 | Inicializar `CompletedAt = DateTime.UtcNow` no construtor ou chamar `MarkCompleted()` | 15 min | — |
| 11 | Padronizar exceções para `DomainException` em todas as entidades | 1h | — |
| 12 | Corrigir `FindAsync` para filtrar no banco ao invés de em memória | 2h | Repositórios |

### Prioridade 3 — Médios (Esforço: ~10h)

| # | Ação | Tempo Estimado |
|---|---|---|
| 13 | Implementar Unit of Work para operações atômicas | 4h |
| 14 | Definir estratégia consistente de Eager Loading | 2h |
| 15 | Remover `?` nullable da coleção `CompletedExercises` | 15 min |
| 16 | Habilitar Domain Events (descomentar) | 2h |
| 17 | Padronizar idioma das mensagens de erro (escolher PT-BR ou EN) | 1h |
| 18 | Renomear arquivos com extensão duplicada | 5 min |

---

## Score / Qualidade do Serviço

### Nota Geral: **5/10**

| Dimensão | Score | Observações |
|---|---|---|
| **Domínio** | 6/10 | Bom uso de Value Objects, porém `CompletedAt` nunca inicializado e exceções inconsistentes |
| **Application** | 5/10 | Bug crítico no Mapper (parâmetros invertidos), camada service redundante |
| **Infrastructure** | 4/10 | Paginação em memória, specifications com bugs, sem índices, sem Unit of Work |
| **API** | 4/10 | Sem autenticação, JWT exposto, validação ID insuficiente |
| **Testes** | 2/10 | Apenas stub vazio — 0 testes implementados (conforme test-plan) |
| **Segurança** | 3/10 | Endpoints públicos, sem isolamento de dados, secrets em config |

### Resumo de Issues por Severidade

| Severidade | Quantidade |
|---|---|
| CRÍTICO | 5 |
| ALTO | 7 |
| MÉDIO | 6 |
| BAIXO | 2 |
| **TOTAL** | **20** |

### Indicadores de Cobertura de Testes (Meta)

| Tipo | Meta | Status Atual |
|---|---|---|
| Unitários | ~200 testes | 0 (stub vazio) |
| Integração | ~60 testes | 0 |
| E2E | ~35 testes | 0 |
| **Total** | **~295 testes** | **0** |
| **Cobertura** | **85%+** | **N/A** |

### Débitos Técnicos Acumulados

- **Bug em produção:** Mapper com parâmetros invertidos troca Name/Description
- **Performance:** Todas as listagens carregam dados em memória (O(n))
- **Segurança:** API sem autenticação, dados de usuários acessíveis entre si
- **Dados inválidos:** `CompletedAt` com valor default (01/01/0001)

---

## Problemas Conhecidos (Legado)

| Severidade | Problema | Descrição |
|---|---|---|
| CRÍTICO | RoutineMapper com parâmetros invertidos | `Name` e `Description` estão trocados no `ToDTO()` — respostas da API retornam dados invertidos |
| CRÍTICO | Paginação em memória | `GetPagedAsync`, `FindAsync`, `CountAsync` carregam todos os dados em memória |
| CRÍTICO | Controllers sem `[Authorize]` | Todos os endpoints são públicos |
| CRÍTICO | RoutineExerciseSpecification: `SetsEquals` usa `<=` | Deveria usar `==` para comparação de igualdade |
| ALTO | CompletedExerciseConfiguration duplicada | `OwnsOne(ce => ce.SetsCompleted)` registrado duas vezes |
| ALTO | TrainingSessionSpecification NullReference | `Notes.Contains()` pode lançar exceção se `Notes` for null |
| ALTO | Sem isolamento de dados por usuário | Dados de todos os usuários são acessíveis |
| ALTO | Arquivos com extensão duplicada | `UpdateRoutineExerciseDTO.cs.cs` e `UpdateTrainingSessionDTO.cs.cs` |
| ALTO | `CompletedAt` nunca inicializado | `MarkCompleted()` existe mas nunca é chamado |
| MÉDIO | Exceções inconsistentes | Exercise usa `ArgumentException`, Routine usa `DomainException` |
| MÉDIO | Domain events comentados | Eventos de domínio declarados mas desativados |

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

### Schema do Banco (Relacionamentos)

```
Exercises ──→ RoutineExercises ──→ Routines
                     │
                     ↓
              CompletedExercises ──→ TrainingSessions
```

**Relacionamentos:**
- `Routine` 1:N `RoutineExercise`
- `Exercise` 1:N `RoutineExercise`
- `Routine` 1:N `TrainingSession`
- `TrainingSession` 1:N `CompletedExercise`
- `RoutineExercise` 1:N `CompletedExercise`

**Owned Types (value objects mapeados no banco):**
- `RoutineExercise.Sets` → `Sets_Value`
- `RoutineExercise.Repetitions` → `Repetitions_Value`
- `RoutineExercise.RestBetweenSets` → `RestBetweenSets_Value`
- `RoutineExercise.RecommendedWeight` → `RecommendedWeight_Value`, `RecommendedWeight_Unit`
- `CompletedExercise.SetsCompleted` → `SetsCompleted_Value`
- `CompletedExercise.RepetitionsCompleted` → `RepetitionsCompleted_Value`
- `CompletedExercise.WeightUsed` → `WeightUsed_Value`, `WeightUsed_Unit`

---

## API Examples

### Create Exercise

```bash
curl -X POST http://localhost:5004/api/exercises \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"name": "Bench Press", "description": "Chest exercise", "muscleGroups": ["chest", "triceps"], "difficultyLevel": "intermediate"}'
```

### Create Routine

```bash
curl -X POST http://localhost:5004/api/routines \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"name": "Push Day", "description": "Upper body pushing exercises"}'
```

### Start Training Session

```bash
curl -X POST http://localhost:5004/api/training-sessions \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"userId": 1, "routineId": 1, "startTime": "2024-01-15T10:00:00Z"}'
```

---

## Erros

| Código | HTTP Status | Mensagem | Remediation |
|--------|-------------|----------|-------------|
| EXERCISE_NOT_FOUND | 404 | Exercício não encontrado | Verificar se o ID do exercício existe |
| ROUTINE_NOT_FOUND | 404 | Rotina não encontrada | Verificar se o ID da rotina existe |
| TRAINING_SESSION_NOT_FOUND | 404 | Sessão de treino não encontrada | Verificar se o ID da sessão existe |
| INVALID_DIFFICULTY | 400 | Nível de dificuldade inválido | Usar valores válidos: beginner, intermediate, advanced, expert |

---

## 📚 Documentação Relacionada

| Tipo | Documento | Descrição |
|------|-----------|----------|
| 📋 Code Review | [GYM_CODE_REVIEW.md](../code-reviews/GYM_CODE_REVIEW.md) | Revisão detalhada de código com issues por severidade |
| 🧪 Test Plan | [Gym-Test-Plan.md](../test-plans/Gym-Test-Plan.md) | Plano de testes unitários, integração e E2E |
| 🔧 Building Blocks | [building-blocks.md](building-blocks.md) | Bibliotecas compartilhadas (CQRS, Result Pattern, Messaging) |
| 🏗️ Arquitetura | [API-GATEWAY.md](../architecture/API-GATEWAY.md) | Gateway que roteia chamadas para este serviço |
| 📊 Review Consolidado | [LIFESYNC_CODE_REVIEW_CONSOLIDADO.md](../code-reviews/LIFESYNC_CODE_REVIEW_CONSOLIDADO.md) | Visão consolidada de todos os serviços |

[← Voltar ao Índice de Documentação](../README.md)
