using BuildingBlocks.CQRS.Requests.Queries;
using BuildingBlocks.Results;
using Financial.Application.DTOs.RecurrenceSchedule;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetByFilter
{
    public record GetRecurrenceScheduleByFilterQuery(RecurrenceScheduleFilterDTO Filter) : IQuery<GetRecurrenceScheduleByFilterResult>;
    public record GetRecurrenceScheduleByFilterResult(IEnumerable<RecurrenceScheduleDTO> Items, PaginationData Pagination);
}
