# LifeSyncApp Tests

This project contains unit and integration tests for the LifeSync MAUI application.

## Running Tests

### Using .NET CLI

```bash
cd ClientApp/LifeSyncApp.Tests
dotnet test
```

### Using Visual Studio

1. Open the solution in Visual Studio
2. Open Test Explorer (Test > Test Explorer)
3. Click "Run All Tests"

### With Code Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Structure

```
LifeSyncApp.Tests/
├── ViewModels/
│   └── Financial/
│       ├── ManageTransactionViewModelTests.cs
│       ├── TransactionListViewModelTests.cs
│       └── FilterTransactionViewModelTests.cs
├── Services/
│   └── Financial/
│       └── TransactionServiceTests.cs
└── README.md
```

## Test Categories

Tests are categorized using xUnit traits:

- **Category**: `Unit` or `Integration`
- **Layer**: `ViewModel`, `Service`, `Converter`, etc.

Filter tests by category:

```bash
dotnet test --filter "Category=Unit"
dotnet test --filter "Layer=ViewModel"
```

## Writing New Tests

### ViewModel Tests

ViewModel tests use Moq to mock service dependencies:

```csharp
[Fact]
public void MyTest()
{
    // Arrange
    var mockService = new Mock<SomeService>();
    mockService.Setup(x => x.Method()).Returns(expectedValue);
    var viewModel = new MyViewModel(mockService.Object);

    // Act
    viewModel.SomeCommand.Execute(null);

    // Assert
    viewModel.SomeProperty.Should().Be(expectedValue);
    mockService.Verify(x => x.Method(), Times.Once);
}
```

### Service Tests

Service tests may require mocking HttpClient or using in-memory databases.

## TODO: Expand Test Coverage

The current test suite provides a foundation. Consider adding:

1. **ManageTransactionViewModel**: Tests for SaveCommand, validation, and error handling
2. **TransactionListViewModel**: Tests for filtering, grouping, and refresh
3. **FilterTransactionViewModel**: Tests for filter application and clearing
4. **TransactionDetailViewModel**: Tests for edit/delete commands
5. **Service Integration Tests**: End-to-end API testing with test server
6. **Converter Tests**: Value converter logic validation

## Dependencies

- **xUnit**: Testing framework
- **Moq**: Mocking library
- **FluentAssertions**: Assertion library for readable tests
- **coverlet.collector**: Code coverage collector

## Notes

- MAUI application testing has limitations - some components like Shell navigation
  and Application.Current require special test harnesses or integration testing
- Focus unit tests on business logic and data transformations
- Use integration tests for navigation and UI interactions
