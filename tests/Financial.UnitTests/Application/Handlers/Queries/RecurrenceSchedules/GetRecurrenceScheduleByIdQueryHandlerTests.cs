using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.Features.RecurrenceSchedules.Queries.GetById;
using Financial.Domain.Enums;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Queries.RecurrenceSchedules
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class GetRecurrenceScheduleByIdQueryHandlerTests
    {
        private readonly Mock<IRecurrenceScheduleService> _serviceMock;
        private readonly GetRecurrenceScheduleByIdQueryHandler _handler;

        public GetRecurrenceScheduleByIdQueryHandlerTests()
        {
            _serviceMock = new Mock<IRecurrenceScheduleService>();
            _handler = new GetRecurrenceScheduleByIdQueryHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithExistingId_ShouldReturnSchedule()
        {
            // Arrange
            var query = new GetRecurrenceScheduleByIdQuery(1);
            var scheduleDto = new RecurrenceScheduleDTO(
                Id: 1,
                TransactionId: 1,
                transaction: null!,
                CreatedAt: DateTime.UtcNow,
                UpdatedAt: null,
                Frequency: RecurrenceFrequency.Monthly,
                StartDate: DateTime.UtcNow,
                EndDate: DateTime.UtcNow.AddMonths(6),
                NextOccurrence: DateTime.UtcNow.AddMonths(1),
                MaxOccurrences: 10,
                OccurrencesGenerated: 0,
                IsActive: true);

            _serviceMock
                .Setup(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success<RecurrenceScheduleDTO?>(scheduleDto));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Schedule.Should().NotBeNull();
            result.Value.Schedule.Id.Should().Be(1);
            _serviceMock.Verify(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingId_ShouldReturnFailure()
        {
            // Arrange
            var query = new GetRecurrenceScheduleByIdQuery(999);
            var error = Error.NotFound("Agendamento não encontrado");

            _serviceMock
                .Setup(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<RecurrenceScheduleDTO?>(error));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
