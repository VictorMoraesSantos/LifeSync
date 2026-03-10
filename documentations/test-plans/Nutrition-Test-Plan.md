# Plano de Testes - Nutrition Microservice

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

O microserviço **Nutrition** do projeto LifeSync é responsável por gerenciar diários alimentares, refeições, alimentos, líquidos e progresso diário dos usuários. Atualmente **não possui nenhum projeto de teste**. Este documento descreve a estratégia completa para implementar uma cobertura de testes robusta em todas as camadas.

### Objetivos

- **Criar projeto de testes unitários** com ~230 testes
- **Criar projeto de testes de integração** com ~65 testes
- **Criar projeto de testes E2E** com ~40 testes
- **Atingir 85%+ de cobertura de código** no projeto
- **Documentar padrões e práticas** de testes

### Arquitetura do Nutrition

```
Services/Nutrition/
├── Nutrition.Domain/            # Entidades, Value Objects, Enums, Interfaces, Events
├── Nutrition.Application/       # Casos de Uso (CQRS), DTOs, Services
├── Nutrition.Infrastructure/    # Repositórios, DbContext, Configurations
└── Nutrition.API/              # Controllers, Endpoints HTTP
```

**Padrões Utilizados:**
- Clean Architecture
- CQRS (Command Query Responsibility Segregation)
- Repository Pattern
- Result Pattern (error handling)
- Domain-Driven Design
- Domain Events (MealAddedToDiaryEvent, MealFoodAddedEvent, MealFoodRemovedEvent)
- Value Objects (DailyGoal)

### Entidades do Domínio

| Entidade | Propriedades Principais | Métodos de Negócio |
|----------|------------------------|--------------------|
| **Diary** | UserId, Date, TotalCalories (computed), TotalLiquidsMl (computed), Meals, Liquids | AddMeal(), RemoveMeal(), AddLiquid(), RemoveLiquid(), UpdateDate() |
| **Meal** | Name, Description, DiaryId, TotalCalories (computed), MealFoods | SetDiaryId(), Update(), AddMealFood(), RemoveMealFood() |
| **MealFood** | MealId, FoodId, Food, Quantity, TotalCalories (computed) | Update() |
| **Food** | Name, Calories, Protein?, Lipids?, Carbohydrates?, Calcium?, Magnesium?, Iron?, Sodium?, Potassium? | (Read-only, seeded) |
| **Liquid** | DiaryId, LiquidTypeId, LiquidType, Quantity | Update() |
| **LiquidType** | Name | (Read-only) |
| **DailyProgress** | UserId, Date, CaloriesConsumed, LiquidsConsumedMl, Goal (DailyGoal VO) | SetGoal(), ResetGoal(), SetConsumed(), AddCalories(), AddLiquidsQuantity(), GetCaloriesProgressPercentage(), GetLiquidsProgressPercentage(), IsGoalMet(), IsCaloriesGoalMet(), IsLiquidsGoalMet() |

### Value Objects

| Value Object | Validações |
|-------------|-----------|
| **DailyGoal** | Calories >= 0, QuantityMl >= 0 |

### Domain Events

| Evento | Entidade | Quando é disparado |
|--------|----------|-------------------|
| MealAddedToDiaryEvent | Diary | Quando uma refeição é adicionada ao diário |
| MealFoodAddedEvent | Meal | Quando um alimento é adicionado à refeição |
| MealFoodRemovedEvent | Meal | Quando um alimento é removido da refeição |

---

## Estrutura de Projetos

### Projetos de Teste

