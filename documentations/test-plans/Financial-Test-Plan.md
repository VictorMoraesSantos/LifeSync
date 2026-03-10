# Plano de Testes - Financial Microservice

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

O microserviço **Financial** do projeto LifeSync é responsável por gerenciar categorias e transações financeiras dos usuários. Atualmente possui **24 testes unitários** básicos cobrindo apenas entidades de domínio (CategoryTests e TransactionTests) utilizando `Assert` puro do xUnit. Este documento descreve a estratégia completa para expandir e modernizar a cobertura de testes.

### Estado Atual dos Testes

| Arquivo | Testes | Estilo | Observações |
|---------|--------|--------|-------------|
| CategoryTests.cs | 12 | Assert (xUnit) | Cobertura básica de criação/atualização |
| TransactionTests.cs | 12 | Assert (xUnit) | Cobertura básica de criação/atualização |

> **Nota Importante:** Os testes existentes usam `Assert.Throws<ArgumentNullException>` e `Assert.Throws<ArgumentOutOfRangeException>`, mas a entidade `Category` lança `DomainException`. Essa inconsistência precisa ser corrigida na migração para FluentAssertions.

### Objetivos

- **Migrar testes existentes** de Assert para FluentAssertions (24 testes)
- **Expandir testes unitários** de 24 para ~150 testes
- **Criar projeto de testes de integração** com ~40 testes
- **Criar projeto de testes E2E** com ~25 testes
- **Atingir 85%+ de cobertura de código** no projeto
- **Corrigir inconsistências** nos testes existentes

### Arquitetura do Financial

```
Services/Financial/
├── Financial.Domain/            # Entidades, Value Objects, Enums, Interfaces
├── Financial.Application/       # Casos de Uso (CQRS), DTOs, Services
├── Financial.Infrastructure/    # Repositórios, DbContext, Configurations
└── Financial.API/              # Controllers, Endpoints HTTP
```

**Padrões Utilizados:**
- Clean Architecture
- CQRS (Command Query Responsibility Segregation)
- Repository Pattern
- Result Pattern (error handling)
- Domain-Driven Design
- Value Objects (Money)

### Entidades do Domínio

| Entidade | Propriedades Principais | Métodos de Negócio |
|----------|------------------------|--------------------|
| **Category** | UserId, Name, Description? | Update(name, description?) |
| **Transaction** | UserId, CategoryId?, PaymentMethod, TransactionType, Amount (Money VO), Description, TransactionDate, IsRecurring | Update(categoryId?, paymentMethod, transactionType, amount, description, transactionDate, isRecurring) |

### Value Objects

| Value Object | Validações |
|-------------|-----------|
| **Money** | Amount >= 0, Currency válida (via `Money.Create(amount, currency)`) |

### Enums

- **TransactionType:** Income = 1, Expense = 2 (com `ToFriendlyString()`)
- **PaymentMethod:** Cash = 1, CreditCard = 2, DebitCard = 3, BankTransfer = 4, Pix = 5, Other = 6
- **Currency:** USD, EUR, BRL, GBP, JPY, CNY, AUD, CAD, CHF, INR + 100+ mais (com `ToSymbol()`)

---

## Estrutura de Projetos

### Projetos de Teste

```
tests/
├── Financial.UnitTests/                    (EXISTENTE - expandir e migrar)
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── CategoryTests.cs            (EXISTENTE - migrar para FluentAssertions)
│   │   │   └── TransactionTests.cs         (EXISTENTE - migrar para FluentAssertions)
│   │   ├── ValueObjects/
│   │   │   └── MoneyTests.cs               (NOVO)
│   │   └── Enums/
│   │       ├── TransactionTypeExtensionsTests.cs  (NOVO)
│   │       └── CurrencyExtensionsTests.cs         (NOVO)
│   ├── Application/
│   │   ├── Handlers/
│   │   │   ├── Commands/
│   │   │   │   ├── Categories/
│   │   │   │   │   ├── CreateCategoryCommandHandlerTests.cs
│   │   │   │   │   ├── UpdateCategoryCommandHandlerTests.cs
│   │   │   │   │   └── DeleteCategoryCommandHandlerTests.cs
│   │   │   │   └── Transactions/
│   │   │   │       ├── CreateTransactionCommandHandlerTests.cs
│   │   │   │       ├── UpdateTransactionCommandHandlerTests.cs
│   │   │   │       └── DeleteTransactionCommandHandlerTests.cs
│   │   │   └── Queries/
│   │   │       ├── Categories/
│   │   │       │   ├── GetCategoryByIdQueryHandlerTests.cs
│   │   │       │   ├── GetCategoriesByFilterQueryHandlerTests.cs
│   │   │       │   └── GetCategoriesByUserQueryHandlerTests.cs
│   │   │       └── Transactions/
│   │   │           ├── GetTransactionByIdQueryHandlerTests.cs
│   │   │           ├── GetTransactionsByFilterQueryHandlerTests.cs
│   │   │           └── GetTransactionsByUserQueryHandlerTests.cs
│   │   ├── Mappers/
│   │   │   ├── CategoryMapperTests.cs
│   │   │   └── TransactionMapperTests.cs
│   │   └── Services/
│   │       ├── CategoryServiceTests.cs
│   │       └── TransactionServiceTests.cs
│   └── Helpers/
│       └── Builders/
│           ├── CategoryBuilder.cs
│           ├── TransactionBuilder.cs
│           └── MoneyBuilder.cs
│
├── Financial.IntegrationTests/             (EXISTENTE - implementar)
│   ├── Fixtures/
│   │   └── DatabaseFixture.cs
│   ├── Repositories/
│   │   ├── CategoryRepositoryTests.cs
│   │   └── TransactionRepositoryTests.cs
│   ├── Services/
│   │   ├── CategoryServiceIntegrationTests.cs
│   │   └── TransactionServiceIntegrationTests.cs
│   └── Helpers/
│       └── TestDataFactory.cs
│
└── Financial.E2ETests/                     (NOVO)
    ├── Fixtures/
    │   ├── CustomWebApplicationFactory.cs
    │   └── TestAuthHandler.cs
    ├── Tests/
    │   ├── HealthCheckTests.cs
    │   ├── CategoriesEndpointTests.cs
    │   ├── TransactionsEndpointTests.cs
    │   ├── FinancialLifecycleTests.cs
    │   └── TransactionFilteringTests.cs
    └── Helpers/
        └── HttpResponseExtensions.cs
```

