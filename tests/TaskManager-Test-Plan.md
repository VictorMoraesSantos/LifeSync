# Plano de Testes - TaskManager Microservice

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

O microserviço **TaskManager** do projeto LifeSync é responsável por gerenciar tarefas e etiquetas (labels) dos usuários. Atualmente, possui apenas **42 testes unitários** que cobrem parcialmente a entidade `TaskItem`. Este documento descreve a estratégia completa para atingir uma cobertura de testes robusta.

### Objetivos

- **Expandir testes unitários** de 42 para ~220 testes
- **Criar projeto de testes de integração** com ~70 testes
- **Criar projeto de testes E2E** com ~40 testes
- **Atingir 85%+ de cobertura de código** no projeto
- **Documentar padrões e práticas** de testes

### Arquitetura do TaskManager

```
TaskManager/
├── TaskManager.Domain/          # Entidades, Value Objects, Interfaces
├── TaskManager.Application/     # Casos de Uso (CQRS), Validators, DTOs
├── TaskManager.Infrastructure/  # Repositórios, DbContext, Services
└── TaskManager.API/            # Controllers, Endpoints HTTP
```

**Padrões Utilizados:**
- Clean Architecture
- CQRS (Command Query Responsibility Segregation)
- Repository Pattern
- Specification Pattern
- Result Pattern (error handling)
- FluentValidation
- Domain-Driven Design

---

## Estrutura de Projetos

### Projetos de Teste

```
tests/
├── TaskManager.UnitTests/              (EXISTENTE - expandir)
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── TaskItemTests.cs
│   │   │   └── TaskLabelTests.cs       (NOVO)
│   │   ├── Specifications/             (NOVO)
│   │   └── Filters/                    (NOVO)
│   ├── Application/
│   │   ├── Validators/
│   │   │   ├── TaskItems/
│   │   │   └── TaskLabels/             (NOVO)
│   │   ├── Handlers/                   (NOVO)
│   │   │   ├── Commands/
│   │   │   └── Queries/
│   │   ├── Mappers/                    (NOVO)
│   │   └── Services/
│   └── Helpers/
│       └── Builders/                   (NOVO)
│
├── TaskManager.IntegrationTests/       (NOVO)
│   ├── Fixtures/
│   ├── Repositories/
│   ├── Services/
│   ├── Handlers/
│   ├── BackgroundServices/
│   ├── Events/
│   └── Helpers/
│
└── TaskManager.E2ETests/               (NOVO)
    ├── Fixtures/
    ├── Controllers/
    ├── Scenarios/
    └── Helpers/
```

### Distribuição de Testes

| Projeto | Tipo | Quantidade | Status |
|---------|------|------------|--------|
| TaskManager.UnitTests | Unit | ~220 | Expandir (atual: 42) |
| TaskManager.IntegrationTests | Integration | ~70 | Criar novo |
| TaskManager.E2ETests | E2E | ~40 | Criar novo |
| **TOTAL** | - | **~330** | - |

---

## Convenções de Nomenclatura

### Padrão Principal: `MethodName_StateUnderTest_ExpectedBehavior`

**Exemplos:**
```csharp
Create_WithValidParameters_ShouldCreateEntity()
Update_WithInvalidTitle_ShouldThrowDomainException()
GetByIdAsync_WhenTaskExists_ReturnsSuccessResult()
Handle_WithInvalidCommand_ReturnsValidationError()
```

### Padrão Alternativo: `Given_When_Then` (para testes de comportamento)

**Exemplos:**
```csharp
GivenValidCommand_WhenHandlerExecutes_ThenCreatesTaskItem()
GivenNonExistentId_WhenQuerying_ThenReturnsNotFoundError()
```

### Nomenclatura de Classes

- **Testes unitários:** `{ClassName}Tests`
  - `TaskItemTests`, `CreateTaskItemCommandHandlerTests`
- **Testes de integração:** `{ClassName}IntegrationTests`
  - `TaskItemServiceIntegrationTests`, `TaskItemRepositoryTests`
