using BuildingBlocks.CQRS.Requests.Queries;
using Financial.Application.DTOs.RecurrenceSchedule;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetByUserId
{
    public record GetRecurrenceScheduleByUserIdQuery(int UserId) : IQuery<GetRecurrenceScheduleByUserIdResult>;
    public record GetRecurrenceScheduleByUserIdResult(IEnumerable<RecurrenceScheduleDTO> Schedules);

}