### Distribuição de Testes

| Projeto | Tipo | Quantidade | Status |
|---------|------|------------|--------|
| Financial.UnitTests | Unit | ~150 | Expandir (atual: 24) |
| Financial.IntegrationTests | Integration | ~40 | Implementar (projeto existe vazio) |
| Financial.E2ETests | E2E | ~25 | Criar novo |
| **TOTAL** | - | **~215** | - |

---

## Convenções de Nomenclatura

### Padrão Principal: `MethodName_StateUnderTest_ExpectedBehavior`

**Exemplos:**
```csharp
Create_WithValidParameters_ShouldCreateCategory()
Create_WithInvalidUserId_ShouldThrowDomainException()
Update_WithNullName_ShouldThrowDomainException()
Create_WithNegativeAmount_ShouldThrowArgumentOutOfRangeException()
Create_WithInvalidCurrency_ShouldThrowArgumentException()
ToFriendlyString_Income_ReturnsReceita()
GetByUserIdAsync_WithDateRange_ReturnsFilteredTransactions()
```

### Nomenclatura de Classes

- **Testes unitários:** `{ClassName}Tests`
  - `CategoryTests`, `TransactionTests`, `MoneyTests`, `CreateCategoryCommandHandlerTests`
- **Testes de integração:** `{ClassName}IntegrationTests` ou `{ClassName}Tests`
  - `CategoryServiceIntegrationTests`, `CategoryRepositoryTests`
- **Testes E2E:** `{FeatureName}Tests` ou `{Scenario}Tests`
  - `CategoriesEndpointTests`, `FinancialLifecycleTests`

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

### 5.1 Migração dos Testes Existentes

#### Antes (Assert puro - estilo atual):
```csharp
[Fact]
public void Deve_Criar_Categoria_Com_Parametros_Validos()
{
    var category = new Category(1, "Alimentação", "Gastos com comida");

    Assert.NotNull(category);
    Assert.Equal(1, category.UserId);
    Assert.Equal("Alimentação", category.Name);
    Assert.Equal("Gastos com comida", category.Description);
}
```

#### Depois (FluentAssertions - estilo alvo):
```csharp
[Fact]
[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public void Create_WithValidParameters_ShouldCreateCategory()
{
    // Arrange
    var userId = 1;
    var name = "Alimentação";
    var description = "Gastos com comida";

    // Act
    var category = new Category(userId, name, description);

    // Assert
    category.Should().NotBeNull();
    category.UserId.Should().Be(userId);
    category.Name.Should().Be(name);
    category.Description.Should().Be(description);
}
```

#### Correção de Inconsistência de Exceções:

```csharp
// ANTES (incorreto - Category lança DomainException, não ArgumentNullException):
[Fact]
public void Deve_Lancar_Excecao_Para_Nome_Nulo()
{
    Assert.Throws<ArgumentNullException>(() => new Category(1, null!, "Descrição"));
}

// DEPOIS (correto):
[Fact]
[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public void Create_WithNullName_ShouldThrowDomainException()
{
    // Arrange & Act
    var action = () => new Category(1, null!, "Descrição");

    // Assert
    action.Should().Throw<DomainException>()
        .WithMessage("*" + CategoryErrors.InvalidName + "*");
}
```

### 5.2 Testes de Domínio - Entidades

#### Category (~15 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidParameters_ShouldCreateCategory | UserId, Name, Description válidos | Entidade criada |
| Create_WithValidParametersWithoutDescription_ShouldCreateCategory | Sem descrição | Entidade criada, Description null |
| Create_WithZeroUserId_ShouldThrowDomainException | UserId = 0 | Lança DomainException(CategoryErrors.InvalidUserId) |
| Create_WithNegativeUserId_ShouldThrowDomainException | UserId < 0 | Lança DomainException(CategoryErrors.InvalidUserId) |
| Create_WithNullName_ShouldThrowDomainException | Nome nulo | Lança DomainException(CategoryErrors.InvalidName) |
| Create_WithEmptyName_ShouldThrowDomainException | Nome vazio | Lança DomainException(CategoryErrors.InvalidName) |
| Create_WithWhitespaceName_ShouldThrowDomainException | Nome com espaços | Lança DomainException(CategoryErrors.InvalidName) |
| Update_WithValidParameters_ShouldUpdateProperties | Nome e descrição válidos | Propriedades atualizadas |
| Update_WithNullDescription_ShouldUpdateNameAndClearDescription | Descrição nula | Nome atualizado, Description null |
| Update_WithNullName_ShouldThrowDomainException | Nome nulo no update | Lança DomainException |
| Update_WithEmptyName_ShouldThrowDomainException | Nome vazio no update | Lança DomainException |
| Update_WithWhitespaceName_ShouldThrowDomainException | Nome com espaços no update | Lança DomainException |
| Update_MultipleTimes_ShouldTrackUpdatedAt | Múltiplos updates | UpdatedAt atualizado a cada chamada |

