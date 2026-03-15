using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.Features.RecurrenceSchedules.Commands.Deactivate;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Commands.RecurrenceSchedules
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class DeactivateRecurrenceScheduleCommandHandlerTests
    {
        private readonly Mock<IRecurrenceScheduleService> _serviceMock;
        private readonly DeactivateRecurrenceScheduleCommandHandler _handler;

        public DeactivateRecurrenceScheduleCommandHandlerTests()
        {
            _serviceMock = new Mock<IRecurrenceScheduleService>();
            _handler = new DeactivateRecurrenceScheduleCommandHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var command = new DeactivateRecurrenceScheduleCommand(1);

            _serviceMock
                .Setup(x => x.DeactiveScheduleAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.IsSuccess.Should().BeTrue();
            _serviceMock.Verify(x => x.DeactiveScheduleAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenServiceFails_ShouldReturnFailure()
        {
            // Arrange
            var command = new DeactivateRecurrenceScheduleCommand(999);
            var error = Error.NotFound("Agendamento não encontrado");

            _serviceMock
                .Setup(x => x.DeactiveScheduleAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<bool>(error));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
