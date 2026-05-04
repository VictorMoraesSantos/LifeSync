using Financial.Application.DTOs.Category;
using Financial.Domain.Errors;
using Financial.Infrastructure.Persistence.Repositories;
using Financial.Infrastructure.Services;
using Financial.IntegrationTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Financial.IntegrationTests.Services
{
    [Trait("Category", "Integration")]
    [Trait("Layer", "Infrastructure")]
    public class CategoryServiceIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;

        public CategoryServiceIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private CategoryService CreateService()
        {
            var context = _fixture.CreateNewContext();
            var repo = new CategoryRepository(context);
            return new CategoryService(repo, NullLogger<CategoryService>.Instance);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateCategory()
        {
            // Arrange
            var service = CreateService();
            var dto = new CreateCategoryDTO(1, "Test Category", "Test Description");

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CreateAsync_WithNullDTO_ShouldFail()
        {
            // Act
            var service = CreateService();
            var result = await service.CreateAsync(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingCategory_ShouldReturnDTO()
        {
            // Arrange
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateCategoryDTO(1, "Lookup Category", "Desc"));

            // Act
            var service2 = CreateService();
            var result = await service2.GetByIdAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be("Lookup Category");
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var service = CreateService();
            var result = await service.GetByIdAsync(0);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(CategoryErrors.InvalidId.Description);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateCategory()
        {
            // Arrange
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateCategoryDTO(1, "Old Name", "Old Desc"));

            var updateDto = new UpdateCategoryDTO(createResult.Value, "New Name", "New Desc");

            // Act
            var service2 = CreateService();
            var result = await service2.UpdateAsync(updateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var service3 = CreateService();
            var updated = await service3.GetByIdAsync(createResult.Value);
            updated.Value.Name.Should().Be("New Name");
            updated.Value.Description.Should().Be("New Desc");
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithExistingCategory_ShouldSucceed()
        {
            // Arrange
            var service = CreateService();
            var createResult = await service.CreateAsync(
                new CreateCategoryDTO(1, "To Delete", "Desc"));

            // Act
            var service2 = CreateService();
            var result = await service2.DeleteAsync(createResult.Value);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ShouldFail()
        {
            // Act
            var service = CreateService();
            var result = await service.DeleteAsync(99999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetByUserIdAsync Tests

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnCategoriesForUser()
        {
            // Arrange
            var service = CreateService();
            await service.CreateAsync(new CreateCategoryDTO(1, "Cat 1", null));
            await service.CreateAsync(new CreateCategoryDTO(1, "Cat 2", null));
            await service.CreateAsync(new CreateCategoryDTO(2, "Cat 3", null));

            // Act
            var service2 = CreateService();
            var result = await service2.GetByUserIdAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }

        #endregion

        #region GetByFilterAsync Tests

        [Fact]
        public async Task GetByFilterAsync_WithNameFilter_ShouldReturnFilteredResults()
        {
            // Arrange
            var service = CreateService();
            await service.CreateAsync(new CreateCategoryDTO(1, "Alimentacao", "Comida"));
            await service.CreateAsync(new CreateCategoryDTO(1, "Transporte", "Onibus"));

            var filter = new CategoryFilterDTO(
                null, 1, "Aliment", null, null, null, null, null, null, 1, 10);

            // Act
            var service2 = CreateService();
            var result = await service2.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().NotBeEmpty();
            result.Value.Pagination.Should().NotBeNull();
        }

        #endregion
    }
}