```
tests/
├── Nutrition.UnitTests/                     (NOVO)
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── DiaryTests.cs
│   │   │   ├── MealTests.cs
│   │   │   ├── MealFoodTests.cs
│   │   │   ├── LiquidTests.cs
│   │   │   └── DailyProgressTests.cs
│   │   ├── ValueObjects/
│   │   │   └── DailyGoalTests.cs
│   │   └── Events/
│   │       ├── MealAddedToDiaryEventTests.cs
│   │       ├── MealFoodAddedEventTests.cs
│   │       └── MealFoodRemovedEventTests.cs
│   ├── Application/
│   │   ├── Handlers/
│   │   │   ├── Commands/
│   │   │   │   ├── Diaries/
│   │   │   │   │   ├── CreateDiaryCommandHandlerTests.cs
│   │   │   │   │   ├── UpdateDiaryCommandHandlerTests.cs
│   │   │   │   │   └── DeleteDiaryCommandHandlerTests.cs
│   │   │   │   ├── Meals/
│   │   │   │   │   ├── CreateMealCommandHandlerTests.cs
│   │   │   │   │   ├── UpdateMealCommandHandlerTests.cs
│   │   │   │   │   ├── DeleteMealCommandHandlerTests.cs
│   │   │   │   │   ├── AddMealFoodCommandHandlerTests.cs
│   │   │   │   │   └── RemoveMealFoodCommandHandlerTests.cs
│   │   │   │   ├── MealFoods/
│   │   │   │   │   ├── CreateMealFoodCommandHandlerTests.cs
│   │   │   │   │   ├── UpdateMealFoodCommandHandlerTests.cs
│   │   │   │   │   └── DeleteMealFoodCommandHandlerTests.cs
│   │   │   │   ├── Liquids/
│   │   │   │   │   ├── CreateLiquidCommandHandlerTests.cs
│   │   │   │   │   ├── UpdateLiquidCommandHandlerTests.cs
│   │   │   │   │   └── DeleteLiquidCommandHandlerTests.cs
│   │   │   │   └── DailyProgress/
│   │   │   │       ├── CreateDailyProgressCommandHandlerTests.cs
│   │   │   │       ├── UpdateDailyProgressCommandHandlerTests.cs
│   │   │   │       ├── DeleteDailyProgressCommandHandlerTests.cs
│   │   │   │       └── SetGoalCommandHandlerTests.cs
│   │   │   └── Queries/
│   │   │       ├── Diaries/
│   │   │       │   ├── GetDiaryByIdQueryHandlerTests.cs
│   │   │       │   ├── GetDiariesByFilterQueryHandlerTests.cs
│   │   │       │   └── GetDiariesByUserQueryHandlerTests.cs
│   │   │       ├── Meals/
│   │   │       │   ├── GetMealByIdQueryHandlerTests.cs
│   │   │       │   ├── GetMealsByFilterQueryHandlerTests.cs
│   │   │       │   └── GetMealsByDiaryQueryHandlerTests.cs
│   │   │       ├── MealFoods/
│   │   │       │   ├── GetMealFoodByIdQueryHandlerTests.cs
│   │   │       │   ├── GetMealFoodsByFilterQueryHandlerTests.cs
│   │   │       │   └── GetMealFoodsByMealQueryHandlerTests.cs
│   │   │       ├── Liquids/
│   │   │       │   ├── GetLiquidByIdQueryHandlerTests.cs
│   │   │       │   ├── GetLiquidsByFilterQueryHandlerTests.cs
│   │   │       │   └── GetLiquidsByDiaryQueryHandlerTests.cs
│   │   │       └── DailyProgress/
│   │   │           ├── GetDailyProgressByIdQueryHandlerTests.cs
│   │   │           ├── GetDailyProgressByFilterQueryHandlerTests.cs
│   │   │           └── GetDailyProgressByUserQueryHandlerTests.cs
│   │   ├── Mappers/
│   │   │   ├── DiaryMapperTests.cs
│   │   │   ├── MealMapperTests.cs
│   │   │   ├── MealFoodMapperTests.cs
│   │   │   ├── LiquidMapperTests.cs
│   │   │   └── DailyProgressMapperTests.cs
│   │   └── Services/
│   │       ├── DiaryServiceTests.cs
│   │       ├── MealServiceTests.cs
│   │       ├── MealFoodServiceTests.cs
│   │       ├── LiquidServiceTests.cs
│   │       └── DailyProgressServiceTests.cs
│   └── Helpers/
│       └── Builders/
│           ├── DiaryBuilder.cs
│           ├── MealBuilder.cs
│           ├── MealFoodBuilder.cs
│           ├── LiquidBuilder.cs
│           ├── FoodBuilder.cs
│           └── DailyProgressBuilder.cs
│
├── Nutrition.IntegrationTests/              (NOVO)
│   ├── Fixtures/
│   │   └── DatabaseFixture.cs
│   ├── Repositories/
│   │   ├── DiaryRepositoryTests.cs
│   │   ├── MealRepositoryTests.cs
│   │   ├── MealFoodRepositoryTests.cs
│   │   ├── LiquidRepositoryTests.cs
│   │   └── DailyProgressRepositoryTests.cs
│   ├── Services/
│   │   ├── DiaryServiceIntegrationTests.cs
│   │   ├── MealServiceIntegrationTests.cs
│   │   ├── LiquidServiceIntegrationTests.cs
│   │   └── DailyProgressServiceIntegrationTests.cs
│   └── Helpers/
│       └── TestDataFactory.cs
│
└── Nutrition.E2ETests/                      (NOVO)
    ├── Fixtures/
    │   ├── CustomWebApplicationFactory.cs
    │   └── TestAuthHandler.cs
    ├── Tests/
    │   ├── HealthCheckTests.cs
    │   ├── DiariesEndpointTests.cs
    │   ├── MealsEndpointTests.cs
    │   ├── MealFoodsEndpointTests.cs
    │   ├── LiquidsEndpointTests.cs
    │   ├── DailyProgressEndpointTests.cs
    │   ├── NutritionTrackingLifecycleTests.cs
    │   └── DailyProgressGoalTests.cs
    └── Helpers/
        └── HttpResponseExtensions.cs
```

### Distribuição de Testes

| Projeto | Tipo | Quantidade | Status |
|---------|------|------------|--------|
| Nutrition.UnitTests | Unit | ~230 | Criar novo |
| Nutrition.IntegrationTests | Integration | ~65 | Criar novo |
| Nutrition.E2ETests | E2E | ~40 | Criar novo |
| **TOTAL** | - | **~335** | - |

---

## Convenções de Nomenclatura

### Padrão Principal: `MethodName_StateUnderTest_ExpectedBehavior`

**Exemplos:**
```csharp
AddMeal_WithValidMeal_ShouldAddToCollectionAndRaiseEvent()
RemoveMeal_WithNullMeal_ShouldThrowDomainException()
TotalCalories_WithMultipleMealFoods_ShouldReturnCorrectSum()
SetGoal_WithNegativeCalories_ShouldThrowDomainException()
GetCaloriesProgressPercentage_WhenGoalSet_ReturnsCorrectPercentage()
IsGoalMet_WhenBothGoalsMet_ReturnsTrue()
```

### Nomenclatura de Classes

- **Testes unitários:** `{ClassName}Tests`
  - `DiaryTests`, `MealTests`, `DailyProgressTests`, `DailyGoalTests`
- **Testes de integração:** `{ClassName}IntegrationTests` ou `{ClassName}Tests`
  - `DiaryServiceIntegrationTests`, `DiaryRepositoryTests`
- **Testes E2E:** `{FeatureName}Tests` ou `{Scenario}Tests`
  - `DiariesEndpointTests`, `NutritionTrackingLifecycleTests`

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

### 5.1 Testes de Domínio - Entidades