#### Transaction (~20 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidParameters_ShouldCreateTransaction | Todos os parâmetros válidos | Entidade criada |
| Create_WithZeroUserId_ShouldThrowArgumentOutOfRangeException | UserId = 0 | Lança ArgumentOutOfRangeException |
| Create_WithNegativeUserId_ShouldThrowArgumentOutOfRangeException | UserId < 0 | Lança ArgumentOutOfRangeException |
| Create_WithNullAmount_ShouldThrowArgumentNullException | Amount nulo | Lança ArgumentNullException |
| Create_WithNullDescription_ShouldThrowArgumentNullException | Description nula | Lança ArgumentNullException |
| Create_WithEmptyDescription_ShouldThrowArgumentNullException | Description vazia | Lança exceção |
| Create_WithWhitespaceDescription_ShouldThrowArgumentNullException | Description espaços | Lança exceção |
| Create_WithLocalDate_ShouldConvertToUtc | Data local | TransactionDate em UTC |
| Create_WithUtcDate_ShouldKeepUtc | Data UTC | TransactionDate mantida em UTC |
| Create_WithCategoryId_ShouldSetCategory | CategoryId válido | CategoryId definido |
| Create_WithoutCategoryId_ShouldHaveNullCategory | Sem CategoryId | CategoryId null |
| Create_WithIsRecurringTrue_ShouldSetFlag | IsRecurring true | Flag definida |
| Create_DefaultIsRecurring_ShouldBeFalse | Sem IsRecurring | IsRecurring = false |
| Update_WithValidParameters_ShouldUpdateAllProperties | Dados válidos | Todas propriedades atualizadas |
| Update_WithNullAmount_ShouldThrowException | Amount nulo | Lança exceção |
| Update_WithNullDescription_ShouldThrowException | Description nula | Lança exceção |
| Update_WithInvalidCategoryId_ShouldThrowException | CategoryId inválido | Lança exceção |
| Update_WithLocalDate_ShouldConvertToUtc | Data local no update | Convertida para UTC |
| Update_ChangingTransactionType_ShouldUpdate | Mudar Income para Expense | TransactionType atualizado |
| Update_ChangingPaymentMethod_ShouldUpdate | Mudar Cash para Pix | PaymentMethod atualizado |

### 5.3 Testes de Value Objects

#### Money (~12 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| Create_WithValidValues_ShouldCreateMoney | Amount >= 0 e Currency válida | Money criado |
| Create_WithZeroAmount_ShouldCreateMoney | Amount = 0 | Money criado |
| Create_WithNegativeAmount_ShouldThrowArgumentOutOfRangeException | Amount < 0 | Lança ArgumentOutOfRangeException |
| Create_WithBRL_ShouldSetCurrency | Currency.BRL | Moeda correta |
| Create_WithUSD_ShouldSetCurrency | Currency.USD | Moeda correta |
| Create_WithEUR_ShouldSetCurrency | Currency.EUR | Moeda correta |
| Create_WithInvalidCurrency_ShouldThrowArgumentException | Currency inválida | Lança ArgumentException |
| Equality_WithSameValues_ShouldBeEqual | Mesmo Amount e Currency | Records iguais |
| Equality_WithDifferentAmount_ShouldNotBeEqual | Amounts diferentes | Records diferentes |
| Equality_WithDifferentCurrency_ShouldNotBeEqual | Currencies diferentes | Records diferentes |
| JsonDeserialization_ShouldWork | JSON válido | Money desserializado |

### 5.4 Testes de Enums/Extensions

#### TransactionType Extensions (~4 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| ToFriendlyString_Income_ReturnsReceita | TransactionType.Income | "Receita" |
| ToFriendlyString_Expense_ReturnsDespesa | TransactionType.Expense | "Despesa" |

#### Currency Extensions (~6 testes)

| Teste | Cenário | Resultado Esperado |
|-------|---------|-------------------|
| ToSymbol_BRL_ReturnsRS | Currency.BRL | "R$" |
| ToSymbol_USD_ReturnsDollarSign | Currency.USD | "$" |
| ToSymbol_EUR_ReturnsEuroSign | Currency.EUR | "€" |
| ToSymbol_GBP_ReturnsPoundSign | Currency.GBP | "£" |
| ToSymbol_JPY_ReturnsYenSign | Currency.JPY | "¥" |

### 5.5 Testes de Handlers (Commands)

```csharp
public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryService> _mockService;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _mockService = new Mock<ICategoryService>();

        _handler = new CreateCategoryCommandHandler(
            _mockService.Object,
            Mock.Of<IHttpContextAccessor>()
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateCategoryCommand(1, "Alimentação", "Gastos com comida");

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateCategoryDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _mockService.Verify(
            s => s.CreateAsync(It.IsAny<CreateCategoryDTO>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenServiceFails_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateCategoryCommand(1, "Alimentação", null);

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateCategoryDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<int>("Erro ao criar categoria"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
```

```csharp
public class CreateTransactionCommandHandlerTests
{
    private readonly Mock<ITransactionService> _mockService;
    private readonly CreateTransactionCommandHandler _handler;

    public CreateTransactionCommandHandlerTests()
    {
        _mockService = new Mock<ITransactionService>();

        _handler = new CreateTransactionCommandHandler(
            _mockService.Object,
            Mock.Of<IHttpContextAccessor>()
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateTransactionCommand(
            1,
            1,
            PaymentMethod.Pix,
            TransactionType.Expense,
            Money.Create(150, Currency.BRL),
            "Compras no supermercado",
            DateTime.UtcNow,
            false
        );

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateTransactionDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _mockService.Verify(
            s => s.CreateAsync(It.IsAny<CreateTransactionDTO>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
```

#### Testes de Command Handlers (~18 testes)

