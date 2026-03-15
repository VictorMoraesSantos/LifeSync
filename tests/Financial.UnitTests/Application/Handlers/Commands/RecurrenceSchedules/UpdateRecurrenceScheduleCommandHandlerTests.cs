using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.Features.RecurrenceSchedules.Commands.Update;
using Financial.Domain.Enums;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Commands.RecurrenceSchedules
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class UpdateRecurrenceScheduleCommandHandlerTests
    {
        private readonly Mock<IRecurrenceScheduleService> _serviceMock;
        private readonly UpdateRecurrenceScheduleCommandHandler _handler;

        public UpdateRecurrenceScheduleCommandHandlerTests()
        {
            _serviceMock = new Mock<IRecurrenceScheduleService>();
            _handler = new UpdateRecurrenceScheduleCommandHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnSuccess()
        {
            // Arrange
            var command = new UpdateRecurrenceScheduleCommand(1, RecurrenceFrequency.Weekly, DateTime.UtcNow.AddMonths(6), 10);
            UpdateRecurrenceScheduleDTO? capturedDto = null;

            _serviceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateRecurrenceScheduleDTO>(), It.IsAny<CancellationToken>()))
                .Callback<UpdateRecurrenceScheduleDTO, CancellationToken>((dto, _) => capturedDto = dto)
                .ReturnsAsync(Result.Success(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.IsSuccess.Should().BeTrue();
            capturedDto.Should().NotBeNull();
            capturedDto!.Id.Should().Be(command.Id);
            capturedDto.Frequency.Should().Be(command.Frequency);
            capturedDto.EndDate.Should().Be(command.EndDate);
            capturedDto.MaxOccurrences.Should().Be(command.MaxOccurrences);
            _serviceMock.Verify(x => x.UpdateAsync(It.IsAny<UpdateRecurrenceScheduleDTO>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenServiceFails_ShouldReturnFailure()
        {
            // Arrange
            var command = new UpdateRecurrenceScheduleCommand(999, RecurrenceFrequency.Daily);
            var error = Error.NotFound("Agendamento não encontrado");

            _serviceMock
                .Setup(x => x.UpdateAsync(It.IsAny<UpdateRecurrenceScheduleDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<bool>(error));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