#### Diary (~25 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidParameters_ShouldCreateDiary | UserId e Date válidos | Diário criado, coleções vazias |
| Create_WithInvalidUserId_ShouldThrowDomainException | UserId <= 0 | Lança DomainException |
| AddMeal_WithValidMeal_ShouldAddToCollection | Refeição válida | Adicionada à coleção Meals |
| AddMeal_WithValidMeal_ShouldRaiseMealAddedEvent | Refeição válida | Evento MealAddedToDiaryEvent disparado |
| AddMeal_WithNullMeal_ShouldThrowDomainException | Refeição nula | Lança DomainException |
| RemoveMeal_WithExistingMeal_ShouldRemoveFromCollection | Refeição existente | Removida da coleção |
| RemoveMeal_WithNullMeal_ShouldThrowDomainException | Refeição nula | Lança DomainException |
| AddLiquid_WithValidLiquid_ShouldAddToCollection | Líquido válido | Adicionado à coleção Liquids |
| AddLiquid_WithNullLiquid_ShouldThrowDomainException | Líquido nulo | Lança DomainException |
| RemoveLiquid_WithExistingLiquid_ShouldRemoveFromCollection | Líquido existente | Removido da coleção |
| RemoveLiquid_WithNullLiquid_ShouldThrowDomainException | Líquido nulo | Lança DomainException |
| UpdateDate_WithValidDate_ShouldUpdateDate | Data válida | Data atualizada |
| TotalCalories_WithMultipleMeals_ShouldReturnCorrectSum | Múltiplas refeições | Soma correta das calorias |
| TotalCalories_WithNoMeals_ShouldReturnZero | Sem refeições | Retorna 0 |
| TotalLiquidsMl_WithMultipleLiquids_ShouldReturnCorrectSum | Múltiplos líquidos | Soma correta dos ml |
| TotalLiquidsMl_WithNoLiquids_ShouldReturnZero | Sem líquidos | Retorna 0 |
| Meals_ShouldBeReadOnly | Acesso à coleção | Retorna IReadOnlyCollection |
| Liquids_ShouldBeReadOnly | Acesso à coleção | Retorna IReadOnlyCollection |

#### Meal (~18 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidParameters_ShouldCreateMeal | Nome e descrição válidos | Refeição criada, MealFoods vazio |
| SetDiaryId_WithValidId_ShouldSetDiaryId | DiaryId válido | DiaryId atualizado |
| Update_WithValidParameters_ShouldUpdateProperties | Dados válidos | Propriedades atualizadas |
| AddMealFood_WithValidMealFood_ShouldAddToCollection | MealFood válido | Adicionado à coleção |
| AddMealFood_WithValidMealFood_ShouldRaiseMealFoodAddedEvent | MealFood válido | Evento MealFoodAddedEvent disparado |
| RemoveMealFood_WithExistingId_ShouldRemoveFromCollection | ID existente | Removido da coleção |
| RemoveMealFood_WithExistingId_ShouldRaiseMealFoodRemovedEvent | ID existente | Evento MealFoodRemovedEvent disparado |
| TotalCalories_WithMultipleFoods_ShouldReturnCorrectSum | Múltiplos alimentos com quantidades | Soma correta (Quantity × Food.Calories) |
| TotalCalories_WithNoFoods_ShouldReturnZero | Sem alimentos | Retorna 0 |
| MealFoods_ShouldBeReadOnly | Acesso à coleção | Retorna IReadOnlyCollection |

#### MealFood (~10 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidParameters_ShouldCreateMealFood | MealId, FoodId e Quantity válidos | Entidade criada |
| Create_WithInvalidMealId_ShouldThrowDomainException | MealId inválido | Lança DomainException |
| Create_WithInvalidFoodId_ShouldThrowDomainException | FoodId inválido | Lança DomainException |
| Create_WithZeroQuantity_ShouldThrowDomainException | Quantity = 0 | Lança DomainException |
| Create_WithNegativeQuantity_ShouldThrowDomainException | Quantity < 0 | Lança DomainException |
| Update_WithValidParameters_ShouldUpdateProperties | FoodId e Quantity válidos | Propriedades atualizadas |
| Update_WithZeroQuantity_ShouldThrowDomainException | Quantity = 0 | Lança DomainException |
| TotalCalories_ShouldReturnQuantityTimesCalories | Quantity × Food.Calories | Valor correto |

#### Liquid (~8 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidParameters_ShouldCreateLiquid | DiaryId, LiquidTypeId e Quantity válidos | Entidade criada |
| Create_WithInvalidDiaryId_ShouldThrowDomainException | DiaryId inválido | Lança DomainException |
| Create_WithInvalidLiquidTypeId_ShouldThrowDomainException | LiquidTypeId inválido | Lança DomainException |
| Create_WithZeroQuantity_ShouldThrowDomainException | Quantity = 0 | Lança DomainException |
| Create_WithNegativeQuantity_ShouldThrowDomainException | Quantity < 0 | Lança DomainException |
| Update_WithValidParameters_ShouldUpdateProperties | Dados válidos | Propriedades atualizadas |

#### DailyProgress (~30 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidParameters_ShouldCreateDailyProgress | UserId e Date válidos | Entidade criada |
| Create_WithPastDate_ShouldThrowDomainException | Data no passado | Lança DomainException(DailyProgressErrors.InvalidDate) |
| SetGoal_WithValidGoal_ShouldSetGoal | DailyGoal com valores positivos | Goal atualizado |
| ResetGoal_ShouldClearGoal | Após ter Goal definido | Goal resetado |
| SetConsumed_WithValidValues_ShouldSetValues | Calorias e líquidos positivos | Valores atualizados |
| AddCalories_WithPositiveValue_ShouldIncrementCalories | Valor positivo | CaloriesConsumed incrementado |
| AddCalories_MultipleTimes_ShouldAccumulate | Múltiplas chamadas | Soma acumulada |
| AddLiquidsQuantity_WithPositiveValue_ShouldIncrementLiquids | Valor positivo | LiquidsConsumedMl incrementado |
| AddLiquidsQuantity_MultipleTimes_ShouldAccumulate | Múltiplas chamadas | Soma acumulada |
| GetCaloriesProgressPercentage_WhenGoalSet_ReturnsCorrectPercentage | Meta de 2000cal, consumo de 1000cal | Retorna 50.0 |
| GetCaloriesProgressPercentage_WhenNoGoal_ReturnsZero | Sem meta definida | Retorna 0 |
| GetCaloriesProgressPercentage_WhenExceededGoal_ReturnsOver100 | Consumo > meta | Retorna > 100 |
| GetLiquidsProgressPercentage_WhenGoalSet_ReturnsCorrectPercentage | Meta definida | Percentual correto |
| GetLiquidsProgressPercentage_WhenNoGoal_ReturnsZero | Sem meta | Retorna 0 |
| IsGoalMet_WhenBothGoalsMet_ReturnsTrue | Calorias e líquidos atingidos | Retorna true |
| IsGoalMet_WhenOnlyCaloriesMet_ReturnsFalse | Apenas calorias atingidas | Retorna false |
| IsGoalMet_WhenOnlyLiquidsMet_ReturnsFalse | Apenas líquidos atingidos | Retorna false |
| IsGoalMet_WhenNoGoalsMet_ReturnsFalse | Nenhuma meta atingida | Retorna false |
| IsCaloriesGoalMet_WhenCaloriesReached_ReturnsTrue | Calorias >= meta | Retorna true |
| IsCaloriesGoalMet_WhenCaloriesNotReached_ReturnsFalse | Calorias < meta | Retorna false |
| IsLiquidsGoalMet_WhenLiquidsReached_ReturnsTrue | Líquidos >= meta | Retorna true |
| IsLiquidsGoalMet_WhenLiquidsNotReached_ReturnsFalse | Líquidos < meta | Retorna false |