| Handler | Testes | Cenários |
|---------|--------|----------|
| CreateCategoryCommandHandler | 3 | Sucesso, falha do serviço, validação |
| UpdateCategoryCommandHandler | 3 | Sucesso, categoria inexistente, falha |
| DeleteCategoryCommandHandler | 3 | Sucesso, categoria inexistente, falha |
| CreateTransactionCommandHandler | 3 | Sucesso, falha do serviço, validação |
| UpdateTransactionCommandHandler | 3 | Sucesso, transação inexistente, falha |
| DeleteTransactionCommandHandler | 3 | Sucesso, transação inexistente, falha |

### 5.6 Testes de Handlers (Queries)

#### Testes de Query Handlers (~12 testes)

| Handler | Testes | Cenários |
|---------|--------|----------|
| GetCategoryByIdQueryHandler | 2 | Encontrada, não encontrada |
| GetCategoriesByFilterQueryHandler | 2 | Com resultados, sem resultados |
| GetCategoriesByUserQueryHandler | 2 | Com categorias, sem categorias |
| GetTransactionByIdQueryHandler | 2 | Encontrada, não encontrada |
| GetTransactionsByFilterQueryHandler | 2 | Com resultados, sem resultados |
| GetTransactionsByUserQueryHandler | 2 | Com transações, sem transações |

### 5.7 Testes de Services

```csharp
public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockRepository;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidDTO_ReturnsSuccessWithId()
    {
        // Arrange
        var dto = new CreateCategoryDTO(1, "Alimentação", "Gastos com comida");

        _mockRepository
            .Setup(r => r.Create(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.Create(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByNameAsync_WhenFound_ReturnsSuccess()
    {
        // Arrange
        var category = CategoryBuilder.Default().Build();

        _mockRepository
            .Setup(r => r.GetByNameContains("Alimentação", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category> { category });

        // Act
        var result = await _service.GetByNameAsync("Alimentação", 1, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetByUserIdAsync_WithTransactions_ReturnsCorrectCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            CategoryBuilder.Default().WithName("Alimentação").Build(),
            CategoryBuilder.Default().WithName("Transporte").Build()
        };

        _mockRepository
            .Setup(r => r.GetAllByUserId(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _service.GetByUserIdAsync(1, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
}
```

```csharp
public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _mockRepository;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _mockRepository = new Mock<ITransactionRepository>();
        _service = new TransactionService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithDateRange_ReturnsFilteredTransactions()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        _mockRepository
            .Setup(r => r.GetByUserIdAsync(1, startDate, endDate, null, null))
            .ReturnsAsync(new List<Transaction>());

        // Act
        var result = await _service.GetByUserIdAsync(1, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
```

### 5.8 Test Data Builders

```csharp
public class CategoryBuilder
{
    private int _userId = 1;
    private string _name = "Alimentação";
    private string? _description = "Gastos com alimentação";

    public CategoryBuilder WithUserId(int userId) { _userId = userId; return this; }
    public CategoryBuilder WithName(string name) { _name = name; return this; }
    public CategoryBuilder WithDescription(string? description) { _description = description; return this; }

    public Category Build() => new Category(_userId, _name, _description);

    public static CategoryBuilder Default() => new();
}

public class TransactionBuilder
{
    private int _userId = 1;
    private int? _categoryId = 1;
    private PaymentMethod _paymentMethod = PaymentMethod.Pix;
    private TransactionType _transactionType = TransactionType.Expense;
    private Money _amount = Money.Create(100, Currency.BRL);
    private string _description = "Compras no supermercado";
    private DateTime _transactionDate = DateTime.UtcNow;
    private bool _isRecurring = false;

    public TransactionBuilder WithUserId(int userId) { _userId = userId; return this; }
    public TransactionBuilder WithCategoryId(int? categoryId) { _categoryId = categoryId; return this; }
    public TransactionBuilder WithPaymentMethod(PaymentMethod method) { _paymentMethod = method; return this; }
    public TransactionBuilder WithTransactionType(TransactionType type) { _transactionType = type; return this; }
    public TransactionBuilder WithAmount(Money amount) { _amount = amount; return this; }
    public TransactionBuilder WithDescription(string description) { _description = description; return this; }
    public TransactionBuilder WithTransactionDate(DateTime date) { _transactionDate = date; return this; }
    public TransactionBuilder WithIsRecurring(bool isRecurring) { _isRecurring = isRecurring; return this; }

    public Transaction Build()
    {
        return new Transaction(
            _userId, _categoryId, _paymentMethod, _transactionType,
            _amount, _description, _transactionDate, _isRecurring
        );
    }

    public static TransactionBuilder Default() => new();
}

public class MoneyBuilder
{
    private int _amount = 100;
    private Currency _currency = Currency.BRL;

    public MoneyBuilder WithAmount(int amount) { _amount = amount; return this; }
    public MoneyBuilder WithCurrency(Currency currency) { _currency = currency; return this; }

    public Money Build() => Money.Create(_amount, _currency);

    public static MoneyBuilder Default() => new();
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
    public FinancialDbContext DbContext { get; private set; } = null!;
    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("financial_test")
            .WithUsername("test")
            .WithPassword("test123")
            .WithCleanUp(true)
            .Build();

        await _postgresContainer.StartAsync();

        ConnectionString = _postgresContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<FinancialDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        DbContext = new FinancialDbContext(options);

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
public class CategoryRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private readonly CategoryRepository _repository;

    public CategoryRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new CategoryRepository(_fixture.DbContext);
    }

    public async Task InitializeAsync() => await _fixture.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_WithValidEntity_SavesToDatabase()
    {
        // Arrange
        var category = new Category(1, "Alimentação", "Gastos com comida");

        // Act
        await _repository.Create(category);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var saved = await _repository.GetById(category.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Alimentação");
        saved.UserId.Should().Be(1);
    }

    [Fact]
    public async Task GetAllByUserId_WithMultipleCategories_ReturnsOnlyUserCategories()
    {
        // Arrange
        var cat1 = new Category(1, "Alimentação", null);
        var cat2 = new Category(2, "Transporte", null);
        var cat3 = new Category(1, "Lazer", null);

        _fixture.DbContext.Categories.AddRange(cat1, cat2, cat3);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var results = await _repository.GetAllByUserId(1, CancellationToken.None);

        // Assert
        results.Should().HaveCount(2);
        results.Should().AllSatisfy(c => c.UserId.Should().Be(1));
    }

    [Fact]
    public async Task GetByNameContains_WithPartialMatch_ReturnsMatchingCategories()
    {
        // Arrange
        var cat1 = new Category(1, "Alimentação", null);
        var cat2 = new Category(1, "Transporte", null);
        var cat3 = new Category(1, "Alimentação Saudável", null);

        _fixture.DbContext.Categories.AddRange(cat1, cat2, cat3);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var results = await _repository.GetByNameContains("Aliment", CancellationToken.None);

        // Assert
        results.Should().HaveCount(2);
        results.Should().AllSatisfy(c => c.Name.Should().Contain("Aliment"));
    }
}
```

