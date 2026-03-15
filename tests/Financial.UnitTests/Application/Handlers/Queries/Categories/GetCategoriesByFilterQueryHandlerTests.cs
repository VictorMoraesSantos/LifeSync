using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Category;
using Financial.Application.Features.Categories.Queries.GetByFilter;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Queries.Categories
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class GetCategoriesByFilterQueryHandlerTests
    {
        private readonly Mock<ICategoryService> _serviceMock;
        private readonly GetCategoriesByFilterQueryHandler _handler;

        public GetCategoriesByFilterQueryHandlerTests()
        {
            _serviceMock = new Mock<ICategoryService>();
            _handler = new GetCategoriesByFilterQueryHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidFilter_ShouldReturnItemsAndPagination()
        {
            // Arrange
            var filter = new CategoryFilterDTO(
                Id: null,
                UserId: 1,
                NameContains: null,
                DescriptionContains: null,
                CreatedAt: null,
                UpdatedAt: null,
                IsDeleted: null,
                SortBy: null,
                SortDesc: null,
                Page: 1,
                PageSize: 10);

            var query = new GetCategoriesByFilterQuery(filter);

            var categories = new List<CategoryDTO>
            {
                new CategoryDTO(1, 1, DateTime.UtcNow, null, "Category 1", "Desc 1")
            };

            var pagination = new PaginationData(1, 10, 1, 1);

            _serviceMock
                .Setup(x => x.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success<(IEnumerable<CategoryDTO> Items, PaginationData Pagination)>((categories, pagination)));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
            result.Value.Pagination.Should().NotBeNull();
            _serviceMock.Verify(x => x.GetByFilterAsync(filter, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenServiceFails_ShouldReturnFailure()
        {
            // Arrange
            var filter = new CategoryFilterDTO(null, null, null, null, null, null, null, null, null, 1, 10);
            var query = new GetCategoriesByFilterQuery(filter);
            var error = Error.Failure("Erro ao buscar categorias");

            _serviceMock
                .Setup(x => x.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<(IEnumerable<CategoryDTO> Items, PaginationData Pagination)>(error));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