### 5.2 Testes de Value Objects

#### DailyGoal (~8 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidValues_ShouldCreateDailyGoal | Calories e QuantityMl >= 0 | DailyGoal criado |
| Create_WithZeroCalories_ShouldCreateDailyGoal | Calories = 0 | DailyGoal criado |
| Create_WithZeroQuantityMl_ShouldCreateDailyGoal | QuantityMl = 0 | DailyGoal criado |
| Create_WithNegativeCalories_ShouldThrowDomainException | Calories < 0 | Lança DomainException |
| Create_WithNegativeQuantityMl_ShouldThrowDomainException | QuantityMl < 0 | Lança DomainException |
| Equality_WithSameValues_ShouldBeEqual | Mesmos valores | Records iguais |
| Equality_WithDifferentValues_ShouldNotBeEqual | Valores diferentes | Records diferentes |

### 5.3 Testes de Domain Events

```csharp
[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class MealAddedToDiaryEventTests
{
    [Fact]
    public void AddMeal_WhenCalled_ShouldRaiseMealAddedToDiaryEvent()
    {
        // Arrange
        var diary = DiaryBuilder.Default().Build();
        var meal = MealBuilder.Default().Build();

        // Act
        diary.AddMeal(meal);

        // Assert
        diary.DomainEvents.Should().ContainSingle();
        diary.DomainEvents.First().Should().BeOfType<MealAddedToDiaryEvent>();
        var domainEvent = (MealAddedToDiaryEvent)diary.DomainEvents.First();
        domainEvent.DiaryId.Should().Be(diary.Id);
        domainEvent.MealId.Should().Be(meal.Id);
    }
}
```

### 5.4 Testes de Handlers (Commands)

```csharp
public class CreateDiaryCommandHandlerTests
{
    private readonly Mock<IDiaryService> _mockService;
    private readonly CreateDiaryCommandHandler _handler;

    public CreateDiaryCommandHandlerTests()
    {
        _mockService = new Mock<IDiaryService>();

        _handler = new CreateDiaryCommandHandler(
            _mockService.Object,
            Mock.Of<IHttpContextAccessor>()
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateDiaryCommand(1, DateOnly.FromDateTime(DateTime.UtcNow));

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateDiaryDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _mockService.Verify(
            s => s.CreateAsync(It.IsAny<CreateDiaryDTO>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenServiceFails_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateDiaryCommand(1, DateOnly.FromDateTime(DateTime.UtcNow));

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateDiaryDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<int>("Erro ao criar diário"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
```

#### Testes de Command Handlers (~54 testes)

| Handler | Testes | Cenários |
|---------|--------|----------|
| CreateDiaryCommandHandler | 3 | Sucesso, falha do serviço, validação |
| UpdateDiaryCommandHandler | 3 | Sucesso, diário inexistente, falha |
| DeleteDiaryCommandHandler | 3 | Sucesso, diário inexistente, falha |
| CreateMealCommandHandler | 3 | Sucesso, falha, validação |
| UpdateMealCommandHandler | 3 | Sucesso, refeição inexistente, falha |
| DeleteMealCommandHandler | 3 | Sucesso, refeição inexistente, falha |
| AddMealFoodCommandHandler | 3 | Sucesso, refeição inexistente, falha |
| RemoveMealFoodCommandHandler | 3 | Sucesso, alimento inexistente, falha |
| CreateMealFoodCommandHandler | 3 | Sucesso, falha, validação |
| UpdateMealFoodCommandHandler | 3 | Sucesso, inexistente, falha |
| DeleteMealFoodCommandHandler | 3 | Sucesso, inexistente, falha |
| CreateLiquidCommandHandler | 3 | Sucesso, falha, validação |
| UpdateLiquidCommandHandler | 3 | Sucesso, inexistente, falha |
| DeleteLiquidCommandHandler | 3 | Sucesso, inexistente, falha |
| CreateDailyProgressCommandHandler | 3 | Sucesso, falha, validação |
| UpdateDailyProgressCommandHandler | 3 | Sucesso, inexistente, falha |
| DeleteDailyProgressCommandHandler | 3 | Sucesso, inexistente, falha |
| SetGoalCommandHandler | 3 | Sucesso, progresso inexistente, falha |

### 5.5 Testes de Handlers (Queries)

#### Testes de Query Handlers (~30 testes)

| Handler | Testes | Cenários |
|---------|--------|----------|
| GetDiaryByIdQueryHandler | 2 | Encontrado, não encontrado |
| GetDiariesByFilterQueryHandler | 2 | Com resultados, sem resultados |
| GetDiariesByUserQueryHandler | 2 | Com diários, sem diários |
| GetMealByIdQueryHandler | 2 | Encontrado, não encontrado |
| GetMealsByFilterQueryHandler | 2 | Com resultados, sem resultados |
| GetMealsByDiaryQueryHandler | 2 | Com refeições, sem refeições |
| GetMealFoodByIdQueryHandler | 2 | Encontrado, não encontrado |
| GetMealFoodsByFilterQueryHandler | 2 | Com resultados, sem resultados |
| GetMealFoodsByMealQueryHandler | 2 | Com alimentos, sem alimentos |
| GetLiquidByIdQueryHandler | 2 | Encontrado, não encontrado |
| GetLiquidsByFilterQueryHandler | 2 | Com resultados, sem resultados |
| GetLiquidsByDiaryQueryHandler | 2 | Com líquidos, sem líquidos |
| GetDailyProgressByIdQueryHandler | 2 | Encontrado, não encontrado |
| GetDailyProgressByFilterQueryHandler | 2 | Com resultados, sem resultados |
| GetDailyProgressByUserQueryHandler | 2 | Com progresso, sem progresso |