```csharp
[Trait("Category", "Integration")]
[Trait("Layer", "Infrastructure")]
public class TransactionRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private readonly TransactionRepository _repository;

    public TransactionRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new TransactionRepository(_fixture.DbContext);
    }

    public async Task InitializeAsync() => await _fixture.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_WithValidTransaction_SavesToDatabase()
    {
        // Arrange
        var transaction = new Transaction(
            1, null, PaymentMethod.Pix, TransactionType.Expense,
            Money.Create(150, Currency.BRL), "Supermercado",
            DateTime.UtcNow, false
        );

        // Act
        await _repository.Create(transaction);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var saved = await _repository.GetById(transaction.Id);
        saved.Should().NotBeNull();
        saved!.Description.Should().Be("Supermercado");
        saved.Amount.Amount.Should().Be(150);
        saved.Amount.Currency.Should().Be(Currency.BRL);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithDateRange_ReturnsFilteredTransactions()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var t1 = new Transaction(1, null, PaymentMethod.Pix, TransactionType.Expense,
            Money.Create(100, Currency.BRL), "Hoje", now, false);
        var t2 = new Transaction(1, null, PaymentMethod.Cash, TransactionType.Income,
            Money.Create(200, Currency.BRL), "Ontem", now.AddDays(-1), false);
        var t3 = new Transaction(1, null, PaymentMethod.Pix, TransactionType.Expense,
            Money.Create(300, Currency.BRL), "Mês passado", now.AddDays(-45), false);

        _fixture.DbContext.Transactions.AddRange(t1, t2, t3);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var results = await _repository.GetByUserIdAsync(1, now.AddDays(-7), now);

        // Assert
        results.Should().HaveCount(2);
        results.Should().NotContain(t => t.Description == "Mês passado");
    }

    [Fact]
    public async Task GetByUserIdAsync_WithTransactionTypeFilter_ReturnsOnlyMatchingType()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var expense = new Transaction(1, null, PaymentMethod.Pix, TransactionType.Expense,
            Money.Create(100, Currency.BRL), "Despesa", now, false);
        var income = new Transaction(1, null, PaymentMethod.BankTransfer, TransactionType.Income,
            Money.Create(5000, Currency.BRL), "Salário", now, false);

        _fixture.DbContext.Transactions.AddRange(expense, income);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var results = await _repository.GetByUserIdAsync(1, now.AddDays(-1), now.AddDays(1), null, TransactionType.Income);

        // Assert
        results.Should().ContainSingle();
        results.First().TransactionType.Should().Be(TransactionType.Income);
    }
}
```

#### Testes de Repository (~20 testes)

| Repository | Testes | Cenários |
|-----------|--------|----------|
| CategoryRepository | 8 | CRUD + GetAllByUserId + GetByNameContains + filtros |
| TransactionRepository | 12 | CRUD + GetByUserId com filtros de data, tipo e categoria |

#### Testes de Service de Integração (~20 testes)

| Service | Testes | Cenários |
|---------|--------|----------|
| CategoryServiceIntegration | 10 | Create, GetById, GetAll, Update, Delete, GetByUser, GetByName |
| TransactionServiceIntegration | 10 | Create, GetById, GetAll, Update, Delete, GetByUser com filtros |

### Test Data Factory com Bogus

