using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.DTOs.Transaction;
using Financial.Application.Features.Transactions.Commands.Create;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Commands.Transactions
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class CreateTransactionCommandHandlerTests
    {
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mock<IRecurrenceScheduleService> _recurrenceServiceMock;
        private readonly CreateTransactionCommandHandler _handler;

        public CreateTransactionCommandHandlerTests()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _recurrenceServiceMock = new Mock<IRecurrenceScheduleService>();
            _handler = new CreateTransactionCommandHandler(
                _transactionServiceMock.Object,
                _recurrenceServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnSuccess()
        {
            // Arrange
            var command = new CreateTransactionCommand(
                1, 5, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(1000, Currency.BRL), "Test", DateTime.UtcNow);

            _transactionServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateTransactionDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.TransactionId.Should().Be(1);
            _transactionServiceMock.Verify(x => x.CreateAsync(It.IsAny<CreateTransactionDTO>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithRecurringTransaction_ShouldCreateSchedule()
        {
            // Arrange
            var command = new CreateTransactionCommand(
                1, 5, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(1000, Currency.BRL), "Recurring", DateTime.UtcNow,
                IsRecurring: true, Frequency: RecurrenceFrequency.Monthly);

            _transactionServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateTransactionDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(1));

            _recurrenceServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateRecurrenceScheduleDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _recurrenceServiceMock.Verify(x => x.CreateAsync(It.IsAny<CreateRecurrenceScheduleDTO>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonRecurringTransaction_ShouldNotCreateSchedule()
        {
            // Arrange
            var command = new CreateTransactionCommand(
                1, 5, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(1000, Currency.BRL), "Normal", DateTime.UtcNow,
                IsRecurring: false);

            _transactionServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateTransactionDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _recurrenceServiceMock.Verify(x => x.CreateAsync(It.IsAny<CreateRecurrenceScheduleDTO>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenTransactionServiceFails_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateTransactionCommand(
                1, 5, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(1000, Currency.BRL), "Test", DateTime.UtcNow);
            var error = Error.Failure("Erro ao criar transacao");

            _transactionServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateTransactionDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<int>(error));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WhenRecurrenceServiceFails_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateTransactionCommand(
                1, 5, PaymentMethod.Pix, TransactionType.Expense,
                Money.Create(1000, Currency.BRL), "Recurring", DateTime.UtcNow,
                IsRecurring: true, Frequency: RecurrenceFrequency.Monthly);

            _transactionServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateTransactionDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(1));

            var error = Error.Failure("Erro ao criar agendamento");
            _recurrenceServiceMock
                .Setup(x => x.CreateAsync(It.IsAny<CreateRecurrenceScheduleDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<int>(error));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
