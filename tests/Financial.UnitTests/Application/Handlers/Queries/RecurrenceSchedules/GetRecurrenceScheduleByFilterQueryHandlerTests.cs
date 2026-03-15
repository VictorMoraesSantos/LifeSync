using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.Features.RecurrenceSchedules.Queries.GetByFilter;
using Financial.Domain.Enums;
using FluentAssertions;
using Moq;

namespace Financial.UnitTests.Application.Handlers.Queries.RecurrenceSchedules
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Application")]
    public class GetRecurrenceScheduleByFilterQueryHandlerTests
    {
        private readonly Mock<IRecurrenceScheduleService> _serviceMock;
        private readonly GetRecurrenceScheduleByFilterQueryHandler _handler;

        public GetRecurrenceScheduleByFilterQueryHandlerTests()
        {
            _serviceMock = new Mock<IRecurrenceScheduleService>();
            _handler = new GetRecurrenceScheduleByFilterQueryHandler(_serviceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidFilter_ShouldReturnItemsAndPagination()
        {
            // Arrange
            var filter = new RecurrenceScheduleFilterDTO(
                UserId: 1,
                IsActive: true,
                Page: 1,
                PageSize: 10);

            var query = new GetRecurrenceScheduleByFilterQuery(filter);

            var schedules = new List<RecurrenceScheduleDTO>
            {
                new RecurrenceScheduleDTO(
                    Id: 1, TransactionId: 1, transaction: null!,
                    CreatedAt: DateTime.UtcNow, UpdatedAt: null,
                    Frequency: RecurrenceFrequency.Monthly,
                    StartDate: DateTime.UtcNow, EndDate: DateTime.UtcNow.AddMonths(6),
                    NextOccurrence: DateTime.UtcNow.AddMonths(1),
                    MaxOccurrences: 10, OccurrencesGenerated: 0, IsActive: true)
            };

            var pagination = new PaginationData(1, 10, 1, 1);

            _serviceMock
                .Setup(x => x.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success<(IEnumerable<RecurrenceScheduleDTO> Items, PaginationData Pagination)>((schedules, pagination)));

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
            var filter = new RecurrenceScheduleFilterDTO();
            var query = new GetRecurrenceScheduleByFilterQuery(filter);
            var error = Error.Failure("Erro ao buscar agendamentos");

            _serviceMock
                .Setup(x => x.GetByFilterAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<(IEnumerable<RecurrenceScheduleDTO> Items, PaginationData Pagination)>(error));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }
    }
}