```csharp
public static class TestDataFactory
{
    private static readonly Faker<Category> _categoryFaker = new Faker<Category>()
        .CustomInstantiator(f => new Category(
            f.Random.Int(1, 100),
            f.PickRandom(new[] { "Alimentação", "Transporte", "Lazer", "Saúde", "Educação", "Moradia", "Vestuário" }),
            f.Lorem.Sentence()
        ));

    private static readonly Faker<Transaction> _transactionFaker = new Faker<Transaction>()
        .CustomInstantiator(f => new Transaction(
            f.Random.Int(1, 100),
            null,
            f.PickRandom<PaymentMethod>(),
            f.PickRandom<TransactionType>(),
            Money.Create(f.Random.Int(1, 10000), Currency.BRL),
            f.Commerce.ProductName(),
            f.Date.Recent(30).ToUniversalTime(),
            f.Random.Bool(0.2f)
        ));

    public static Category CreateCategory() => _categoryFaker.Generate();
    public static List<Category> CreateCategories(int count) => _categoryFaker.Generate(count);
    public static Transaction CreateTransaction() => _transactionFaker.Generate();
    public static List<Transaction> CreateTransactions(int count) => _transactionFaker.Generate(count);
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
                d => d.ServiceType == typeof(DbContextOptions<FinancialDbContext>)
            );

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<FinancialDbContext>(options =>
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
            .WithDatabase("financial_e2e")
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
                SchemasToInclude = new[] { "public" }
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
public class CategoriesEndpointTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CategoriesEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task POST_CreateCategory_ReturnsCreatedResponse()
    {
        // Arrange
        var request = new { UserId = 1, Name = "Alimentação", Description = "Gastos com comida" };

        // Act
        var response = await _client.PostAsync("/api/categories", request.ToJsonContent());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.DeserializeEnvelopeAsync<int>();
        result.Data.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GET_GetCategoryById_WhenExists_ReturnsOk()
    {
        // Arrange
        var createRequest = new { UserId = 1, Name = "Transporte", Description = "Gastos com transporte" };
        var createResponse = await _client.PostAsync("/api/categories", createRequest.ToJsonContent());
        var createResult = await createResponse.DeserializeEnvelopeAsync<int>();

        // Act
        var response = await _client.GetAsync($"/api/categories/{createResult.Data}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.DeserializeEnvelopeAsync<CategoryDTO>();
        result.Data.Name.Should().Be("Transporte");
    }

    [Fact]
    public async Task PUT_UpdateCategory_WhenExists_ReturnsOk()
    {
        // Arrange
        var createRequest = new { UserId = 1, Name = "Original", Description = "Descrição original" };
        var createResponse = await _client.PostAsync("/api/categories", createRequest.ToJsonContent());
        var createResult = await createResponse.DeserializeEnvelopeAsync<int>();

        var updateRequest = new { Name = "Atualizada", Description = "Nova descrição" };

        // Act
        var response = await _client.PutAsync(
            $"/api/categories/{createResult.Data}",
            updateRequest.ToJsonContent()
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DELETE_DeleteCategory_WhenExists_ReturnsNoContent()
    {
        // Arrange
        var createRequest = new { UserId = 1, Name = "Para Deletar" };
        var createResponse = await _client.PostAsync("/api/categories", createRequest.ToJsonContent());
        var createResult = await createResponse.DeserializeEnvelopeAsync<int>();

        // Act
        var response = await _client.DeleteAsync($"/api/categories/{createResult.Data}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
```

### Testes de Scenario - Ciclo Financeiro Completo

```csharp
[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class FinancialLifecycleTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public FinancialLifecycleTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CompleteFinancialLifecycle_CreateCategoriesAndTransactions()
    {
        // 1. Criar categorias
        var categories = new[]
        {
            new { UserId = 1, Name = "Alimentação", Description = "Gastos com comida" },
            new { UserId = 1, Name = "Transporte", Description = "Gastos com transporte" },
            new { UserId = 1, Name = "Salário", Description = "Receita mensal" }
        };

        var categoryIds = new List<int>();
        foreach (var category in categories)
        {
            var response = await _client.PostAsync("/api/categories", category.ToJsonContent());
            response.EnsureSuccessStatusCode();
            var result = await response.DeserializeEnvelopeAsync<int>();
            categoryIds.Add(result.Data);
        }

        // 2. Criar transações de despesa
        var expenseRequest = new
        {
            UserId = 1,
            CategoryId = categoryIds[0], // Alimentação
            PaymentMethod = PaymentMethod.Pix,
            TransactionType = TransactionType.Expense,
            Amount = new { Amount = 250, Currency = Currency.BRL },
            Description = "Supermercado mensal",
            TransactionDate = DateTime.UtcNow,
            IsRecurring = true
        };

        var expenseResponse = await _client.PostAsync("/api/transactions", expenseRequest.ToJsonContent());
        expenseResponse.EnsureSuccessStatusCode();

        // 3. Criar transação de receita
        var incomeRequest = new
        {
            UserId = 1,
            CategoryId = categoryIds[2], // Salário
            PaymentMethod = PaymentMethod.BankTransfer,
            TransactionType = TransactionType.Income,
            Amount = new { Amount = 5000, Currency = Currency.BRL },
            Description = "Salário de fevereiro",
            TransactionDate = DateTime.UtcNow,
            IsRecurring = true
        };

        var incomeResponse = await _client.PostAsync("/api/transactions", incomeRequest.ToJsonContent());
        incomeResponse.EnsureSuccessStatusCode();

        // 4. Verificar categorias do usuário
        var getCategoriesResponse = await _client.GetAsync("/api/categories/user/1");
        var userCategories = await getCategoriesResponse.DeserializeEnvelopeAsync<List<CategoryDTO>>();
        userCategories.Data.Should().HaveCount(3);

        // 5. Verificar transações do usuário
        var getTransactionsResponse = await _client.GetAsync("/api/transactions/user/1");
        var userTransactions = await getTransactionsResponse.DeserializeEnvelopeAsync<List<TransactionDTO>>();
        userTransactions.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task TransactionWithoutCategory_ShouldCreateSuccessfully()
    {
        // Arrange
        var request = new
        {
            UserId = 1,
            CategoryId = (int?)null,
            PaymentMethod = PaymentMethod.Cash,
            TransactionType = TransactionType.Expense,
            Amount = new { Amount = 50, Currency = Currency.BRL },
            Description = "Gasto avulso",
            TransactionDate = DateTime.UtcNow,
            IsRecurring = false
        };

        // Act
        var response = await _client.PostAsync("/api/transactions", request.ToJsonContent());

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeEnvelopeAsync<int>();
        result.Data.Should().BeGreaterThan(0);
    }
}
```

### Testes de Filtragem de Transações