- **Testes E2E:** `{FeatureName}Tests` ou `{Scenario}Tests`
  - `TaskItemsControllerTests`, `TaskLifecycleTests`

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
  <PackageReference Include="Testcontainers.RabbitMq" Version="3.10.0" />

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
  <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="9.0.0" />

  <!-- Containers -->
  <PackageReference Include="Testcontainers.PostgreSql" Version="3.10.0" />
  <PackageReference Include="Testcontainers.RabbitMq" Version="3.10.0" />

  <!-- Assertions -->
  <PackageReference Include="FluentAssertions" Version="7.0.0" />
  <PackageReference Include="FluentAssertions.Web" Version="1.2.2" />
</ItemGroup>
```

### Ferramentas de Cobertura

```bash
# Instalar ReportGenerator globalmente
dotnet tool install -g dotnet-reportgenerator-globaltool

# Executar testes com cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Gerar relatório HTML
reportgenerator -reports:"**/TestResults/coverage.cobertura.xml" \
                -targetdir:"TestResults/CoverageReport" \
                -reporttypes:Html
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
public void Create_WithValidParameters_ShouldCreateEntity()
{
    // Arrange (Preparar)
    var title = "Valid Title";
    var description = "Valid Description";
    var priority = Priority.High;
    var dueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
    var userId = 1;

    // Act (Agir)
    var taskItem = new TaskItem(title, description, priority, dueDate, userId, null);

    // Assert (Verificar)
    Assert.NotNull(taskItem);
    Assert.Equal(title, taskItem.Title);
    Assert.Equal(description, taskItem.Description);
    Assert.Equal(priority, taskItem.Priority);
    Assert.Equal(dueDate, taskItem.DueDate);
    Assert.Equal(userId, taskItem.UserId);
    Assert.Equal(Status.Pending, taskItem.Status);
}
```

### Testes de Domínio

**Objetivo:** Validar regras de negócio nas entidades

```csharp
public class TaskItemTests
{
    [Fact]
    public void ChangeStatus_FromPendingToInProgress_ShouldUpdateStatus()
    {
        // Arrange
        var taskItem = TaskItemBuilder.Default().Build();

        // Act
        taskItem.ChangeStatus(Status.InProgress);

        // Assert
        taskItem.Status.Should().Be(Status.InProgress);
    }

    [Fact]
    public void AddLabel_WithValidLabel_ShouldAddToCollection()
    {
        // Arrange
        var taskItem = TaskItemBuilder.Default().Build();
        var label = TaskLabelBuilder.Default().Build();

        // Act
        taskItem.AddLabel(label);

        // Assert
        taskItem.Labels.Should().Contain(label);
    }

    [Fact]
    public void AddLabel_WithDuplicateLabel_ShouldThrowDomainException()
    {
        // Arrange
        var taskItem = TaskItemBuilder.Default().Build();
        var label = TaskLabelBuilder.Default().Build();
        taskItem.AddLabel(label);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => taskItem.AddLabel(label));
        exception.Message.Should().Contain("já está associada");
    }
}
```

### Testes de Validators

**Objetivo:** Validar regras de validação com FluentValidation

```csharp
public class CreateTaskItemCommandValidatorTests
{
    private readonly CreateTaskItemCommandValidator _validator;

