using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Transaction;
using Financial.Application.Features.Transactions.Commands.Update;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Commands.Transactions
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class UpdateTransactionCommandHandlerTests
    {
        private readonly Mock<ITransactionService> _serviceMock;
        private readonly UpdateTransactionCommandHandler _handler;

        public UpdateTransactionCommandHandlerTests()
        {
            _serviceMock = new Mock<ITransactionService>();
            _handler = new UpdateTransactionCommandHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnSuccess()
        {
            // Arrange
            var command = new UpdateTransactionCommand(
                1, 5, PaymentMethod.DebitCard, TransactionType.Income,
                Money.Create(2000, Currency.BRL), "Updated", DateTime.UtcNow);
            UpdateTransactionDTO? capturedDto = null;

            _serviceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateTransactionDTO>(), It.IsAny<CancellationToken>()))
                .Callback<UpdateTransactionDTO, CancellationToken>((dto, _) => capturedDto = dto)
                .ReturnsAsync(Result.Success(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.IsSuccess.Should().BeTrue();
            capturedDto.Should().NotBeNull();
            capturedDto!.Id.Should().Be(command.Id);
            capturedDto.CategoryId.Should().Be(command.CategoryId);
            capturedDto.PaymentMethod.Should().Be(command.PaymentMethod);
            capturedDto.TransactionType.Should().Be(command.TransactionType);
            capturedDto.Amount.Should().Be(command.Amount);
            capturedDto.Description.Should().Be(command.Description);
            _serviceMock.Verify(x => x.UpdateAsync(It.IsAny<UpdateTransactionDTO>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenServiceFails_ShouldReturnFailure()
        {
            // Arrange
            var command = new UpdateTransactionCommand(
                999, null, PaymentMethod.Cash, TransactionType.Expense,
                Money.Create(100, Currency.BRL), "Test", DateTime.UtcNow);
            var error = Error.NotFound("Transacao nao encontrada");

            _serviceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateTransactionDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<bool>(error));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
