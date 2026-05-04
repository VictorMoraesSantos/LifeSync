using Financial.Application.DTOs.Category;
using Financial.Domain.Entities;
using Financial.Domain.Errors;
using Financial.Domain.Filters;
using Financial.Domain.Repositories;
using Financial.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Financial.UnitTests.Application
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _repoMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _repoMock = new Mock<ICategoryRepository>();
            _service = new CategoryService(
                _repoMock.Object,
                NullLogger<CategoryService>.Instance);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidDTO_ShouldReturnSuccess()
        {
            // Arrange
            var dto = new CreateCategoryDTO(1, "Test Category", "Description");

            _repoMock
                .Setup(x => x.Create(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repoMock.Verify(x => x.Create(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithNullDTO_ShouldReturnFailure()
        {
            // Act
            var result = await _service.CreateAsync(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetByIdAsync(0);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(CategoryErrors.InvalidId.Description);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnCategory()
        {
            // Arrange
            var category = new Category(1, "Test", "Desc");
            _repoMock
                .Setup(x => x.GetById(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be("Test");
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithNullDTO_ShouldReturnFailure()
        {
            // Act
            var result = await _service.UpdateAsync(null!);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Arrange
            var dto = new UpdateCategoryDTO(0, "Name", null);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(CategoryErrors.InvalidId.Description);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            var dto = new UpdateCategoryDTO(999, "Name", null);

            _repoMock
                .Setup(x => x.GetById(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithValidDTO_ShouldReturnSuccess()
        {
            // Arrange
            var category = new Category(1, "Old", "Old Desc");
            var dto = new UpdateCategoryDTO(1, "New", "New Desc");

            _repoMock
                .Setup(x => x.GetById(dto.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _repoMock
                .Setup(x => x.Update(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
            _repoMock.Verify(x => x.Update(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.DeleteAsync(0);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            _repoMock
                .Setup(x => x.GetById(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var category = new Category(1, "Test", "Desc");

            _repoMock
                .Setup(x => x.GetById(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _repoMock
                .Setup(x => x.Delete(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repoMock.Verify(x => x.Delete(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetByUserIdAsync Tests

        [Fact]
        public async Task GetByUserIdAsync_WithInvalidUserId_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetByUserIdAsync(0);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Description.Should().Be(CategoryErrors.InvalidUserId.Description);
        }

        [Fact]
        public async Task GetByUserIdAsync_WithValidUserId_ShouldReturnCategories()
        {
            // Arrange
            var categories = new List<Category?>
            {
                new Category(1, "Cat 1", "Desc 1"),
                new Category(1, "Cat 2", "Desc 2")
            };

            _repoMock
                .Setup(x => x.GetAllByUserId(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            // Act
            var result = await _service.GetByUserIdAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
        }

        #endregion

        #region GetByFilterAsync Tests

        [Fact]
        public async Task GetByFilterAsync_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            var filter = new CategoryFilterDTO(null, null, null, null, null, null, null, null, null, 1, 10);

            _repoMock
                .Setup(x => x.FindByFilter(It.IsAny<CategoryQueryFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Enumerable.Empty<Category>(), 0));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByFilterAsync_WithResults_ShouldReturnPaginatedData()
        {
            // Arrange
            var filter = new CategoryFilterDTO(null, 1, null, null, null, null, null, null, null, 1, 10);
            var categories = new List<Category> { new Category(1, "Cat", "Desc") };

            _repoMock
                .Setup(x => x.FindByFilter(It.IsAny<CategoryQueryFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((categories, 1));

            // Act
            var result = await _service.GetByFilterAsync(filter, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
            result.Value.Pagination.Should().NotBeNull();
        }

        #endregion
    }
}
