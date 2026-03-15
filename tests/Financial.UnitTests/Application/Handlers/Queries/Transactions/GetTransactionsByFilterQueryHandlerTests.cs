using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Transaction;
using Financial.Application.Features.Transactions.Queries.GetByFilter;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Queries.Transactions
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class GetTransactionsByFilterQueryHandlerTests
    {
        private readonly Mock<ITransactionService> _serviceMock;
        private readonly GetTransactionsByFilterQueryHandler _handler;

        public GetTransactionsByFilterQueryHandlerTests()
        {
            _serviceMock = new Mock<ITransactionService>();
            _handler = new GetTransactionsByFilterQueryHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidFilter_ShouldReturnItemsAndPagination()
        {
            // Arrange
            var filter = new TransactionFilterDTO(
                null, 1, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, 1, 10);

            var query = new GetTransactionsByFilterQuery(filter);

            var transactions = new List<TransactionDTO>
            {
                new TransactionDTO(1, 1, null!, DateTime.UtcNow, null,
                    PaymentMethod.Pix, TransactionType.Expense,
                    Money.Create(1000, Currency.BRL), "Test", DateTime.UtcNow)
            };

            var pagination = new PaginationData(1, 10, 1, 1);

            _serviceMock
                .Setup(x => x.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success<(IEnumerable<TransactionDTO> Items, PaginationData Pagination)>((transactions, pagination)));

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
            var filter = new TransactionFilterDTO(
                null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, 1, 10);
            var query = new GetTransactionsByFilterQuery(filter);
            var error = Error.Failure("Erro ao buscar transacoes");

            _serviceMock
                .Setup(x => x.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<(IEnumerable<TransactionDTO> Items, PaginationData Pagination)>(error));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