### 5.6 Test Data Builders

```csharp
public class DiaryBuilder
{
    private int _userId = 1;
    private DateOnly _date = DateOnly.FromDateTime(DateTime.UtcNow);

    public DiaryBuilder WithUserId(int userId) { _userId = userId; return this; }
    public DiaryBuilder WithDate(DateOnly date) { _date = date; return this; }

    public Diary Build() => new Diary(_userId, _date);

    public static DiaryBuilder Default() => new();
}

public class MealBuilder
{
    private string _name = "Café da Manhã";
    private string _description = "Primeira refeição do dia";
    private int _diaryId = 1;

    public MealBuilder WithName(string name) { _name = name; return this; }
    public MealBuilder WithDescription(string description) { _description = description; return this; }
    public MealBuilder WithDiaryId(int diaryId) { _diaryId = diaryId; return this; }

    public Meal Build()
    {
        var meal = new Meal(_name, _description);
        meal.SetDiaryId(_diaryId);
        return meal;
    }

    public static MealBuilder Default() => new();
}

public class FoodBuilder
{
    private string _name = "Arroz Branco";
    private int _calories = 130;
    private decimal? _protein = 2.7m;
    private decimal? _carbohydrates = 28.2m;
    private decimal? _lipids = 0.3m;

    public FoodBuilder WithName(string name) { _name = name; return this; }
    public FoodBuilder WithCalories(int calories) { _calories = calories; return this; }
    public FoodBuilder WithProtein(decimal? protein) { _protein = protein; return this; }
    public FoodBuilder WithCarbohydrates(decimal? carbs) { _carbohydrates = carbs; return this; }
    public FoodBuilder WithLipids(decimal? lipids) { _lipids = lipids; return this; }

    public Food Build() => new Food(_name, _calories, _protein, _lipids, _carbohydrates, null, null, null, null, null);

    public static FoodBuilder Default() => new();
}

public class DailyProgressBuilder
{
    private int _userId = 1;
    private DateOnly _date = DateOnly.FromDateTime(DateTime.UtcNow);
    private int _caloriesConsumed = 0;
    private int _liquidsConsumedMl = 0;
    private DailyGoal? _goal = null;

    public DailyProgressBuilder WithUserId(int userId) { _userId = userId; return this; }
    public DailyProgressBuilder WithDate(DateOnly date) { _date = date; return this; }
    public DailyProgressBuilder WithCaloriesConsumed(int calories) { _caloriesConsumed = calories; return this; }
    public DailyProgressBuilder WithLiquidsConsumedMl(int ml) { _liquidsConsumedMl = ml; return this; }
    public DailyProgressBuilder WithGoal(DailyGoal goal) { _goal = goal; return this; }

    public DailyProgress Build()
    {
        var progress = new DailyProgress(_userId, _date);
        if (_caloriesConsumed > 0 || _liquidsConsumedMl > 0)
            progress.SetConsumed(_caloriesConsumed, _liquidsConsumedMl);
        if (_goal != null)
            progress.SetGoal(_goal);
        return progress;
    }

    public static DailyProgressBuilder Default() => new();
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
    public NutritionDbContext DbContext { get; private set; } = null!;
    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("nutrition_test")
            .WithUsername("test")
            .WithPassword("test123")
            .WithCleanUp(true)
            .Build();

        await _postgresContainer.StartAsync();

        ConnectionString = _postgresContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<NutritionDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        DbContext = new NutritionDbContext(options);

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
                SchemasToInclude = new[] { "public" },
                TablesToIgnore = new Respawn.Graph.Table[] { "Foods", "LiquidTypes" } // Dados seedados
            }
        );

        await respawner.ResetAsync(ConnectionString);
    }
}
```

> **Nota:** As tabelas `Foods` e `LiquidTypes` são ignoradas pelo Respawn pois contêm dados seedados que não devem ser limpos entre testes.

### Testes de Repository

```csharp
[Trait("Category", "Integration")]
[Trait("Layer", "Infrastructure")]
public class DiaryRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private readonly DiaryRepository _repository;

    public DiaryRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new DiaryRepository(_fixture.DbContext);
    }

    public async Task InitializeAsync() => await _fixture.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_WithValidEntity_SavesToDatabase()
    {
        // Arrange
        var diary = new Diary(1, DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        await _repository.Create(diary);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var saved = await _repository.GetById(diary.Id);
        saved.Should().NotBeNull();
        saved!.UserId.Should().Be(1);
    }

    [Fact]
    public async Task GetByDate_WithExistingDate_ReturnsDiary()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var diary = new Diary(1, date);
        _fixture.DbContext.Diaries.Add(diary);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDate(1, date, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Date.Should().Be(date);
        result.UserId.Should().Be(1);
    }

    [Fact]
    public async Task GetAllByUserId_WithMultipleDiaries_ReturnsOnlyUserDiaries()
    {
        // Arrange
        var diary1 = new Diary(1, DateOnly.FromDateTime(DateTime.UtcNow));
        var diary2 = new Diary(2, DateOnly.FromDateTime(DateTime.UtcNow));
        var diary3 = new Diary(1, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        _fixture.DbContext.Diaries.AddRange(diary1, diary2, diary3);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var results = await _repository.GetAllByUserId(1, CancellationToken.None);

        // Assert
        results.Should().HaveCount(2);
        results.Should().AllSatisfy(d => d.UserId.Should().Be(1));
    }
}
```

#### Testes de Repository (~35 testes)

| Repository | Testes | Cenários |
|-----------|--------|----------|
| DiaryRepository | 8 | CRUD + GetByDate + GetAllByUserId + filtros |
| MealRepository | 6 | CRUD + filtros + include MealFoods |
| MealFoodRepository | 5 | CRUD + filtros por MealId |
| LiquidRepository | 5 | CRUD + filtros por DiaryId |
| DailyProgressRepository | 8 | CRUD + GetByUserIdAndDate + GetAllByUserId + filtros |