    public CreateTaskItemCommandValidatorTests()
    {
        _validator = new CreateTaskItemCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldBeValid()
    {
        // Arrange
        var command = new CreateTaskItemCommand(
            "Valid Title",
            "Valid Description",
            Priority.Medium,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            1
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Validate_WithEmptyTitle_ShouldBeInvalid(string? title)
    {
        // Arrange
        var command = new CreateTaskItemCommand(
            title!,
            "Valid Description",
            Priority.Medium,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            1
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Title");
        result.Errors.First().ErrorMessage.Should().Be("O título é obrigatório.");
    }
}
```

### Testes de Handlers

**Objetivo:** Validar orquestração de comandos/queries

```csharp
public class CreateTaskItemCommandHandlerTests
{
    private readonly Mock<ITaskItemService> _mockService;
    private readonly Mock<IValidator<CreateTaskItemCommand>> _mockValidator;
    private readonly CreateTaskItemCommandHandler _handler;

    public CreateTaskItemCommandHandlerTests()
    {
        _mockService = new Mock<ITaskItemService>();
        _mockValidator = new Mock<IValidator<CreateTaskItemCommand>>();

        _handler = new CreateTaskItemCommandHandler(
            _mockService.Object,
            _mockValidator.Object,
            Mock.Of<IHttpContextAccessor>()
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateTaskItemCommand(
            "Test Task",
            "Test Description",
            Priority.High,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            1
        );

        _mockValidator
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockService
            .Setup(s => s.CreateAsync(It.IsAny<CreateTaskItemDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(1);

        _mockService.Verify(
            s => s.CreateAsync(It.IsAny<CreateTaskItemDTO>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
```

### Test Data Builders

**Objetivo:** Criar objetos de teste de forma fluente e reutilizável

```csharp
public class TaskItemBuilder
{
    private string _title = "Default Title";
    private string _description = "Default Description";
    private Priority _priority = Priority.Medium;
    private DateOnly _dueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
    private int _userId = 1;
    private List<TaskLabel>? _labels = null;

    public TaskItemBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public TaskItemBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TaskItemBuilder WithPriority(Priority priority)
    {
        _priority = priority;
        return this;
    }

    public TaskItemBuilder WithDueDate(DateOnly dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public TaskItemBuilder WithUserId(int userId)
    {
        _userId = userId;
        return this;
    }

    public TaskItemBuilder WithLabels(params TaskLabel[] labels)
    {
        _labels = labels.ToList();
        return this;
    }

    public TaskItem Build()
    {
        return new TaskItem(_title, _description, _priority, _dueDate, _userId, _labels);
    }

    public static TaskItemBuilder Default() => new();
}

// Uso:
var taskItem = TaskItemBuilder.Default()
    .WithTitle("Custom Title")
    .WithPriority(Priority.Urgent)
    .Build();
```

---

## Guia de Testes de Integração

### Princípios

1. **Infraestrutura Real:** Usar PostgreSQL e RabbitMQ reais (via containers)
2. **Isolamento de Dados:** Limpar banco entre testes
3. **Fixtures:** Reutilizar setup de containers
4. **Performance:** Otimizar para execução rápida

### Testcontainers Setup

```csharp
public class DatabaseFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;
    public ApplicationDbContext DbContext { get; private set; } = null!;
    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        // Criar container PostgreSQL
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("taskmanager_test")
            .WithUsername("test")
            .WithPassword("test123")
            .WithCleanUp(true)
            .Build();

        await _postgresContainer.StartAsync();

        ConnectionString = _postgresContainer.GetConnectionString();

        // Configurar DbContext
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        DbContext = new ApplicationDbContext(options);

        // Executar migrations
        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        if (_postgresContainer != null)
        {
            await _postgresContainer.DisposeAsync();
        }
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
public class TaskItemRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly TaskItemRepository _repository;

    public TaskItemRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new TaskItemRepository(_fixture.DbContext);
    }

    [Fact]
    public async Task Add_WithValidEntity_SavesToDatabase()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        var taskItem = new TaskItem(
            "Integration Test Task",
            "Test Description",
            Priority.High,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            1,
            null
        );

        // Act
        await _repository.Add(taskItem);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var saved = await _repository.GetById(taskItem.Id, CancellationToken.None);
        saved.Should().NotBeNull();
        saved!.Title.Should().Be(taskItem.Title);
        saved.Description.Should().Be(taskItem.Description);
    }

    [Fact]
    public async Task FindByFilter_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        // Criar 15 itens
        var items = Enumerable.Range(1, 15)
            .Select(i => new TaskItem($"Task {i}", "Description", Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)), 1, null))
            .ToList();

        _fixture.DbContext.TaskItems.AddRange(items);
        await _fixture.DbContext.SaveChangesAsync();

        var filter = new TaskItemQueryFilter
        {
            UserId = 1,
            Page = 2,
            PageSize = 5
        };

        // Act
        var (results, total) = await _repository.FindByFilter(filter, CancellationToken.None);

        // Assert
        results.Should().HaveCount(5);
        total.Should().Be(15);
        results.First().Title.Should().Be("Task 6"); // Segunda página
    }
}
```

### Test Data Factory com Bogus

```csharp
public static class TestDataFactory
{
    private static readonly Faker<TaskItem> _taskItemFaker = new Faker<TaskItem>()
        .CustomInstantiator(f => new TaskItem(
            f.Lorem.Sentence(3, 5),
            f.Lorem.Paragraph(),
            f.PickRandom<Priority>(),
            DateOnly.FromDateTime(f.Date.Future()),
            f.Random.Int(1, 100),
            null
        ));

    private static readonly Faker<TaskLabel> _taskLabelFaker = new Faker<TaskLabel>()
        .CustomInstantiator(f => new TaskLabel(
            f.Commerce.Categories(1).First(),
            f.PickRandom<LabelColor>(),
            f.Random.Int(1, 100)
        ));

    public static TaskItem CreateTaskItem() => _taskItemFaker.Generate();

    public static List<TaskItem> CreateTaskItems(int count) => _taskItemFaker.Generate(count);

    public static TaskLabel CreateTaskLabel() => _taskLabelFaker.Generate();

    public static List<TaskLabel> CreateTaskLabels(int count) => _taskLabelFaker.Generate(count);
}

// Uso:
var tasks = TestDataFactory.CreateTaskItems(100);
```

---

## Guia de Testes E2E

### Princípios

1. **Black Box:** Testar através da API HTTP
2. **Ambiente Real:** Containers para todas as dependências
3. **Scenarios:** Focar em jornadas completas de usuário
4. **Autenticação:** Incluir JWT nos testes

### WebApplicationFactory Setup

```csharp
public class WebApplicationFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;
    private RabbitMqContainer? _rabbitMqContainer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover DbContext existente
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
            );

            if (descriptor != null)
                services.Remove(descriptor);

            // Adicionar DbContext com container de teste
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_postgresContainer!.GetConnectionString());
            });

            // Substituir configuração RabbitMQ
            services.Configure<RabbitMqOptions>(options =>
            {
                options.ConnectionString = _rabbitMqContainer!.GetConnectionString();
            });
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("taskmanager_e2e")
            .Build();

        _rabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:3-management")
            .Build();

        await _postgresContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_postgresContainer != null)
            await _postgresContainer.DisposeAsync();

