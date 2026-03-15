using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Category;
using Financial.Application.Features.Categories.Queries.GetById;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Queries.Categories
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class GetCategoryByIdQueryHandlerTests
    {
        private readonly Mock<ICategoryService> _serviceMock;
        private readonly GetCategoryByIdQueryHandler _handler;

        public GetCategoryByIdQueryHandlerTests()
        {
            _serviceMock = new Mock<ICategoryService>();
            _handler = new GetCategoryByIdQueryHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithExistingId_ShouldReturnCategory()
        {
            // Arrange
            var query = new GetCategoryByIdQuery(1);
            var categoryDto = new CategoryDTO(
                Id: 1,
                UserId: 1,
                CreatedAt: DateTime.UtcNow,
                UpdatedAt: null,
                Name: "Test Category",
                Description: "Test description");

            _serviceMock
                .Setup(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(categoryDto));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Category.Should().NotBeNull();
            result.Value.Category.Id.Should().Be(1);
            result.Value.Category.Name.Should().Be("Test Category");
            _serviceMock.Verify(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            var query = new GetCategoryByIdQuery(999);
            var error = Error.NotFound("Categoria nao encontrada");

            _serviceMock
                .Setup(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<CategoryDTO>(error));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