#### Testes de Service de Integração (~30 testes)

| Service | Testes | Cenários |
|---------|--------|----------|
| DiaryServiceIntegration | 8 | Create, GetById, GetAll, Update, Delete, GetByUser, AddMealToDiary |
| MealServiceIntegration | 8 | Create, GetById, Update, Delete, AddMealFood, RemoveMealFood |
| LiquidServiceIntegration | 6 | Create, GetById, Update, Delete, GetByFilter |
| DailyProgressServiceIntegration | 8 | Create, GetById, Update, Delete, GetByUser, SetGoal, SetConsumed |

### Test Data Factory com Bogus

```csharp
public static class TestDataFactory
{
    private static readonly Faker<Diary> _diaryFaker = new Faker<Diary>()
        .CustomInstantiator(f => new Diary(
            f.Random.Int(1, 100),
            DateOnly.FromDateTime(f.Date.Recent(30))
        ));

    private static readonly Faker<Meal> _mealFaker = new Faker<Meal>()
        .CustomInstantiator(f => new Meal(
            f.PickRandom(new[] { "Café da Manhã", "Almoço", "Jantar", "Lanche" }),
            f.Lorem.Sentence()
        ));

    public static Diary CreateDiary() => _diaryFaker.Generate();
    public static List<Diary> CreateDiaries(int count) => _diaryFaker.Generate(count);
    public static Meal CreateMeal() => _mealFaker.Generate();
    public static List<Meal> CreateMeals(int count) => _mealFaker.Generate(count);
}
```

---

## Guia de Testes E2E

### Princípios

1. **Black Box:** Testar através da API HTTP
2. **Ambiente Real:** Containers para todas as dependências
3. **Scenarios:** Focar em jornadas completas de usuário
4. **Autenticação:** TestAuthHandler para bypass de JWT

### WebApplicationFactory Setup

```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<NutritionDbContext>)
            );

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<NutritionDbContext>(options =>
            {
                options.UseNpgsql(_postgresContainer!.GetConnectionString());
            });

            services.AddAuthentication("TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", null);
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("nutrition_e2e")
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
        var connectionString = _postgresContainer!.GetConnectionString();

        var respawner = await Respawner.CreateAsync(
            connectionString,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" },
                TablesToIgnore = new Respawn.Graph.Table[] { "Foods", "LiquidTypes" }
            }
        );

        await respawner.ResetAsync(connectionString);
    }
}
```

### Testes de Endpoints

```csharp
[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class DiariesEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public DiariesEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task POST_CreateDiary_ReturnsCreatedResponse()
    {
        // Arrange
        var request = new { UserId = 1, Date = DateOnly.FromDateTime(DateTime.UtcNow) };

        // Act
        var response = await _client.PostAsync("/api/diaries", request.ToJsonContent());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.DeserializeEnvelopeAsync<int>();
        result.Data.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GET_GetDiaryById_WhenExists_ReturnsOkWithMealsAndLiquids()
    {
        // Arrange
        var createRequest = new { UserId = 1, Date = DateOnly.FromDateTime(DateTime.UtcNow) };
        var createResponse = await _client.PostAsync("/api/diaries", createRequest.ToJsonContent());
        var createResult = await createResponse.DeserializeEnvelopeAsync<int>();

        // Act
        var response = await _client.GetAsync($"/api/diaries/{createResult.Data}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.DeserializeEnvelopeAsync<DiaryDTO>();
        result.Data.UserId.Should().Be(1);
    }
}
```

### Testes de Scenario - Ciclo Nutricional Completo

```csharp
[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class NutritionTrackingLifecycleTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public NutritionTrackingLifecycleTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CompleteNutritionTracking_CreateDiaryAddMealsTrackProgress()
    {
        // 1. Criar diário do dia
        var diaryRequest = new { UserId = 1, Date = DateOnly.FromDateTime(DateTime.UtcNow) };
        var diaryResponse = await _client.PostAsync("/api/diaries", diaryRequest.ToJsonContent());
        diaryResponse.EnsureSuccessStatusCode();
        var diaryResult = await diaryResponse.DeserializeEnvelopeAsync<int>();
        var diaryId = diaryResult.Data;

        // 2. Criar refeição (Café da Manhã)
        var mealRequest = new { Name = "Café da Manhã", Description = "Primeira refeição" };
        var mealResponse = await _client.PostAsync("/api/meals", mealRequest.ToJsonContent());
        mealResponse.EnsureSuccessStatusCode();
        var mealResult = await mealResponse.DeserializeEnvelopeAsync<int>();
        var mealId = mealResult.Data;

        // 3. Adicionar alimentos à refeição
        var addFoodRequest = new { FoodId = 1, Quantity = 2 };
        var addFoodResponse = await _client.PostAsync(
            $"/api/meals/{mealId}/foods",
            addFoodRequest.ToJsonContent()
        );
        addFoodResponse.EnsureSuccessStatusCode();

        // 4. Adicionar líquido ao diário
        var liquidRequest = new { DiaryId = diaryId, LiquidTypeId = 1, Quantity = 500 };
        var liquidResponse = await _client.PostAsync("/api/liquids", liquidRequest.ToJsonContent());
        liquidResponse.EnsureSuccessStatusCode();

        // 5. Verificar diário completo
        var getDiaryResponse = await _client.GetAsync($"/api/diaries/{diaryId}");
        var diary = await getDiaryResponse.DeserializeEnvelopeAsync<DiaryDTO>();
        diary.Data.TotalCalories.Should().BeGreaterThan(0);
        diary.Data.TotalLiquidsMl.Should().Be(500);

        // 6. Criar progresso diário com meta
        var progressRequest = new { UserId = 1, Date = DateOnly.FromDateTime(DateTime.UtcNow) };
        var progressResponse = await _client.PostAsync("/api/dailyprogress", progressRequest.ToJsonContent());
        progressResponse.EnsureSuccessStatusCode();
        var progressResult = await progressResponse.DeserializeEnvelopeAsync<int>();

        // 7. Definir meta diária
        var goalRequest = new { Calories = 2000, QuantityMl = 2000 };
        var goalResponse = await _client.PutAsync(
            $"/api/dailyprogress/{progressResult.Data}/goal",
            goalRequest.ToJsonContent()
        );
        goalResponse.EnsureSuccessStatusCode();
    }
}
```

