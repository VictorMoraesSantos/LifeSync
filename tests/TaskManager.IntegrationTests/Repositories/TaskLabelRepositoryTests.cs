using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Persistence.Repositories;
using TaskManager.IntegrationTests.Fixtures;

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
    public async Task GetById_WithExistingLabel_ReturnsLabel()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        var label = new TaskLabel("Integration Test", LabelColor.Green, 1);
        _fixture.DbContext.TaskLabels.Add(label);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetById(label.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Integration Test");
        result.LabelColor.Should().Be(LabelColor.Green);
    }

    [Fact]
    public async Task GetById_WithNonExistingLabel_ReturnsNull()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        // Act
        var result = await _repository.GetById(999, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAll_ReturnsAllLabels()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        var labels = new List<TaskLabel>
        {
            new TaskLabel("Work", LabelColor.Blue, 1),
            new TaskLabel("Personal", LabelColor.Green, 1),
            new TaskLabel("Urgent", LabelColor.Red, 1)
        };

        _fixture.DbContext.TaskLabels.AddRange(labels);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAll(CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Create_WithValidLabel_SavesToDatabase()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        var label = new TaskLabel("New Label", LabelColor.Purple, 1);

        // Act
        await _repository.Create(label);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var saved = await _repository.GetById(label.Id, CancellationToken.None);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("New Label");
    }

    [Fact]
    public async Task Delete_ExistingLabel_MarksAsDeleted()
    {
        // Arrange
        await _fixture.ResetDatabaseAsync();

        var label = new TaskLabel("To Delete", LabelColor.Yellow, 1);
        _fixture.DbContext.TaskLabels.Add(label);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        _repository.Delete(label);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        label.IsDeleted.Should().BeTrue();
    }
}