```csharp
[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class TransactionFilteringTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TransactionFilteringTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task FilterByTransactionType_ShouldReturnOnlyMatchingTransactions()
    {
        // Arrange - criar transações de diferentes tipos
        var expense = new
        {
            UserId = 1, PaymentMethod = PaymentMethod.Pix,
            TransactionType = TransactionType.Expense,
            Amount = new { Amount = 100, Currency = Currency.BRL },
            Description = "Despesa", TransactionDate = DateTime.UtcNow, IsRecurring = false
        };

        var income = new
        {
            UserId = 1, PaymentMethod = PaymentMethod.BankTransfer,
            TransactionType = TransactionType.Income,
            Amount = new { Amount = 5000, Currency = Currency.BRL },
            Description = "Receita", TransactionDate = DateTime.UtcNow, IsRecurring = false
        };

        await _client.PostAsync("/api/transactions", expense.ToJsonContent());
        await _client.PostAsync("/api/transactions", income.ToJsonContent());

        // Act
        var response = await _client.GetAsync("/api/transactions/search?userId=1&transactionType=Income");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.DeserializeEnvelopeAsync<List<TransactionDTO>>();
        result.Data.Should().ContainSingle();
        result.Data.First().TransactionType.Should().Be(TransactionType.Income);
    }
}
```

#### Testes E2E (~25 testes)

| Classe | Testes | Cenários |
|--------|--------|----------|
| HealthCheckTests | 1 | Verificar endpoint de saúde |
| CategoriesEndpointTests | 7 | CRUD completo + filtro por usuário + busca por nome |
| TransactionsEndpointTests | 7 | CRUD + filtros por usuário, data, tipo |
| FinancialLifecycleTests | 5 | Ciclo completo: categorias + transações + consultas |
| TransactionFilteringTests | 5 | Filtros avançados: tipo, categoria, período, paginação |

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
public class CategoryTests { }

[Trait("Category", "Unit")]
[Trait("Layer", "Application")]
public class CreateCategoryCommandHandlerTests { }

[Trait("Category", "Integration")]
[Trait("Layer", "Infrastructure")]
public class CategoryRepositoryTests { }

