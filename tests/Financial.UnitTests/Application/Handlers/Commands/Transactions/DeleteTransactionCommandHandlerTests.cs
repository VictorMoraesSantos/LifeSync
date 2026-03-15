using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.Features.Transactions.Commands.Delete;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Commands.Transactions
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class DeleteTransactionCommandHandlerTests
    {
        private readonly Mock<ITransactionService> _serviceMock;
        private readonly DeleteTransactionCommandHandler _handler;

        public DeleteTransactionCommandHandlerTests()
        {
            _serviceMock = new Mock<ITransactionService>();
            _handler = new DeleteTransactionCommandHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var command = new DeleteTransactionCommand(1);

            _serviceMock
                .Setup(x => x.DeleteAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.IsSuccess.Should().BeTrue();
            _serviceMock.Verify(x => x.DeleteAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenServiceFails_ShouldReturnFailure()
        {
            // Arrange
            var command = new DeleteTransactionCommand(999);
            var error = Error.NotFound("Transacao nao encontrada");

            _serviceMock
                .Setup(x => x.DeleteAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<bool>(error));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
