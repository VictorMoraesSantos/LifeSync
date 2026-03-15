using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Transaction;
using Financial.Application.Features.Transactions.Queries.GetById;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Queries.Transactions
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class GetTransactionByIdQueryHandlerTests
    {
        private readonly Mock<ITransactionService> _serviceMock;
        private readonly GetTransactionByIdQueryHandler _handler;

        public GetTransactionByIdQueryHandlerTests()
        {
            _serviceMock = new Mock<ITransactionService>();
            _handler = new GetTransactionByIdQueryHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithExistingId_ShouldReturnTransaction()
        {
            // Arrange
            var query = new GetTransactionByIdQuery(1);
            var transactionDto = new TransactionDTO(
                Id: 1,
                UserId: 1,
                Category: null!,
                CreatedAt: DateTime.UtcNow,
                UpdatedAt: null,
                PaymentMethod: PaymentMethod.Pix,
                TransactionType: TransactionType.Expense,
                Amount: Money.Create(1000, Currency.BRL),
                Description: "Test transaction",
                TransactionDate: DateTime.UtcNow);

            _serviceMock
                .Setup(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(transactionDto));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Transaction.Should().NotBeNull();
            result.Value.Transaction.Id.Should().Be(1);
            _serviceMock.Verify(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            var query = new GetTransactionByIdQuery(999);
            var error = Error.NotFound("Transacao nao encontrada");

            _serviceMock
                .Setup(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<TransactionDTO>(error));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
