using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.Features.RecurrenceSchedules.Queries.GetByUserId;
using Financial.Domain.Enums;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Queries.RecurrenceSchedules
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class GetRecurrenceScheduleByUserIdQueryHandlerTests
    {
        private readonly Mock<IRecurrenceScheduleService> _serviceMock;
        private readonly GetRecurrenceScheduleByUserIdQueryHandler _handler;

        public GetRecurrenceScheduleByUserIdQueryHandlerTests()
        {
            _serviceMock = new Mock<IRecurrenceScheduleService>();
            _handler = new GetRecurrenceScheduleByUserIdQueryHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidUserId_ShouldReturnSchedules()
        {
            // Arrange
            var query = new GetRecurrenceScheduleByUserIdQuery(1);
            var schedules = new List<RecurrenceScheduleDTO>
            {
                new RecurrenceScheduleDTO(
                    Id: 1, TransactionId: 1, transaction: null!,
                    CreatedAt: DateTime.UtcNow, UpdatedAt: null,
                    Frequency: RecurrenceFrequency.Monthly,
                    StartDate: DateTime.UtcNow, EndDate: DateTime.UtcNow.AddMonths(6),
                    NextOccurrence: DateTime.UtcNow.AddMonths(1),
                    MaxOccurrences: 10, OccurrencesGenerated: 0, IsActive: true),
                new RecurrenceScheduleDTO(
                    Id: 2, TransactionId: 2, transaction: null!,
                    CreatedAt: DateTime.UtcNow, UpdatedAt: null,
                    Frequency: RecurrenceFrequency.Weekly,
                    StartDate: DateTime.UtcNow, EndDate: DateTime.UtcNow.AddMonths(3),
                    NextOccurrence: DateTime.UtcNow.AddDays(7),
                    MaxOccurrences: null, OccurrencesGenerated: 0, IsActive: true)
            };

            _serviceMock
                .Setup(x => x.GetActiveByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success<IEnumerable<RecurrenceScheduleDTO>>(schedules));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Schedules.Should().HaveCount(2);
            _serviceMock.Verify(x => x.GetActiveByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenServiceFails_ShouldReturnFailure()
        {
            // Arrange
            var query = new GetRecurrenceScheduleByUserIdQuery(999);
            var error = Error.Failure("Erro ao buscar agendamentos");

            _serviceMock
                .Setup(x => x.GetActiveByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<IEnumerable<RecurrenceScheduleDTO>>(error));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
