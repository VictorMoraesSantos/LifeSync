using BuildingBlocks.CQRS.Requests.Queries;
using Financial.Application.DTOs.RecurrenceSchedule;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetById
{
    public record GetRecurrenceScheduleByIdQuery(int Id) : IQuery<GetRecurrenceScheduleByIdResult>;
    public record GetRecurrenceScheduleByIdResult(RecurrenceScheduleDTO Schedule);
}