        if (_rabbitMqContainer != null)
            await _rabbitMqContainer.DisposeAsync();
    }
}
```

### Testes de Controller

```csharp
public class TaskItemsControllerTests : IClassFixture<WebApplicationFixture>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFixture _factory;

    public TaskItemsControllerTests(WebApplicationFixture factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        // Setup autenticação
        var token = GenerateTestJwtToken();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task POST_CreateTaskItem_ReturnsCreatedResponse()
    {
        // Arrange
        var request = new CreateTaskItemCommand(
            "E2E Test Task",
            "Test Description",
            Priority.High,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            1
        );

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await _client.PostAsync("/api/taskitems", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CreateTaskItemResult>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        result.Should().NotBeNull();
        result!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GET_GetTaskItemsByFilter_WithPagination_ReturnsCorrectPage()
    {
        // Arrange - Criar múltiplas tarefas primeiro
        for (int i = 1; i <= 10; i++)
        {
            var createRequest = new CreateTaskItemCommand(
                $"Task {i}",
                "Description",
                Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)),
                1
            );

            await _client.PostAsync("/api/taskitems",
                new StringContent(JsonSerializer.Serialize(createRequest),
                    Encoding.UTF8, "application/json"));
        }

        // Act
        var response = await _client.GetAsync(
            "/api/taskitems/search?userId=1&page=2&pageSize=3"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetTaskItemsByFilterResult>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(3);
        result.Pagination.CurrentPage.Should().Be(2);
        result.Pagination.TotalItems.Should().BeGreaterOrEqualTo(10);
    }

    private string GenerateTestJwtToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("SuperSecretKeyForJWTAuthentication2024!@#$%");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Test User")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "LifeSyncAPI",
            Audience = "LifeSyncApp",
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
```

### Testes de Scenarios

```csharp
public class TaskLifecycleTests : IClassFixture<WebApplicationFixture>
{
    private readonly HttpClient _client;

    public TaskLifecycleTests(WebApplicationFixture factory)
    {
        _client = factory.CreateClient();
        var token = GenerateTestJwtToken();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CompleteTaskLifecycle_CreateUpdateAddLabelComplete()
    {
        // 1. Criar tarefa
        var createRequest = new CreateTaskItemCommand(
            "Lifecycle Test Task",
            "Test Description",
            Priority.Medium,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            1
        );

        var createResponse = await _client.PostAsync(
            "/api/taskitems",
            new StringContent(JsonSerializer.Serialize(createRequest),
                Encoding.UTF8, "application/json")
        );

        createResponse.EnsureSuccessStatusCode();
        var createResult = await DeserializeResponse<CreateTaskItemResult>(createResponse);
        var taskId = createResult.Id;

        // 2. Criar etiqueta
        var labelRequest = new CreateTaskLabelCommand("Important", LabelColor.Red, 1);
        var labelResponse = await _client.PostAsync(
            "/api/tasklabels",
            new StringContent(JsonSerializer.Serialize(labelRequest),
                Encoding.UTF8, "application/json")
        );

        var labelResult = await DeserializeResponse<CreateTaskLabelResult>(labelResponse);

        // 3. Adicionar etiqueta à tarefa
        var addLabelRequest = new AddLabelCommand(taskId, new List<int> { labelResult.Id });
        var addLabelResponse = await _client.PostAsync(
            $"/api/taskitems/{taskId}/addLabels",
            new StringContent(JsonSerializer.Serialize(addLabelRequest),
                Encoding.UTF8, "application/json")
        );

        addLabelResponse.EnsureSuccessStatusCode();

        // 4. Atualizar status para Em Progresso
        var updateRequest = new UpdateTaskItemCommand(
            taskId,
            "Lifecycle Test Task",
            "Test Description",
            Status.InProgress,
            Priority.Medium,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
        );

        var updateResponse = await _client.PutAsync(
            $"/api/taskitems/{taskId}",
            new StringContent(JsonSerializer.Serialize(updateRequest),
                Encoding.UTF8, "application/json")
        );

        updateResponse.EnsureSuccessStatusCode();

        // 5. Verificar estado final
        var getResponse = await _client.GetAsync($"/api/taskitems/{taskId}");
        var finalState = await DeserializeResponse<TaskItemDTO>(getResponse);

        // Assertions
        finalState.Should().NotBeNull();
        finalState.Status.Should().Be(Status.InProgress);
        finalState.Labels.Should().ContainSingle();
        finalState.Labels.First().Name.Should().Be("Important");
    }

    private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }
}
```

---

## Estratégia de Execução

### Execução Paralela

xUnit suporta execução paralela por padrão. Configure em `xunit.runner.json`:

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

Use Traits para organizar:

```csharp
[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class TaskItemTests { }

[Trait("Category", "Integration")]
[Trait("Layer", "Infrastructure")]
public class TaskItemRepositoryTests { }

[Trait("Category", "E2E")]
[Trait("Layer", "API")]
public class TaskItemsControllerTests { }
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

Exemplo de workflow GitHub Actions:

```yaml
name: Tests

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
| **Domain** | 95%+ | Contém lógica de negócio crítica |
| **Application - Handlers** | 90%+ | Orquestração importante de casos de uso |
| **Application - Validators** | 95%+ | Validação de entrada crítica |
| **Application - Services** | 85%+ | Lógica de negócio e orquestração |
| **Infrastructure - Repositories** | 80%+ | Acesso a dados (testado via integração) |
| **API - Controllers** | 75%+ | Camada de entrada (testada via E2E) |
| **TOTAL** | **85%+** | Cobertura geral saudável |

### Configuração de Cobertura

Adicionar ao `.csproj` de cada projeto de teste:

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
open TestResults/CoverageReport/index.html   # macOS
```

### Métricas de Qualidade

- **Test Pass Rate:** 100% (todos os testes devem passar)
- **Code Coverage:** 85%+ no total
- **Performance:**
  - Testes unitários: < 1 segundo total
  - Testes de integração: < 30 segundos total
  - Testes E2E: < 2 minutos total
- **Manutenibilidade:** Complexity < 10 por método

---

## Exemplos de Código

### Exemplo Completo: Teste de Domínio

```csharp
using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using Xunit;

namespace TaskManager.UnitTests.Domain.Entities;

[Trait("Category", "Unit")]
[Trait("Layer", "Domain")]
public class TaskLabelTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateEntity()
    {
        // Arrange
        var name = "Work";
        var color = LabelColor.Blue;
        var userId = 1;

        // Act
        var label = new TaskLabel(name, color, userId);

        // Assert
        label.Should().NotBeNull();
        label.Name.Should().Be(name);
        label.LabelColor.Should().Be(color);
        label.UserId.Should().Be(userId);
        label.Items.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithInvalidName_ShouldThrowDomainException(string? invalidName)
    {
        // Arrange & Act
        var action = () => new TaskLabel(invalidName!, LabelColor.Blue, 1);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("*nome*obrigatório*");
    }

    [Fact]
    public void AddTaskItem_WithValidItem_ShouldAddToCollection()
    {
        // Arrange
        var label = new TaskLabel("Work", LabelColor.Blue, 1);
        var taskItem = TaskItemBuilder.Default().Build();

        // Act
        label.AddTaskItem(taskItem);

        // Assert
        label.Items.Should().ContainSingle();
        label.Items.First().Should().Be(taskItem);
    }

    [Fact]
    public void AddTaskItem_WithDuplicateItem_ShouldThrowDomainException()
    {
        // Arrange
        var label = new TaskLabel("Work", LabelColor.Blue, 1);
        var taskItem = TaskItemBuilder.Default().Build();
        label.AddTaskItem(taskItem);

        // Act
        var action = () => label.AddTaskItem(taskItem);

        // Assert
        action.Should().Throw<DomainException>()
            .WithMessage("*já está associada*");
    }

    [Fact]
    public void RemoveTaskItem_WithExistingItem_ShouldRemoveFromCollection()
    {
        // Arrange
        var label = new TaskLabel("Work", LabelColor.Blue, 1);
        var taskItem = TaskItemBuilder.Default().Build();
        label.AddTaskItem(taskItem);

        // Act
        label.RemoveTaskItem(taskItem);

        // Assert
        label.Items.Should().BeEmpty();
    }
}
```

### Exemplo Completo: Teste de Repository (Integração)

```csharp
using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Repositories;
using Xunit;

namespace TaskManager.IntegrationTests.Repositories;

[Trait("Category", "Integration")]
[Trait("Layer", "Infrastructure")]
public class TaskLabelRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly TaskLabelRepository _repository;

    public TaskLabelRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new TaskLabelRepository(_fixture.DbContext);
    }

    [Fact]
    public async Task Add_WithValidEntity_SavesToDatabase()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        var label = new TaskLabel("Integration Test", LabelColor.Green, 1);

        // Act
        await _repository.Add(label);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var saved = await _repository.GetById(label.Id, CancellationToken.None);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Integration Test");
        saved.LabelColor.Should().Be(LabelColor.Green);
    }

    [Fact]
    public async Task FindByFilter_WithUserId_ReturnsMatchingLabels()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        var label1 = new TaskLabel("User1 Label", LabelColor.Blue, 1);
        var label2 = new TaskLabel("User2 Label", LabelColor.Red, 2);
        var label3 = new TaskLabel("User1 Label2", LabelColor.Green, 1);

        _fixture.DbContext.TaskLabels.AddRange(label1, label2, label3);
        await _fixture.DbContext.SaveChangesAsync();

        var filter = new TaskLabelQueryFilter { UserId = 1 };

        // Act
        var (results, total) = await _repository.FindByFilter(filter, CancellationToken.None);

        // Assert
        results.Should().HaveCount(2);
        results.Should().AllSatisfy(l => l.UserId.Should().Be(1));
        total.Should().Be(2);
    }
}
```

### Exemplo Completo: Teste E2E de Scenario

```csharp
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace TaskManager.E2ETests.Scenarios;

[Trait("Category", "E2E")]
public class LabelManagementTests : IClassFixture<WebApplicationFixture>
{
    private readonly HttpClient _client;

    public LabelManagementTests(WebApplicationFixture factory)
    {
        _client = factory.CreateClient();
        var token = TestAuthHelper.GenerateJwtToken(userId: 1);
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CreateLabel_AttachToMultipleTasks_Success()
    {
        // 1. Criar etiqueta
        var createLabelRequest = new CreateTaskLabelCommand(
            "Important",
            LabelColor.Red,
            1
        );

        var labelResponse = await _client.PostAsync(
            "/api/tasklabels",
            SerializeContent(createLabelRequest)
        );

        labelResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var label = await DeserializeResponse<CreateTaskLabelResult>(labelResponse);

        // 2. Criar 3 tarefas
        var taskIds = new List<int>();
        for (int i = 1; i <= 3; i++)
        {
            var createTaskRequest = new CreateTaskItemCommand(
                $"Task {i}",
                $"Description {i}",
                Priority.Medium,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)),
                1
            );

            var taskResponse = await _client.PostAsync(
                "/api/taskitems",
                SerializeContent(createTaskRequest)
            );

            var task = await DeserializeResponse<CreateTaskItemResult>(taskResponse);
            taskIds.Add(task.Id);
        }

        // 3. Adicionar etiqueta a todas as 3 tarefas
        foreach (var taskId in taskIds)
        {
            var addLabelRequest = new AddLabelCommand(
                taskId,
                new List<int> { label.Id }
            );

            var addResponse = await _client.PostAsync(
                $"/api/taskitems/{taskId}/addLabels",
                SerializeContent(addLabelRequest)
            );

            addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // 4. Verificar que todas as tarefas têm a etiqueta
        foreach (var taskId in taskIds)
        {
            var getResponse = await _client.GetAsync($"/api/taskitems/{taskId}");
            var task = await DeserializeResponse<TaskItemDTO>(getResponse);

            task.Labels.Should().ContainSingle();
            task.Labels.First().Id.Should().Be(label.Id);
            task.Labels.First().Name.Should().Be("Important");
        }

        // 5. Buscar etiqueta e verificar que ela está associada a 3 tarefas
        var getLabelResponse = await _client.GetAsync($"/api/tasklabels/{label.Id}");
        var labelWithItems = await DeserializeResponse<TaskLabelDTO>(getLabelResponse);

        labelWithItems.Items.Should().HaveCount(3);
    }

    private StringContent SerializeContent<T>(T obj)
    {
        return new StringContent(
            JsonSerializer.Serialize(obj),
            Encoding.UTF8,
            "application/json"
        );
    }

    private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        )!;
    }
}
```

---

## Resumo

### Checklist de Implementação

- [ ] **Fase 1: Testes Unitários**
  - [ ] Adicionar pacotes NuGet (FluentAssertions, AutoFixture, Bogus)
  - [ ] Criar Test Data Builders
  - [ ] Implementar testes de domínio para TaskLabel
  - [ ] Implementar testes de Specifications
  - [ ] Implementar testes de Validators (5 novos)
  - [ ] Implementar testes de Command Handlers (8)
  - [ ] Implementar testes de Query Handlers (8)
  - [ ] Implementar testes de Mappers (2)
  - [ ] Implementar testes de TaskLabelService

- [ ] **Fase 2: Testes de Integração**
  - [ ] Criar novo projeto TaskManager.IntegrationTests
  - [ ] Configurar Testcontainers (PostgreSQL, RabbitMQ)
  - [ ] Implementar DatabaseFixture e RabbitMqFixture
  - [ ] Implementar testes de Repositories (2)
  - [ ] Implementar testes de Services com DB real (2)
  - [ ] Implementar testes de Handlers com pipeline completo (2)
  - [ ] Implementar testes de Background Services
  - [ ] Implementar testes de Events

- [ ] **Fase 3: Testes E2E**
  - [ ] Criar novo projeto TaskManager.E2ETests
  - [ ] Configurar WebApplicationFactory
  - [ ] Implementar TestAuthHelper
  - [ ] Implementar testes de Controllers (2)
  - [ ] Implementar testes de Scenarios (3)

- [ ] **Fase 4: Documentação e Validação**
  - [ ] Validar documentação (este arquivo)
  - [ ] Executar todos os testes
  - [ ] Gerar relatório de cobertura
  - [ ] Validar metas de cobertura

### Resultados Esperados

- **330+ testes** cobrindo todas as camadas
- **85%+ de cobertura de código**
- **100% de test pass rate**
- **CI/CD** pronto para execução automatizada
- **Documentação completa** para novos desenvolvedores

---

**Criado em:** 2026-02-08
**Versão:** 1.0
**Projeto:** LifeSync - TaskManager Microservice
