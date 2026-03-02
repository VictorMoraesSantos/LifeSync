# Plano de Testes - Gym Microservice

## Índice

1. [Visão Geral](#visão-geral)
2. [Estrutura de Projetos](#estrutura-de-projetos)
3. [Convenções de Nomenclatura](#convenções-de-nomenclatura)
4. [Pacotes e Ferramentas](#pacotes-e-ferramentas)
5. [Guia de Testes Unitários](#guia-de-testes-unitários)
6. [Guia de Testes de Integração](#guia-de-testes-de-integração)
7. [Guia de Testes E2E](#guia-de-testes-e2e)
8. [Estratégia de Execução](#estratégia-de-execução)
9. [Metas de Cobertura](#metas-de-cobertura)
10. [Exemplos de Código](#exemplos-de-código)

---

## Visão Geral

### Contexto

O microserviço **Gym** do projeto LifeSync é responsável por gerenciar exercícios, rotinas de treino, sessões de treinamento e exercícios completados. Atualmente **não possui nenhum projeto de teste**. Este documento descreve a estratégia completa para implementar uma cobertura de testes robusta em todas as camadas.

### Objetivos

- **Criar projeto de testes unitários** com ~200 testes
- **Criar projeto de testes de integração** com ~60 testes
- **Criar projeto de testes E2E** com ~35 testes
- **Atingir 85%+ de cobertura de código** no projeto
- **Documentar padrões e práticas** de testes

### Arquitetura do Gym

```
Services/Gym/
├── Gym.Domain/              # Entidades, Value Objects, Enums, Interfaces
├── Gym.Application/         # Casos de Uso (CQRS), DTOs, Services
├── Gym.Infrastructure/      # Repositórios, DbContext, Configurations
└── Gym.API/                 # Controllers, Endpoints HTTP
```

**Padrões Utilizados:**
- Clean Architecture
- CQRS (Command Query Responsibility Segregation)
- Repository Pattern
- Result Pattern (error handling)
- Domain-Driven Design
- Value Objects (Weight, SetCount, RepetitionCount, RestTime)

### Entidades do Domínio

| Entidade | Propriedades Principais | Métodos de Negócio |
|----------|------------------------|--------------------|
| **Exercise** | Name, Description, MuscleGroup, Type, EquipmentType? | Update(), SetEquipmentType() |
| **Routine** | Name, Description, RoutineExercises | Update(), AddExercise(), RemoveExercise() |
| **RoutineExercise** | RoutineId, ExerciseId, Sets, Repetitions, RestBetweenSets, RecommendedWeight?, Instructions? | Update() |
| **TrainingSession** | UserId, RoutineId, StartTime, EndTime?, Notes?, CompletedExercises | Update(), AddCompletedExercise(), Complete(), GetDuration() |
| **CompletedExercise** | TrainingSessionId, RoutineExerciseId, SetsCompleted, RepetitionsCompleted, WeightUsed?, CompletedAt, Notes? | Update(), MarkCompleted() |

### Value Objects

| Value Object | Validações |
|-------------|-----------|
| **Weight** | Value >= 0, MeasurementUnit válido |
| **SetCount** | Count >= 1 |
| **RepetitionCount** | Count >= 1 |
| **RestTime** | Seconds >= 0 |

### Enums

- **MuscleGroup:** Chest, Back, Shoulders, Biceps, Triceps, Forearms, Abs, Quadriceps, Hamstrings, Calves, Glutes, LowerBack, Traps, Neck, FullBody, Core
- **ExerciseType:** Strength, Hypertrophy, Endurance, Power, Flexibility, Cardio, HIIT, Recovery
- **EquipmentType:** Barbell, Dumbbell, Machine, Cable, Bodyweight, ResistanceBand, Kettlebell, MedicineBall, FoamRoller, BattleRope, PullUpBar, Bench, Other
- **MeasurementUnit:** Kilogram, Pound, Meter, Kilometer, Mile, Second, Minute, Hour
- **WorkoutStatus:** Planned, InProgress, Completed, Cancelled

---

## Estrutura de Projetos

### Projetos de Teste

```
tests/
├── Gym.UnitTests/                        (NOVO)
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── ExerciseTests.cs
│   │   │   ├── RoutineTests.cs
│   │   │   ├── RoutineExerciseTests.cs
│   │   │   ├── TrainingSessionTests.cs
│   │   │   └── CompletedExerciseTests.cs
│   │   └── ValueObjects/
│   │       ├── WeightTests.cs
│   │       ├── SetCountTests.cs
│   │       ├── RepetitionCountTests.cs
│   │       └── RestTimeTests.cs
│   ├── Application/
│   │   ├── Handlers/
│   │   │   ├── Commands/
│   │   │   │   ├── Exercises/
│   │   │   │   │   ├── CreateExerciseCommandHandlerTests.cs
│   │   │   │   │   ├── UpdateExerciseCommandHandlerTests.cs
│   │   │   │   │   └── DeleteExerciseCommandHandlerTests.cs
│   │   │   │   ├── Routines/
│   │   │   │   │   ├── CreateRoutineCommandHandlerTests.cs
│   │   │   │   │   ├── UpdateRoutineCommandHandlerTests.cs
│   │   │   │   │   └── DeleteRoutineCommandHandlerTests.cs
│   │   │   │   ├── RoutineExercises/
│   │   │   │   │   ├── CreateRoutineExerciseCommandHandlerTests.cs
│   │   │   │   │   ├── UpdateRoutineExerciseCommandHandlerTests.cs
│   │   │   │   │   └── DeleteRoutineExerciseCommandHandlerTests.cs
│   │   │   │   ├── TrainingSessions/
│   │   │   │   │   ├── CreateTrainingSessionCommandHandlerTests.cs
│   │   │   │   │   ├── UpdateTrainingSessionCommandHandlerTests.cs
│   │   │   │   │   └── DeleteTrainingSessionCommandHandlerTests.cs
│   │   │   │   └── CompletedExercises/
│   │   │   │       ├── CreateCompletedExerciseCommandHandlerTests.cs
│   │   │   │       ├── UpdateCompletedExerciseCommandHandlerTests.cs
│   │   │   │       └── DeleteCompletedExerciseCommandHandlerTests.cs
│   │   │   └── Queries/
│   │   │       ├── Exercises/
│   │   │       │   ├── GetExerciseByIdQueryHandlerTests.cs
│   │   │       │   └── GetExercisesByFilterQueryHandlerTests.cs
│   │   │       ├── Routines/
│   │   │       │   ├── GetRoutineByIdQueryHandlerTests.cs
│   │   │       │   └── GetRoutinesByFilterQueryHandlerTests.cs
│   │   │       ├── RoutineExercises/
│   │   │       │   ├── GetRoutineExerciseByIdQueryHandlerTests.cs
│   │   │       │   └── GetRoutineExercisesByFilterQueryHandlerTests.cs
│   │   │       ├── TrainingSessions/
│   │   │       │   ├── GetTrainingSessionByIdQueryHandlerTests.cs
│   │   │       │   ├── GetTrainingSessionsByFilterQueryHandlerTests.cs
│   │   │       │   └── GetTrainingSessionsByUserQueryHandlerTests.cs
│   │   │       └── CompletedExercises/
│   │   │           ├── GetCompletedExerciseByIdQueryHandlerTests.cs
│   │   │           └── GetCompletedExercisesByFilterQueryHandlerTests.cs
│   │   ├── Mappers/
│   │   │   ├── ExerciseMapperTests.cs
│   │   │   ├── RoutineMapperTests.cs
│   │   │   ├── RoutineExerciseMapperTests.cs
│   │   │   ├── TrainingSessionMapperTests.cs
│   │   │   └── CompletedExerciseMapperTests.cs
│   │   └── Services/
│   │       ├── ExerciseServiceTests.cs
│   │       ├── RoutineServiceTests.cs
│   │       ├── RoutineExerciseServiceTests.cs
│   │       ├── TrainingSessionServiceTests.cs
│   │       └── CompletedExerciseServiceTests.cs
│   └── Helpers/
│       └── Builders/
│           ├── ExerciseBuilder.cs
│           ├── RoutineBuilder.cs
│           ├── RoutineExerciseBuilder.cs
│           ├── TrainingSessionBuilder.cs
│           └── CompletedExerciseBuilder.cs
│
├── Gym.IntegrationTests/                 (NOVO)
│   ├── Fixtures/
│   │   └── DatabaseFixture.cs
│   ├── Repositories/
│   │   ├── ExerciseRepositoryTests.cs
│   │   ├── RoutineRepositoryTests.cs
│   │   ├── RoutineExerciseRepositoryTests.cs
│   │   ├── TrainingSessionRepositoryTests.cs
│   │   └── CompletedExerciseRepositoryTests.cs
│   ├── Services/
│   │   ├── ExerciseServiceIntegrationTests.cs
│   │   ├── RoutineServiceIntegrationTests.cs
│   │   ├── TrainingSessionServiceIntegrationTests.cs
│   │   └── CompletedExerciseServiceIntegrationTests.cs
│   └── Helpers/
│       └── TestDataFactory.cs
│
└── Gym.E2ETests/                         (NOVO)
    ├── Fixtures/
    │   ├── CustomWebApplicationFactory.cs
    │   └── TestAuthHandler.cs
    ├── Tests/
    │   ├── HealthCheckTests.cs
    │   ├── ExercisesEndpointTests.cs
    │   ├── RoutinesEndpointTests.cs
    │   ├── TrainingSessionsEndpointTests.cs
    │   ├── CompletedExercisesEndpointTests.cs
    │   ├── WorkoutLifecycleTests.cs
    │   └── RoutineManagementTests.cs
    └── Helpers/
        └── HttpResponseExtensions.cs
```

### Distribuição de Testes

| Projeto | Tipo | Quantidade | Status |
|---------|------|------------|--------|
| Gym.UnitTests | Unit | ~200 | Criar novo |
| Gym.IntegrationTests | Integration | ~60 | Criar novo |
| Gym.E2ETests | E2E | ~35 | Criar novo |
| **TOTAL** | - | **~295** | - |

---

## Convenções de Nomenclatura

### Padrão Principal: `MethodName_StateUnderTest_ExpectedBehavior`

**Exemplos:**
```csharp
Create_WithValidParameters_ShouldCreateExercise()
Update_WithNullName_ShouldThrowArgumentException()
AddExercise_WithNullRoutineExercise_ShouldThrowDomainException()
Complete_WithoutEndTime_ShouldSetEndTimeToUtcNow()
GetDuration_WhenSessionCompleted_ReturnsCorrectTimeSpan()
Create_WithNegativeWeight_ShouldThrowArgumentException()
```

### Nomenclatura de Classes

- **Testes unitários:** `{ClassName}Tests`
  - `ExerciseTests`, `CreateRoutineCommandHandlerTests`, `WeightTests`
- **Testes de integração:** `{ClassName}IntegrationTests` ou `{ClassName}Tests`
  - `ExerciseServiceIntegrationTests`, `ExerciseRepositoryTests`
- **Testes E2E:** `{FeatureName}Tests` ou `{Scenario}Tests`
  - `ExercisesEndpointTests`, `WorkoutLifecycleTests`

---

## Pacotes e Ferramentas

### Testes Unitários

```xml
<ItemGroup>
  <!-- Framework de testes -->
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.0.1" />
  <PackageReference Include="xunit" Version="2.9.3" />
  <PackageReference Include="xunit.runner.visualstudio" Version="3.1.5" />

  <!-- Mocking -->
  <PackageReference Include="Moq" Version="4.20.72" />

  <!-- Assertions -->
  <PackageReference Include="FluentAssertions" Version="7.0.0" />

  <!-- Test Data Generation -->
  <PackageReference Include="AutoFixture.Xunit2" Version="5.0.0" />
  <PackageReference Include="AutoFixture.AutoMoq" Version="5.0.0" />
  <PackageReference Include="Bogus" Version="35.6.1" />

  <!-- Code Coverage -->
  <PackageReference Include="coverlet.collector" Version="6.0.4" />
</ItemGroup>
```

### Testes de Integração

```xml
<ItemGroup>
  <!-- Base -->
  <PackageReference Include="xunit" Version="2.9.3" />
  <PackageReference Include="FluentAssertions" Version="7.0.0" />

  <!-- Containers para testes -->
  <PackageReference Include="Testcontainers" Version="3.10.0" />
  <PackageReference Include="Testcontainers.PostgreSql" Version="3.10.0" />

  <!-- Database cleanup -->
  <PackageReference Include="Respawn" Version="6.2.1" />

  <!-- Test Data -->
  <PackageReference Include="Bogus" Version="35.6.1" />
</ItemGroup>
```

### Testes E2E

```xml
<ItemGroup>
  <!-- ASP.NET Core Testing -->
  <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.0" />

  <!-- Containers -->
  <PackageReference Include="Testcontainers.PostgreSql" Version="3.10.0" />

  <!-- Assertions -->
  <PackageReference Include="FluentAssertions" Version="7.0.0" />
  <PackageReference Include="FluentAssertions.Web" Version="1.2.2" />
</ItemGroup>
```

---

## Guia de Testes Unitários

### Princípios

1. **Isolamento:** Cada teste deve ser independente
2. **Rápido:** Testes unitários devem executar em milissegundos
3. **Repetível:** Mesmos resultados em todas as execuções
4. **Auto-verificável:** Pass ou fail sem intervenção manual
5. **Oportuno:** Escritos junto com o código de produção

### Estrutura AAA (Arrange-Act-Assert)

```csharp
[Fact]
public void Create_WithValidParameters_ShouldCreateExercise()
{
    // Arrange
    var name = "Supino Reto";
    var description = "Exercício para peito com barra";
    var muscleGroup = MuscleGroup.Chest;
    var type = ExerciseType.Hypertrophy;

    // Act
    var exercise = new Exercise(name, description, muscleGroup, type);

    // Assert
    exercise.Should().NotBeNull();
    exercise.Name.Should().Be(name);
    exercise.Description.Should().Be(description);
    exercise.MuscleGroup.Should().Be(muscleGroup);
    exercise.Type.Should().Be(type);
    exercise.EquipmentType.Should().BeNull();
}
```

### 5.1 Testes de Domínio - Entidades

#### Exercise (~15 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidParameters_ShouldCreateExercise | Nome, descrição, grupo muscular e tipo válidos | Entidade criada com propriedades corretas |
| Create_WithNullName_ShouldThrowArgumentException | Nome nulo | Lança ArgumentException |
| Create_WithEmptyName_ShouldThrowArgumentException | Nome vazio | Lança ArgumentException |
| Create_WithWhitespaceName_ShouldThrowArgumentException | Nome com espaços em branco | Lança ArgumentException |
| Create_WithNullDescription_ShouldThrowArgumentException | Descrição nula | Lança ArgumentException |
| Create_WithEmptyDescription_ShouldThrowArgumentException | Descrição vazia | Lança ArgumentException |
| Update_WithValidParameters_ShouldUpdateProperties | Dados válidos | Propriedades atualizadas |
| Update_WithNullName_ShouldThrowArgumentException | Nome nulo no update | Lança ArgumentException |
| Update_WithNullDescription_ShouldThrowArgumentException | Descrição nula no update | Lança ArgumentException |
| SetEquipmentType_WithValidType_ShouldSetEquipment | Tipo de equipamento válido | EquipmentType atualizado |
| SetEquipmentType_WithNull_ShouldSetToNull | Null como equipamento | EquipmentType setado para null |
| Create_WithAllMuscleGroups_ShouldAcceptAllValues | Cada valor de MuscleGroup | Entidade criada para cada valor |
| Create_WithAllExerciseTypes_ShouldAcceptAllValues | Cada valor de ExerciseType | Entidade criada para cada valor |

#### Routine (~15 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidParameters_ShouldCreateRoutine | Nome e descrição válidos | Entidade criada, RoutineExercises vazio |
| Create_WithNullName_ShouldThrowArgumentException | Nome nulo | Lança exceção |
| Create_WithEmptyName_ShouldThrowArgumentException | Nome vazio | Lança exceção |
| Update_WithValidParameters_ShouldUpdateProperties | Dados válidos | Propriedades atualizadas |
| Update_WithNullName_ShouldThrowException | Nome nulo no update | Lança exceção |
| AddExercise_WithValidRoutineExercise_ShouldAddToCollection | RoutineExercise válido | Adicionado à coleção |
| AddExercise_WithNullRoutineExercise_ShouldThrowDomainException | RoutineExercise nulo | Lança DomainException(RoutineErrors.NullRoutineExercise) |
| AddExercise_MultipleTimes_ShouldAccumulateExercises | Múltiplos exercícios | Todos adicionados à coleção |
| RemoveExercise_WithExistingExercise_ShouldRemoveFromCollection | Exercício existente | Removido da coleção |
| RemoveExercise_WithNullRoutineExercise_ShouldThrowDomainException | RoutineExercise nulo | Lança DomainException |
| RoutineExercises_ShouldBeReadOnly | Acesso à coleção | Retorna IReadOnlyCollection |

#### TrainingSession (~20 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidParameters_ShouldCreateSession | UserId, RoutineId, StartTime válidos | Sessão criada |
| Update_WithValidParameters_ShouldUpdateProperties | Dados válidos | Propriedades atualizadas |
| Update_WithEndTimeBeforeStartTime_ShouldThrowDomainException | EndTime < StartTime | Lança DomainException("End time must be after start time") |
| AddCompletedExercise_WithValidExercise_ShouldAddToCollection | CompletedExercise válido | Adicionado à coleção |
| AddCompletedExercise_MultipleExercises_ShouldAccumulate | Múltiplos exercícios | Todos adicionados |
| Complete_WithNotes_ShouldSetEndTimeAndNotes | Notas fornecidas | EndTime e Notes definidos |
| Complete_WithoutNotes_ShouldSetEndTimeOnly | Sem notas | Apenas EndTime definido |
| GetDuration_WhenSessionCompleted_ReturnsCorrectTimeSpan | Sessão com StartTime e EndTime | TimeSpan correto |
| GetDuration_WhenSessionNotCompleted_ReturnsNull | Sessão sem EndTime | Retorna null |
| CompletedExercises_ShouldBeReadOnly | Acesso à coleção | Retorna IReadOnlyCollection |

#### CompletedExercise (~10 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidParameters_ShouldCreateEntity | Dados válidos | Entidade criada |
| Update_WithValidParameters_ShouldUpdateProperties | Dados válidos | Propriedades atualizadas |
| MarkCompleted_ShouldSetCompletedAt | Marcar como completo | CompletedAt atualizado |

### 5.2 Testes de Domínio - Value Objects

#### Weight (~10 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidValues_ShouldCreateWeight | Valor >= 0 e unidade válida | Weight criado |
| Create_WithZeroValue_ShouldCreateWeight | Valor = 0 | Weight criado (peso zero válido) |
| Create_WithNegativeValue_ShouldThrowArgumentException | Valor < 0 | Lança ArgumentException |
| Create_WithKilogram_ShouldSetUnit | MeasurementUnit.Kilogram | Unidade correta |
| Create_WithPound_ShouldSetUnit | MeasurementUnit.Pound | Unidade correta |

#### SetCount (~6 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidCount_ShouldCreateSetCount | Count >= 1 | SetCount criado |
| Create_WithZeroCount_ShouldThrowArgumentException | Count = 0 | Lança ArgumentException |
| Create_WithNegativeCount_ShouldThrowArgumentException | Count < 0 | Lança ArgumentException |
| Create_WithOne_ShouldBeValid | Count = 1 (limite mínimo) | SetCount criado |

#### RepetitionCount (~6 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidCount_ShouldCreateRepetitionCount | Count >= 1 | RepetitionCount criado |
| Create_WithZeroCount_ShouldThrowArgumentException | Count = 0 | Lança ArgumentException |
| Create_WithNegativeCount_ShouldThrowArgumentException | Count < 0 | Lança ArgumentException |

#### RestTime (~6 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidSeconds_ShouldCreateRestTime | Seconds >= 0 | RestTime criado |
| Create_WithZeroSeconds_ShouldCreateRestTime | Seconds = 0 | RestTime criado (sem descanso) |
| Create_WithNegativeSeconds_ShouldThrowArgumentException | Seconds < 0 | Lança ArgumentException |

### 5.3 Testes de Handlers (Commands)

**Objetivo:** Validar orquestração de comandos com mocking de serviços

```csharp
public class CreateExerciseCommandHandlerTests
{
    private readonly Mock<IExerciseService> _mockService;
    private readonly CreateExerciseCommandHandler _handler;

    public CreateExerciseCommandHandlerTests()
    {
        _mockService = new Mock<IExerciseService>();

        _handler = new CreateExerciseCommandHandler(
            _mockService.Object,
            Mock.Of<IHttpContextAccessor>()
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateExerciseCommand(
            "Supino Reto",
            "Exercício para peito",
            MuscleGroup.Chest,
            ExerciseType.Hypertrophy,
            EquipmentType.Barbell
        );

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateExerciseDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _mockService.Verify(
            s => s.CreateAsync(It.IsAny<CreateExerciseDTO>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenServiceFails_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateExerciseCommand(
            "Supino Reto",
            "Exercício para peito",
            MuscleGroup.Chest,
            ExerciseType.Hypertrophy,
            null
        );

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateExerciseDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<int>("Erro ao criar exercício"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
```

#### Testes de Command Handlers (~45 testes)

| Handler | Testes | Cenários |
|---------|--------|----------|
| CreateExerciseCommandHandler | 3 | Sucesso, falha do serviço, validação |
| UpdateExerciseCommandHandler | 3 | Sucesso, exercício inexistente, falha |
| DeleteExerciseCommandHandler | 3 | Sucesso, exercício inexistente, falha |
| CreateRoutineCommandHandler | 3 | Sucesso, falha, validação |
| UpdateRoutineCommandHandler | 3 | Sucesso, rotina inexistente, falha |
| DeleteRoutineCommandHandler | 3 | Sucesso, rotina inexistente, falha |
| CreateRoutineExerciseCommandHandler | 3 | Sucesso, falha, validação |
| UpdateRoutineExerciseCommandHandler | 3 | Sucesso, inexistente, falha |
| DeleteRoutineExerciseCommandHandler | 3 | Sucesso, inexistente, falha |
| CreateTrainingSessionCommandHandler | 3 | Sucesso, falha, validação |
| UpdateTrainingSessionCommandHandler | 3 | Sucesso, sessão inexistente, falha |
| DeleteTrainingSessionCommandHandler | 3 | Sucesso, sessão inexistente, falha |
| CreateCompletedExerciseCommandHandler | 3 | Sucesso, falha, validação |
| UpdateCompletedExerciseCommandHandler | 3 | Sucesso, inexistente, falha |
| DeleteCompletedExerciseCommandHandler | 3 | Sucesso, inexistente, falha |

### 5.4 Testes de Handlers (Queries)

```csharp
public class GetExerciseByIdQueryHandlerTests
{
    private readonly Mock<IExerciseService> _mockService;
    private readonly GetExerciseByIdQueryHandler _handler;

    public GetExerciseByIdQueryHandlerTests()
    {
        _mockService = new Mock<IExerciseService>();

        _handler = new GetExerciseByIdQueryHandler(
            _mockService.Object,
            Mock.Of<IHttpContextAccessor>()
        );
    }

    [Fact]
    public async Task Handle_WhenExerciseExists_ReturnsSuccessWithData()
    {
        // Arrange
        var exerciseDTO = new ExerciseDTO(1, "Supino Reto", "Descrição", MuscleGroup.Chest, ExerciseType.Hypertrophy, EquipmentType.Barbell);
        var query = new GetExerciseByIdQuery(1);

        _mockService
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(exerciseDTO));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be("Supino Reto");
    }

    [Fact]
    public async Task Handle_WhenExerciseDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var query = new GetExerciseByIdQuery(999);

        _mockService
            .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ExerciseDTO>("Exercício não encontrado"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
```

#### Testes de Query Handlers (~22 testes)

| Handler | Testes | Cenários |
|---------|--------|----------|
| GetExerciseByIdQueryHandler | 2 | Encontrado, não encontrado |
| GetExercisesByFilterQueryHandler | 2 | Com resultados, sem resultados |
| GetRoutineByIdQueryHandler | 2 | Encontrado, não encontrado |
| GetRoutinesByFilterQueryHandler | 2 | Com resultados, sem resultados |
| GetRoutineExerciseByIdQueryHandler | 2 | Encontrado, não encontrado |
| GetRoutineExercisesByFilterQueryHandler | 2 | Com resultados, sem resultados |
| GetTrainingSessionByIdQueryHandler | 2 | Encontrado, não encontrado |
| GetTrainingSessionsByFilterQueryHandler | 2 | Com resultados, sem resultados |
| GetTrainingSessionsByUserQueryHandler | 2 | Com sessões, sem sessões |
| GetCompletedExerciseByIdQueryHandler | 2 | Encontrado, não encontrado |
| GetCompletedExercisesByFilterQueryHandler | 2 | Com resultados, sem resultados |

### 5.5 Testes de Services

```csharp
public class ExerciseServiceTests
{
    private readonly Mock<IExerciseRepository> _mockRepository;
    private readonly ExerciseService _service;

    public ExerciseServiceTests()
    {
        _mockRepository = new Mock<IExerciseRepository>();
        _service = new ExerciseService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidDTO_ReturnsSuccessWithId()
    {
        // Arrange
        var dto = new CreateExerciseDTO("Supino Reto", "Descrição", MuscleGroup.Chest, ExerciseType.Hypertrophy, EquipmentType.Barbell);

        _mockRepository
            .Setup(r => r.Create(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.Create(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetById(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise?)null);

        // Act
        var result = await _service.GetByIdAsync(999, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
```

### 5.6 Test Data Builders

```csharp
public class ExerciseBuilder
{
    private string _name = "Supino Reto";
    private string _description = "Exercício para peito com barra";
    private MuscleGroup _muscleGroup = MuscleGroup.Chest;
    private ExerciseType _type = ExerciseType.Hypertrophy;
    private EquipmentType? _equipmentType = EquipmentType.Barbell;

    public ExerciseBuilder WithName(string name) { _name = name; return this; }
    public ExerciseBuilder WithDescription(string description) { _description = description; return this; }
    public ExerciseBuilder WithMuscleGroup(MuscleGroup muscleGroup) { _muscleGroup = muscleGroup; return this; }
    public ExerciseBuilder WithType(ExerciseType type) { _type = type; return this; }
    public ExerciseBuilder WithEquipmentType(EquipmentType? equipmentType) { _equipmentType = equipmentType; return this; }

    public Exercise Build()
    {
        var exercise = new Exercise(_name, _description, _muscleGroup, _type);
        if (_equipmentType.HasValue)
            exercise.SetEquipmentType(_equipmentType.Value);
        return exercise;
    }

    public static ExerciseBuilder Default() => new();
}

public class RoutineBuilder
{
    private string _name = "Treino A - Peito e Tríceps";
    private string _description = "Rotina de treino focada em peito e tríceps";

    public RoutineBuilder WithName(string name) { _name = name; return this; }
    public RoutineBuilder WithDescription(string description) { _description = description; return this; }

    public Routine Build() => new Routine(_name, _description);

    public static RoutineBuilder Default() => new();
}

public class TrainingSessionBuilder
{
    private int _userId = 1;
    private int _routineId = 1;
    private DateTime _startTime = DateTime.UtcNow;
    private DateTime? _endTime = null;
    private string? _notes = null;

    public TrainingSessionBuilder WithUserId(int userId) { _userId = userId; return this; }
    public TrainingSessionBuilder WithRoutineId(int routineId) { _routineId = routineId; return this; }
    public TrainingSessionBuilder WithStartTime(DateTime startTime) { _startTime = startTime; return this; }
    public TrainingSessionBuilder WithEndTime(DateTime? endTime) { _endTime = endTime; return this; }
    public TrainingSessionBuilder WithNotes(string? notes) { _notes = notes; return this; }

    public TrainingSession Build()
    {
        return new TrainingSession(_userId, _routineId, _startTime, _endTime, _notes);
    }

    public static TrainingSessionBuilder Default() => new();
}
```

---

## Guia de Testes de Integração

### Princípios

1. **Infraestrutura Real:** Usar PostgreSQL real via Testcontainers
2. **Isolamento de Dados:** Limpar banco entre testes com Respawn
3. **Fixtures:** Reutilizar setup de containers
4. **Performance:** Otimizar para execução rápida

### Testcontainers Setup

```csharp
public class DatabaseFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;
    public GymDbContext DbContext { get; private set; } = null!;
    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("gym_test")
            .WithUsername("test")
            .WithPassword("test123")
            .WithCleanUp(true)
            .Build();

        await _postgresContainer.StartAsync();

        ConnectionString = _postgresContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<GymDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        DbContext = new GymDbContext(options);

        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        if (_postgresContainer != null)
            await _postgresContainer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        var respawner = await Respawner.CreateAsync(
            ConnectionString,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" }
            }
        );

        await respawner.ResetAsync(ConnectionString);
    }
}
```

### Testes de Repository

```csharp
[Trait("Category", "Integration")]
[Trait("Layer", "Infrastructure")]
public class ExerciseRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private readonly ExerciseRepository _repository;

    public ExerciseRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new ExerciseRepository(_fixture.DbContext);
    }

    public async Task InitializeAsync() => await _fixture.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_WithValidEntity_SavesToDatabase()
    {
        // Arrange
        var exercise = new Exercise("Supino Reto", "Exercício para peito", MuscleGroup.Chest, ExerciseType.Hypertrophy);

        // Act
        await _repository.Create(exercise);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var saved = await _repository.GetById(exercise.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Supino Reto");
        saved.MuscleGroup.Should().Be(MuscleGroup.Chest);
    }

    [Fact]
    public async Task FindByFilter_WithMuscleGroup_ReturnsMatchingExercises()
    {
        // Arrange
        var chestExercise = new Exercise("Supino Reto", "Peito", MuscleGroup.Chest, ExerciseType.Hypertrophy);
        var backExercise = new Exercise("Remada", "Costas", MuscleGroup.Back, ExerciseType.Strength);
        var chestExercise2 = new Exercise("Crucifixo", "Peito", MuscleGroup.Chest, ExerciseType.Hypertrophy);

        _fixture.DbContext.Exercises.AddRange(chestExercise, backExercise, chestExercise2);
        await _fixture.DbContext.SaveChangesAsync();

        var filter = new ExerciseFilterDTO { MuscleGroup = MuscleGroup.Chest };

        // Act
        var (results, total) = await _repository.FindByFilter(filter, CancellationToken.None);

        // Assert
        results.Should().HaveCount(2);
        results.Should().AllSatisfy(e => e.MuscleGroup.Should().Be(MuscleGroup.Chest));
        total.Should().Be(2);
    }
}
```

#### Testes de Repository (~30 testes)

| Repository | Testes | Cenários |
|-----------|--------|----------|
| ExerciseRepository | 6 | CRUD + filtro por MuscleGroup, filtro por Type |
| RoutineRepository | 6 | CRUD + filtro + include de RoutineExercises |
| RoutineExerciseRepository | 6 | CRUD + filtro por RoutineId |
| TrainingSessionRepository | 6 | CRUD + filtro por UserId + filtro por data |
| CompletedExerciseRepository | 6 | CRUD + filtro por TrainingSessionId |

#### Testes de Service de Integração (~30 testes)

| Service | Testes | Cenários |
|---------|--------|----------|
| ExerciseServiceIntegration | 8 | Create, GetById, GetAll, Update, Delete, GetByFilter |
| RoutineServiceIntegration | 8 | Create, GetById, GetAll, Update, Delete, GetByFilter |
| TrainingSessionServiceIntegration | 8 | Create, GetById, GetAll, Update, Delete, GetByUser |
| CompletedExerciseServiceIntegration | 6 | Create, GetById, Update, Delete, GetByFilter |

### Test Data Factory com Bogus

```csharp
public static class TestDataFactory
{
    private static readonly Faker<Exercise> _exerciseFaker = new Faker<Exercise>()
        .CustomInstantiator(f => new Exercise(
            f.Lorem.Sentence(2, 3),
            f.Lorem.Paragraph(),
            f.PickRandom<MuscleGroup>(),
            f.PickRandom<ExerciseType>()
        ));

    private static readonly Faker<Routine> _routineFaker = new Faker<Routine>()
        .CustomInstantiator(f => new Routine(
            f.Lorem.Sentence(3, 4),
            f.Lorem.Paragraph()
        ));

    public static Exercise CreateExercise() => _exerciseFaker.Generate();
    public static List<Exercise> CreateExercises(int count) => _exerciseFaker.Generate(count);
    public static Routine CreateRoutine() => _routineFaker.Generate();
    public static List<Routine> CreateRoutines(int count) => _routineFaker.Generate(count);
}
```

---

## Guia de Testes E2E

### Princípios

1. **Black Box:** Testar através da API HTTP
2. **Ambiente Real:** Containers para todas as dependências
3. **Scenarios:** Focar em jornadas completas de usuário
4. **Autenticação:** Incluir JWT ou TestAuthHandler nos testes

### WebApplicationFactory Setup

```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover DbContext existente
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GymDbContext>)
            );

            if (descriptor != null)
                services.Remove(descriptor);

            // Adicionar DbContext com container de teste
            services.AddDbContext<GymDbContext>(options =>
            {
                options.UseNpgsql(_postgresContainer!.GetConnectionString());
            });

            // Substituir autenticação para testes
            services.AddAuthentication("TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", null);
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("gym_e2e")
            .Build();

        await _postgresContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_postgresContainer != null)
            await _postgresContainer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
        var connectionString = _postgresContainer!.GetConnectionString();

        var respawner = await Respawner.CreateAsync(
            connectionString,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" }
            }
        );

        await respawner.ResetAsync(connectionString);
    }
}
```

### TestAuthHandler

```csharp
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "Test User")
        };

        var identity = new ClaimsIdentity(claims, "TestScheme");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
```

### Testes de Endpoints

```csharp
[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class ExercisesEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ExercisesEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task POST_CreateExercise_ReturnsCreatedResponse()
    {
        // Arrange
        var request = new
        {
            Name = "Supino Reto",
            Description = "Exercício para peito com barra",
            MuscleGroup = MuscleGroup.Chest,
            Type = ExerciseType.Hypertrophy,
            EquipmentType = EquipmentType.Barbell
        };

        // Act
        var response = await _client.PostAsync(
            "/api/exercises",
            request.ToJsonContent()
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.DeserializeEnvelopeAsync<int>();
        result.Data.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GET_GetExerciseById_WhenExists_ReturnsOk()
    {
        // Arrange - Criar exercício primeiro
        var createRequest = new
        {
            Name = "Supino Reto",
            Description = "Exercício para peito",
            MuscleGroup = MuscleGroup.Chest,
            Type = ExerciseType.Hypertrophy
        };

        var createResponse = await _client.PostAsync("/api/exercises", createRequest.ToJsonContent());
        var createResult = await createResponse.DeserializeEnvelopeAsync<int>();

        // Act
        var response = await _client.GetAsync($"/api/exercises/{createResult.Data}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.DeserializeEnvelopeAsync<ExerciseDTO>();
        result.Data.Name.Should().Be("Supino Reto");
    }

    [Fact]
    public async Task GET_GetExerciseById_WhenNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/exercises/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DELETE_DeleteExercise_WhenExists_ReturnsNoContent()
    {
        // Arrange
        var createRequest = new
        {
            Name = "Exercício Temporário",
            Description = "Para deletar",
            MuscleGroup = MuscleGroup.Chest,
            Type = ExerciseType.Strength
        };

        var createResponse = await _client.PostAsync("/api/exercises", createRequest.ToJsonContent());
        var createResult = await createResponse.DeserializeEnvelopeAsync<int>();

        // Act
        var response = await _client.DeleteAsync($"/api/exercises/{createResult.Data}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
```

### Testes de Scenario - Ciclo de Treino Completo

```csharp
[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class WorkoutLifecycleTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public WorkoutLifecycleTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CompleteWorkoutLifecycle_CreateRoutineTrainAndComplete()
    {
        // 1. Criar exercícios
        var exerciseIds = new List<int>();
        var exercises = new[]
        {
            new { Name = "Supino Reto", Description = "Peito", MuscleGroup = MuscleGroup.Chest, Type = ExerciseType.Hypertrophy },
            new { Name = "Tríceps Corda", Description = "Tríceps", MuscleGroup = MuscleGroup.Triceps, Type = ExerciseType.Hypertrophy }
        };

        foreach (var exercise in exercises)
        {
            var response = await _client.PostAsync("/api/exercises", exercise.ToJsonContent());
            response.EnsureSuccessStatusCode();
            var result = await response.DeserializeEnvelopeAsync<int>();
            exerciseIds.Add(result.Data);
        }

        // 2. Criar rotina
        var routineRequest = new { Name = "Treino A", Description = "Peito e Tríceps" };
        var routineResponse = await _client.PostAsync("/api/routines", routineRequest.ToJsonContent());
        routineResponse.EnsureSuccessStatusCode();
        var routineResult = await routineResponse.DeserializeEnvelopeAsync<int>();
        var routineId = routineResult.Data;

        // 3. Adicionar exercícios à rotina
        foreach (var exerciseId in exerciseIds)
        {
            var routineExerciseRequest = new
            {
                RoutineId = routineId,
                ExerciseId = exerciseId,
                Sets = 4,
                Repetitions = 12,
                RestBetweenSets = 60
            };

            var reResponse = await _client.PostAsync("/api/routineexercises", routineExerciseRequest.ToJsonContent());
            reResponse.EnsureSuccessStatusCode();
        }

        // 4. Iniciar sessão de treino
        var sessionRequest = new
        {
            RoutineId = routineId,
            StartTime = DateTime.UtcNow
        };

        var sessionResponse = await _client.PostAsync("/api/trainingsessions", sessionRequest.ToJsonContent());
        sessionResponse.EnsureSuccessStatusCode();
        var sessionResult = await sessionResponse.DeserializeEnvelopeAsync<int>();
        var sessionId = sessionResult.Data;

        // 5. Verificar sessão criada
        var getSessionResponse = await _client.GetAsync($"/api/trainingsessions/{sessionId}");
        getSessionResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 6. Verificar rotina com exercícios
        var getRoutineResponse = await _client.GetAsync($"/api/routines/{routineId}");
        var routineData = await getRoutineResponse.DeserializeEnvelopeAsync<RoutineDTO>();
        routineData.Data.RoutineExercises.Should().HaveCount(2);
    }
}
```

#### Testes E2E (~35 testes)

| Classe | Testes | Cenários |
|--------|--------|----------|
| HealthCheckTests | 1 | Verificar endpoint de saúde |
| ExercisesEndpointTests | 7 | CRUD completo + filtros |
| RoutinesEndpointTests | 7 | CRUD completo + filtros |
| TrainingSessionsEndpointTests | 7 | CRUD + filtro por usuário |
| CompletedExercisesEndpointTests | 5 | CRUD + filtros |
| WorkoutLifecycleTests | 4 | Ciclo completo de treino |
| RoutineManagementTests | 4 | Gerenciamento de rotinas com exercícios |

---

## Estratégia de Execução

### Execução Paralela

Configurar em `xunit.runner.json`:

```json
{
  "parallelizeAssembly": true,
  "parallelizeTestCollections": true,
  "maxParallelThreads": 4
}
```

Para testes que devem executar sequencialmente:

```csharp
[Collection("Sequential")]
public class DatabaseMigrationTests
{
    // Testes que não podem executar em paralelo
}

[CollectionDefinition("Sequential", DisableParallelization = true)]
public class SequentialCollection { }
```

### Categorização de Testes

```csharp
[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class ExerciseTests { }

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class CreateExerciseCommandHandlerTests { }

[Trait("Category", "Integration")]
[Trait("Layer", "Infrastructure")]
public class ExerciseRepositoryTests { }

[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class ExercisesEndpointTests { }
```

**Comandos de execução:**

```bash
# Apenas testes unitários
dotnet test --filter "Category=Unit"

# Apenas testes de integração
dotnet test --filter "Category=Integration"

# Apenas testes E2E
dotnet test --filter "Category=E2E"

# Todos exceto E2E
dotnet test --filter "Category!=E2E"

# Por camada
dotnet test --filter "Layer=Domain"
```

### Pipeline CI/CD

```yaml
name: Gym Tests

on: [push, pull_request]

jobs:
  unit-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Run Unit Tests
        run: dotnet test --filter "Category=Unit" --logger "trx" --collect:"XPlat Code Coverage"

      - name: Upload Coverage
        uses: codecov/codecov-action@v3
        with:
          files: '**/coverage.cobertura.xml'

  integration-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3

      - name: Run Integration Tests
        run: dotnet test --filter "Category=Integration"

  e2e-tests:
    runs-on: ubuntu-latest
    needs: [unit-tests, integration-tests]
    steps:
      - uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3

      - name: Run E2E Tests
        run: dotnet test --filter "Category=E2E"
```

---

## Metas de Cobertura

### Por Camada

| Camada | Meta | Justificativa |
|--------|------|---------------|
| **Domain - Entities** | 95%+ | Contém lógica de negócio crítica |
| **Domain - Value Objects** | 95%+ | Validações de tipos fundamentais |
| **Application - Handlers** | 90%+ | Orquestração de casos de uso |
| **Application - Services** | 85%+ | Lógica de negócio e orquestração |
| **Infrastructure - Repositories** | 80%+ | Acesso a dados (testado via integração) |
| **API - Controllers** | 75%+ | Camada de entrada (testada via E2E) |
| **TOTAL** | **85%+** | Cobertura geral saudável |

### Configuração de Cobertura

```xml
<PropertyGroup>
  <CollectCoverage>true</CollectCoverage>
  <CoverletOutputFormat>cobertura,opencover</CoverletOutputFormat>
  <CoverletOutput>./TestResults/</CoverletOutput>
  <Exclude>[*]*.Migrations.*,[*]*.Program,[*]*.Startup</Exclude>
  <ExcludeByAttribute>CompilerGeneratedAttribute,GeneratedCodeAttribute,ExcludeFromCodeCoverageAttribute</ExcludeByAttribute>
</PropertyGroup>
```

### Comandos de Cobertura

```bash
# Executar com cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Gerar relatório HTML
reportgenerator \
    -reports:"**/TestResults/coverage.cobertura.xml" \
    -targetdir:"TestResults/CoverageReport" \
    -reporttypes:Html

# Abrir relatório
start TestResults/CoverageReport/index.html  # Windows
```

### Métricas de Qualidade

- **Test Pass Rate:** 100% (todos os testes devem passar)
- **Code Coverage:** 85%+ no total
- **Performance:**
  - Testes unitários: < 1 segundo total
  - Testes de integração: < 30 segundos total
  - Testes E2E: < 2 minutos total

---

## Exemplos de Código

### Exemplo Completo: Teste de Value Object

```csharp
using FluentAssertions;
using Gym.Domain.ValueObjects;
using Gym.Domain.Enums;
using Xunit;

namespace Gym.UnitTests.Domain.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class WeightTests
{
    [Fact]
    public void Create_WithValidValues_ShouldCreateWeight()
    {
        // Arrange
        var value = 80;
        var unit = MeasurementUnit.Kilogram;

        // Act
        var weight = Weight.Create(value, unit);

        // Assert
        weight.Should().NotBeNull();
        weight.Value.Should().Be(80);
        weight.Unit.Should().Be(MeasurementUnit.Kilogram);
    }

    [Fact]
    public void Create_WithZeroValue_ShouldCreateWeight()
    {
        // Act
        var weight = Weight.Create(0, MeasurementUnit.Kilogram);

        // Assert
        weight.Should().NotBeNull();
        weight.Value.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var action = () => Weight.Create(-1, MeasurementUnit.Kilogram);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void Create_WithValidCount_ShouldCreateSetCount(int count)
    {
        // Act
        var setCount = SetCount.Create(count);

        // Assert
        setCount.Should().NotBeNull();
        setCount.Count.Should().Be(count);
    }
}
```

### Exemplo Completo: Teste de Entidade com Builder

```csharp
using FluentAssertions;
using Gym.Domain.Entities;
using Gym.Domain.Enums;
using Gym.Domain.Exceptions;
using Gym.UnitTests.Helpers.Builders;
using Xunit;

namespace Gym.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class RoutineTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateRoutine()
    {
        // Arrange & Act
        var routine = RoutineBuilder.Default().Build();

        // Assert
        routine.Should().NotBeNull();
        routine.Name.Should().NotBeNullOrWhiteSpace();
        routine.Description.Should().NotBeNullOrWhiteSpace();
        routine.RoutineExercises.Should().BeEmpty();
    }

    [Fact]
    public void AddExercise_WithValidRoutineExercise_ShouldAddToCollection()
    {
        // Arrange
        var routine = RoutineBuilder.Default().Build();
        var routineExercise = new RoutineExercise(
            routine.Id, 1,
            SetCount.Create(4),
            RepetitionCount.Create(12),
            RestTime.Create(60),
            null, null
        );

        // Act
        routine.AddExercise(routineExercise);

        // Assert
        routine.RoutineExercises.Should().ContainSingle();
    }

    [Fact]
    public void AddExercise_WithNullRoutineExercise_ShouldThrowDomainException()
    {
        // Arrange
        var routine = RoutineBuilder.Default().Build();

        // Act & Assert
        var action = () => routine.AddExercise(null!);

        action.Should().Throw<DomainException>()
            .WithMessage("*" + RoutineErrors.NullRoutineExercise + "*");
    }

    [Fact]
    public void RemoveExercise_WithExistingExercise_ShouldRemoveFromCollection()
    {
        // Arrange
        var routine = RoutineBuilder.Default().Build();
        var routineExercise = new RoutineExercise(
            routine.Id, 1,
            SetCount.Create(3),
            RepetitionCount.Create(10),
            RestTime.Create(45),
            null, null
        );
        routine.AddExercise(routineExercise);

        // Act
        routine.RemoveExercise(routineExercise);

        // Assert
        routine.RoutineExercises.Should().BeEmpty();
    }
}
```

### Exemplo Completo: Teste de TrainingSession

```csharp
using FluentAssertions;
using Gym.Domain.Entities;
using Gym.Domain.Exceptions;
using Gym.UnitTests.Helpers.Builders;
using Xunit;

namespace Gym.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class TrainingSessionTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateSession()
    {
        // Arrange & Act
        var session = TrainingSessionBuilder.Default().Build();

        // Assert
        session.Should().NotBeNull();
        session.UserId.Should().Be(1);
        session.EndTime.Should().BeNull();
        session.CompletedExercises.Should().BeEmpty();
    }

    [Fact]
    public void Update_WithEndTimeBeforeStartTime_ShouldThrowDomainException()
    {
        // Arrange
        var startTime = DateTime.UtcNow;
        var session = TrainingSessionBuilder.Default()
            .WithStartTime(startTime)
            .Build();

        // Act & Assert
        var action = () => session.Update(
            session.RoutineId,
            startTime,
            startTime.AddMinutes(-10), // EndTime before StartTime
            null
        );

        action.Should().Throw<DomainException>()
            .WithMessage("End time must be after start time");
    }

    [Fact]
    public void GetDuration_WhenSessionCompleted_ReturnsCorrectTimeSpan()
    {
        // Arrange
        var startTime = DateTime.UtcNow.AddHours(-1);
        var endTime = DateTime.UtcNow;
        var session = TrainingSessionBuilder.Default()
            .WithStartTime(startTime)
            .WithEndTime(endTime)
            .Build();

        // Act
        var duration = session.GetDuration();

        // Assert
        duration.Should().NotBeNull();
        duration!.Value.TotalMinutes.Should().BeApproximately(60, 1);
    }

    [Fact]
    public void GetDuration_WhenSessionNotCompleted_ReturnsNull()
    {
        // Arrange
        var session = TrainingSessionBuilder.Default().Build();

        // Act
        var duration = session.GetDuration();

        // Assert
        duration.Should().BeNull();
    }

    [Fact]
    public void Complete_WithNotes_ShouldSetEndTimeAndNotes()
    {
        // Arrange
        var session = TrainingSessionBuilder.Default().Build();
        var notes = "Ótimo treino!";

        // Act
        session.Complete(notes);

        // Assert
        session.EndTime.Should().NotBeNull();
        session.Notes.Should().Be(notes);
    }
}
```

---

## Resumo

### Checklist de Implementação

- [ ] **Fase 1: Testes Unitários**
  - [ ] Criar projeto Gym.UnitTests
  - [ ] Adicionar pacotes NuGet (FluentAssertions, AutoFixture, Bogus, Moq)
  - [ ] Criar Test Data Builders (Exercise, Routine, TrainingSession, CompletedExercise)
  - [ ] Implementar testes de Value Objects (Weight, SetCount, RepetitionCount, RestTime)
  - [ ] Implementar testes de entidades de domínio (Exercise, Routine, RoutineExercise, TrainingSession, CompletedExercise)
  - [ ] Implementar testes de Command Handlers (15 handlers)
  - [ ] Implementar testes de Query Handlers (11 handlers)
  - [ ] Implementar testes de Mappers (5)
  - [ ] Implementar testes de Services (5)

- [ ] **Fase 2: Testes de Integração**
  - [ ] Criar projeto Gym.IntegrationTests
  - [ ] Configurar Testcontainers (PostgreSQL)
  - [ ] Implementar DatabaseFixture com Respawn
  - [ ] Implementar TestDataFactory com Bogus
  - [ ] Implementar testes de Repositories (5)
  - [ ] Implementar testes de Services com DB real (4)

- [ ] **Fase 3: Testes E2E**
  - [ ] Criar projeto Gym.E2ETests
  - [ ] Configurar CustomWebApplicationFactory
  - [ ] Implementar TestAuthHandler
  - [ ] Implementar HttpResponseExtensions
  - [ ] Implementar testes de Endpoints (4 controllers)
  - [ ] Implementar testes de Scenarios (WorkoutLifecycle, RoutineManagement)

- [ ] **Fase 4: Documentação e Validação**
  - [ ] Executar todos os testes
  - [ ] Gerar relatório de cobertura
  - [ ] Validar metas de cobertura (85%+)

### Resultados Esperados

- **295+ testes** cobrindo todas as camadas
- **85%+ de cobertura de código**
- **100% de test pass rate**
- **CI/CD** pronto para execução automatizada
- **Documentação completa** para novos desenvolvedores

---

**Criado em:** 2026-02-28
**Versão:** 1.0
**Projeto:** LifeSync - Gym Microservice