[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class CategoriesEndpointTests { }
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
name: Financial Tests

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
| **Domain - Entities** | 95%+ | Lógica de negócio (validações, UTC conversion) |
| **Domain - Value Objects** | 95%+ | Validações do Money (amount, currency) |
| **Domain - Enums** | 90%+ | Extensions (ToFriendlyString, ToSymbol) |
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
  - Testes de integração: < 20 segundos total
  - Testes E2E: < 1.5 minutos total

---

## Exemplos de Código

### Exemplo Completo: Teste de Category (migrado)

```csharp
using FluentAssertions;
using Financial.Domain.Entities;
using Financial.Domain.Exceptions;
using Financial.UnitTests.Helpers.Builders;
using Xunit;

namespace Financial.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class CategoryTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateCategory()
    {
        // Arrange
        var userId = 1;
        var name = "Alimentação";
        var description = "Gastos com alimentação";

        // Act
        var category = new Category(userId, name, description);

        // Assert
        category.Should().NotBeNull();
        category.UserId.Should().Be(userId);
        category.Name.Should().Be(name);
        category.Description.Should().Be(description);
    }

    [Fact]
    public void Create_WithoutDescription_ShouldCreateCategoryWithNullDescription()
    {
        // Arrange & Act
        var category = new Category(1, "Alimentação", null);

        // Assert
        category.Should().NotBeNull();
        category.Description.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WithInvalidUserId_ShouldThrowDomainException(int invalidUserId)
    {
        // Arrange & Act
        var action = () => new Category(invalidUserId, "Alimentação", "Descrição");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("*" + CategoryErrors.InvalidUserId + "*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrowDomainException(string? invalidName)
    {
        // Arrange & Act
        var action = () => new Category(1, invalidName!, "Descrição");

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("*" + CategoryErrors.InvalidName + "*");
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var category = CategoryBuilder.Default().Build();

        // Act
        category.Update("Novo Nome", "Nova Descrição");

        // Assert
        category.Name.Should().Be("Novo Nome");
        category.Description.Should().Be("Nova Descrição");
    }

    [Fact]
    public void Update_WithNullDescription_ShouldClearDescription()
    {
        // Arrange
        var category = CategoryBuilder.Default()
            .WithDescription("Descrição original")
            .Build();

        // Act
        category.Update("Nome", null);

        // Assert
        category.Name.Should().Be("Nome");
        category.Description.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithInvalidName_ShouldThrowDomainException(string? invalidName)
    {
        // Arrange
        var category = CategoryBuilder.Default().Build();

        // Act & Assert
        var action = () => category.Update(invalidName!, "Descrição");
        action.Should().Throw<DomainException>();
    }
}
```

### Exemplo Completo: Teste de Money

```csharp
using FluentAssertions;
using Financial.Domain.ValueObjects;
using Financial.Domain.Enums;
using Xunit;

namespace Financial.UnitTests.Domain.ValueObjects;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class MoneyTests
{
    [Fact]
    public void Create_WithValidValues_ShouldCreateMoney()
    {
        // Act
        var money = Money.Create(100, Currency.BRL);

        // Assert
        money.Should().NotBeNull();
        money.Amount.Should().Be(100);
        money.Currency.Should().Be(Currency.BRL);
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldCreateMoney()
    {
        // Act
        var money = Money.Create(0, Currency.BRL);

        // Assert
        money.Should().NotBeNull();
        money.Amount.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange & Act
        var action = () => Money.Create(-1, Currency.BRL);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(Currency.BRL)]
    [InlineData(Currency.USD)]
    [InlineData(Currency.EUR)]
    [InlineData(Currency.GBP)]
    [InlineData(Currency.JPY)]
    public void Create_WithValidCurrency_ShouldSetCurrency(Currency currency)
    {
        // Act
        var money = Money.Create(100, currency);

        // Assert
        money.Currency.Should().Be(currency);
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100, Currency.BRL);
        var money2 = Money.Create(100, Currency.BRL);

        // Assert
        money1.Should().Be(money2);
    }

    [Fact]
    public void Equality_WithDifferentAmount_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100, Currency.BRL);
        var money2 = Money.Create(200, Currency.BRL);

        // Assert
        money1.Should().NotBe(money2);
    }

    [Fact]
    public void Equality_WithDifferentCurrency_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100, Currency.BRL);
        var money2 = Money.Create(100, Currency.USD);

        // Assert
        money1.Should().NotBe(money2);
    }
}
```

### Exemplo Completo: Teste de Transaction

```csharp
using FluentAssertions;
using Financial.Domain.Entities;
using Financial.Domain.ValueObjects;
using Financial.Domain.Enums;
using Financial.UnitTests.Helpers.Builders;
using Xunit;

namespace Financial.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class TransactionTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateTransaction()
    {
        // Arrange
        var userId = 1;
        var amount = Money.Create(150, Currency.BRL);
        var description = "Compras no supermercado";

        // Act
        var transaction = new Transaction(
            userId, 1, PaymentMethod.Pix, TransactionType.Expense,
            amount, description, DateTime.UtcNow, false
        );

        // Assert
        transaction.Should().NotBeNull();
        transaction.UserId.Should().Be(userId);
        transaction.Amount.Should().Be(amount);
        transaction.Description.Should().Be(description);
        transaction.PaymentMethod.Should().Be(PaymentMethod.Pix);
        transaction.TransactionType.Should().Be(TransactionType.Expense);
        transaction.IsRecurring.Should().BeFalse();
    }

    [Fact]
    public void Create_WithLocalDate_ShouldConvertToUtc()
    {
        // Arrange
        var localDate = new DateTime(2026, 2, 28, 10, 0, 0, DateTimeKind.Local);

        // Act
        var transaction = TransactionBuilder.Default()
            .WithTransactionDate(localDate)
            .Build();

        // Assert
        transaction.TransactionDate.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Create_WithoutCategoryId_ShouldHaveNullCategory()
    {
        // Arrange & Act
        var transaction = TransactionBuilder.Default()
            .WithCategoryId(null)
            .Build();

        // Assert
        transaction.CategoryId.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidUserId_ShouldThrowException(int invalidUserId)
    {
        // Arrange & Act
        var action = () => TransactionBuilder.Default()
            .WithUserId(invalidUserId)
            .Build();

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_WithNullAmount_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var action = () => new Transaction(
            1, null, PaymentMethod.Pix, TransactionType.Expense,
            null!, "Descrição", DateTime.UtcNow, false
        );

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Update_WithValidParameters_ShouldUpdateAllProperties()
    {
        // Arrange
        var transaction = TransactionBuilder.Default().Build();
        var newAmount = Money.Create(500, Currency.USD);

        // Act
        transaction.Update(
            2, PaymentMethod.CreditCard, TransactionType.Income,
            newAmount, "Nova descrição", DateTime.UtcNow, true
        );

        // Assert
        transaction.CategoryId.Should().Be(2);
        transaction.PaymentMethod.Should().Be(PaymentMethod.CreditCard);
        transaction.TransactionType.Should().Be(TransactionType.Income);
        transaction.Amount.Should().Be(newAmount);
        transaction.Description.Should().Be("Nova descrição");
        transaction.IsRecurring.Should().BeTrue();
    }
}
```

---

## Resumo

### Checklist de Implementação

- [ ] **Fase 0: Migração de Testes Existentes**
  - [ ] Adicionar pacotes NuGet (FluentAssertions, AutoFixture, Bogus, Moq)
  - [ ] Migrar CategoryTests.cs para FluentAssertions e corrigir exceções
  - [ ] Migrar TransactionTests.cs para FluentAssertions
  - [ ] Adicionar Traits [Category] e [Layer] nos testes existentes
  - [ ] Renomear testes para padrão inglês (MethodName_State_Expected)

- [ ] **Fase 1: Testes Unitários (Expandir)**
  - [ ] Criar Test Data Builders (Category, Transaction, Money)
  - [ ] Implementar testes de Value Objects (Money)
  - [ ] Implementar testes de Enum Extensions (TransactionType, Currency)
  - [ ] Implementar testes de Command Handlers (6 handlers)
  - [ ] Implementar testes de Query Handlers (6 handlers)
  - [ ] Implementar testes de Mappers (2)
  - [ ] Implementar testes de Services (2)

- [ ] **Fase 2: Testes de Integração**
  - [ ] Implementar projeto Financial.IntegrationTests (já existe vazio)
  - [ ] Configurar Testcontainers (PostgreSQL)
  - [ ] Implementar DatabaseFixture com Respawn
  - [ ] Implementar TestDataFactory com Bogus
  - [ ] Implementar testes de Repositories (2)
  - [ ] Implementar testes de Services com DB real (2)

- [ ] **Fase 3: Testes E2E**
  - [ ] Criar projeto Financial.E2ETests
  - [ ] Configurar CustomWebApplicationFactory
  - [ ] Implementar TestAuthHandler
  - [ ] Implementar HttpResponseExtensions
  - [ ] Implementar testes de Endpoints (2 controllers)
  - [ ] Implementar testes de Scenarios (FinancialLifecycle, TransactionFiltering)

- [ ] **Fase 4: Documentação e Validação**
  - [ ] Executar todos os testes
  - [ ] Gerar relatório de cobertura
  - [ ] Validar metas de cobertura (85%+)

### Resultados Esperados

- **215+ testes** cobrindo todas as camadas
- **85%+ de cobertura de código**
- **100% de test pass rate**
- **CI/CD** pronto para execução automatizada
- **Documentação completa** para novos desenvolvedores

---

**Criado em:** 2026-02-28
**Versão:** 1.0
**Projeto:** LifeSync - Financial Microservice