### Testes de Scenario - Progresso de Metas

```csharp
[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class DailyProgressGoalTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public DailyProgressGoalTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task SetGoalAndTrackProgress_ShouldShowCorrectPercentages()
    {
        // 1. Criar progresso diário
        var progressRequest = new { UserId = 1, Date = DateOnly.FromDateTime(DateTime.UtcNow) };
        var progressResponse = await _client.PostAsync("/api/dailyprogress", progressRequest.ToJsonContent());
        var progressResult = await progressResponse.DeserializeEnvelopeAsync<int>();
        var progressId = progressResult.Data;

        // 2. Definir meta (2000 cal, 2000 ml)
        var goalRequest = new { Calories = 2000, QuantityMl = 2000 };
        await _client.PutAsync($"/api/dailyprogress/{progressId}/goal", goalRequest.ToJsonContent());

        // 3. Registrar consumo parcial (1000 cal, 1500 ml)
        var consumedRequest = new { Calories = 1000, LiquidsMl = 1500 };
        await _client.PutAsync($"/api/dailyprogress/{progressId}/consumed", consumedRequest.ToJsonContent());

        // 4. Verificar progresso
        var getResponse = await _client.GetAsync($"/api/dailyprogress/{progressId}");
        var progress = await getResponse.DeserializeEnvelopeAsync<DailyProgressDTO>();

        progress.Data.CaloriesConsumed.Should().Be(1000);
        progress.Data.LiquidsConsumedMl.Should().Be(1500);
    }
}
```

#### Testes E2E (~40 testes)

| Classe | Testes | Cenários |
|--------|--------|----------|
| HealthCheckTests | 1 | Verificar endpoint de saúde |
| DiariesEndpointTests | 7 | CRUD completo + filtros por usuário e data |
| MealsEndpointTests | 7 | CRUD + AddMealFood + RemoveMealFood |
| MealFoodsEndpointTests | 5 | CRUD + filtros |
| LiquidsEndpointTests | 5 | CRUD + filtros por diário |
| DailyProgressEndpointTests | 7 | CRUD + SetGoal + SetConsumed + GetByUser |
| NutritionTrackingLifecycleTests | 4 | Ciclo completo de rastreamento nutricional |
| DailyProgressGoalTests | 4 | Gerenciamento de metas e progresso |

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

### Categorização de Testes

```csharp
[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class DiaryTests { }

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class CreateDiaryCommandHandlerTests { }

[Trait("Category", "Integration")]
[Trait("Layer", "Infrastructure")]
public class DiaryRepositoryTests { }

[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class DiariesEndpointTests { }
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
name: Nutrition Tests

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
| **Domain - Entities** | 95%+ | Lógica de negócio crítica (cálculo de calorias, progresso) |
| **Domain - Value Objects** | 95%+ | Validações de DailyGoal |
| **Domain - Events** | 90%+ | Eventos de domínio |
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
  <Exclude>[*]*.Migrations.*,[*]*.Program,[*]*.Startup,[*]*.SeedData.*</Exclude>
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

### Exemplo Completo: Teste de DailyProgress

```csharp
using FluentAssertions;
using Nutrition.Domain.Entities;
using Nutrition.Domain.ValueObjects;
using Nutrition.Domain.Exceptions;
using Nutrition.UnitTests.Helpers.Builders;
using Xunit;

namespace Nutrition.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class DailyProgressTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateDailyProgress()
    {
        // Arrange
        var userId = 1;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var progress = new DailyProgress(userId, date);

        // Assert
        progress.Should().NotBeNull();
        progress.UserId.Should().Be(userId);
        progress.Date.Should().Be(date);
        progress.CaloriesConsumed.Should().Be(0);
        progress.LiquidsConsumedMl.Should().Be(0);
        progress.Goal.Should().BeNull();
    }

    [Fact]
    public void SetGoal_WithValidGoal_ShouldSetGoal()
    {
        // Arrange
        var progress = DailyProgressBuilder.Default().Build();
        var goal = new DailyGoal(2000, 2000);

        // Act
        progress.SetGoal(goal);

        // Assert
        progress.Goal.Should().NotBeNull();
        progress.Goal!.Calories.Should().Be(2000);
        progress.Goal.QuantityMl.Should().Be(2000);
    }

    [Fact]
    public void GetCaloriesProgressPercentage_WhenGoalSet_ReturnsCorrectPercentage()
    {
        // Arrange
        var progress = DailyProgressBuilder.Default()
            .WithGoal(new DailyGoal(2000, 2000))
            .WithCaloriesConsumed(1000)
            .Build();

        // Act
        var percentage = progress.GetCaloriesProgressPercentage();

        // Assert
        percentage.Should().Be(50.0m);
    }

    [Fact]
    public void IsGoalMet_WhenBothGoalsMet_ReturnsTrue()
    {
        // Arrange
        var progress = DailyProgressBuilder.Default()
            .WithGoal(new DailyGoal(2000, 2000))
            .WithCaloriesConsumed(2000)
            .WithLiquidsConsumedMl(2000)
            .Build();

        // Act
        var result = progress.IsGoalMet();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsGoalMet_WhenOnlyCaloriesMet_ReturnsFalse()
    {
        // Arrange
        var progress = DailyProgressBuilder.Default()
            .WithGoal(new DailyGoal(2000, 2000))
            .WithCaloriesConsumed(2000)
            .WithLiquidsConsumedMl(500)
            .Build();

        // Act
        var result = progress.IsGoalMet();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AddCalories_MultipleTimes_ShouldAccumulate()
    {
        // Arrange
        var progress = DailyProgressBuilder.Default().Build();

        // Act
        progress.AddCalories(500);
        progress.AddCalories(300);
        progress.AddCalories(200);

        // Assert
        progress.CaloriesConsumed.Should().Be(1000);
    }

    [Fact]
    public void ResetGoal_ShouldClearGoal()
    {
        // Arrange
        var progress = DailyProgressBuilder.Default()
            .WithGoal(new DailyGoal(2000, 2000))
            .Build();

        // Act
        progress.ResetGoal();

        // Assert
        progress.Goal.Should().BeNull();
    }
}
```

### Exemplo Completo: Teste de Diary com Domain Events

```csharp
using FluentAssertions;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Events;
using Nutrition.Domain.Exceptions;
using Nutrition.UnitTests.Helpers.Builders;
using Xunit;

namespace Nutrition.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class DiaryTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateDiary()
    {
        // Arrange & Act
        var diary = DiaryBuilder.Default().Build();

        // Assert
        diary.Should().NotBeNull();
        diary.UserId.Should().Be(1);
        diary.Meals.Should().BeEmpty();
        diary.Liquids.Should().BeEmpty();
        diary.TotalCalories.Should().Be(0);
        diary.TotalLiquidsMl.Should().Be(0);
    }

    [Fact]
    public void AddMeal_WithValidMeal_ShouldAddToCollectionAndRaiseEvent()
    {
        // Arrange
        var diary = DiaryBuilder.Default().Build();
        var meal = MealBuilder.Default().Build();

        // Act
        diary.AddMeal(meal);

        // Assert
        diary.Meals.Should().ContainSingle();
        diary.DomainEvents.Should().ContainSingle();
        diary.DomainEvents.First().Should().BeOfType<MealAddedToDiaryEvent>();
    }

    [Fact]
    public void AddMeal_WithNullMeal_ShouldThrowDomainException()
    {
        // Arrange
        var diary = DiaryBuilder.Default().Build();

        // Act & Assert
        var action = () => diary.AddMeal(null!);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void RemoveMeal_WithExistingMeal_ShouldRemoveFromCollection()
    {
        // Arrange
        var diary = DiaryBuilder.Default().Build();
        var meal = MealBuilder.Default().Build();
        diary.AddMeal(meal);

        // Act
        diary.RemoveMeal(meal);

        // Assert
        diary.Meals.Should().BeEmpty();
    }

    [Fact]
    public void AddLiquid_WithValidLiquid_ShouldAddToCollection()
    {
        // Arrange
        var diary = DiaryBuilder.Default().Build();
        var liquid = new Liquid(diary.Id, 1, 500);

        // Act
        diary.AddLiquid(liquid);

        // Assert
        diary.Liquids.Should().ContainSingle();
        diary.TotalLiquidsMl.Should().Be(500);
    }
}
```

### Exemplo Completo: Teste de Value Object DailyGoal

```csharp
using FluentAssertions;
using Nutrition.Domain.ValueObjects;
using Nutrition.Domain.Exceptions;
using Xunit;

namespace Nutrition.UnitTests.Domain.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class DailyGoalTests
{
    [Fact]
    public void Create_WithValidValues_ShouldCreateDailyGoal()
    {
        // Act
        var goal = new DailyGoal(2000, 2000);

        // Assert
        goal.Calories.Should().Be(2000);
        goal.QuantityMl.Should().Be(2000);
    }

    [Fact]
    public void Create_WithZeroValues_ShouldCreateDailyGoal()
    {
        // Act
        var goal = new DailyGoal(0, 0);

        // Assert
        goal.Calories.Should().Be(0);
        goal.QuantityMl.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeCalories_ShouldThrowDomainException()
    {
        // Act & Assert
        var action = () => new DailyGoal(-1, 2000);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithNegativeQuantityMl_ShouldThrowDomainException()
    {
        // Act & Assert
        var action = () => new DailyGoal(2000, -1);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var goal1 = new DailyGoal(2000, 2000);
        var goal2 = new DailyGoal(2000, 2000);

        // Assert
        goal1.Should().Be(goal2);
    }
}
```

---

## Resumo

### Checklist de Implementação

- [ ] **Fase 1: Testes Unitários**
  - [ ] Criar projeto Nutrition.UnitTests
  - [ ] Adicionar pacotes NuGet (FluentAssertions, AutoFixture, Bogus, Moq)
  - [ ] Criar Test Data Builders (Diary, Meal, MealFood, Food, Liquid, DailyProgress)
  - [ ] Implementar testes de Value Objects (DailyGoal)
  - [ ] Implementar testes de entidades de domínio (Diary, Meal, MealFood, Liquid, DailyProgress)
  - [ ] Implementar testes de Domain Events (MealAdded, MealFoodAdded, MealFoodRemoved)
  - [ ] Implementar testes de Command Handlers (18 handlers)
  - [ ] Implementar testes de Query Handlers (15 handlers)
  - [ ] Implementar testes de Mappers (5)
  - [ ] Implementar testes de Services (5)

- [ ] **Fase 2: Testes de Integração**
  - [ ] Criar projeto Nutrition.IntegrationTests
  - [ ] Configurar Testcontainers (PostgreSQL)
  - [ ] Implementar DatabaseFixture com Respawn (excluindo tabelas seedadas)
  - [ ] Implementar TestDataFactory com Bogus
  - [ ] Implementar testes de Repositories (5)
  - [ ] Implementar testes de Services com DB real (4)

- [ ] **Fase 3: Testes E2E**
  - [ ] Criar projeto Nutrition.E2ETests
  - [ ] Configurar CustomWebApplicationFactory
  - [ ] Implementar TestAuthHandler
  - [ ] Implementar HttpResponseExtensions
  - [ ] Implementar testes de Endpoints (5 controllers)
  - [ ] Implementar testes de Scenarios (NutritionTrackingLifecycle, DailyProgressGoal)

- [ ] **Fase 4: Documentação e Validação**
  - [ ] Executar todos os testes
  - [ ] Gerar relatório de cobertura
  - [ ] Validar metas de cobertura (85%+)

### Resultados Esperados

- **335+ testes** cobrindo todas as camadas
- **85%+ de cobertura de código**
- **100% de test pass rate**
- **CI/CD** pronto para execução automatizada
- **Documentação completa** para novos desenvolvedores

---

**Criado em:** 2026-02-28
**Versão:** 1.0
**Projeto:** LifeSync - Nutrition Microservice
