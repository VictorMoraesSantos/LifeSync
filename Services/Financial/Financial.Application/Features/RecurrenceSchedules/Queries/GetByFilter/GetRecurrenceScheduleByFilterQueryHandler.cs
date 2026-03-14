using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetByFilter
{
    public class GetRecurrenceScheduleByFilterQueryHandler : IQueryHandler<GetRecurrenceScheduleByFilterQuery, GetRecurrenceScheduleByFilterResult>
    {
        private readonly IRecurrenceScheduleService _recurrenceScheduleService;

        public GetRecurrenceScheduleByFilterQueryHandler(IRecurrenceScheduleService recurrenceScheduleService)
        {
            _recurrenceScheduleService = recurrenceScheduleService;
        }

        public async Task<Result<GetRecurrenceScheduleByFilterResult>> Handle(GetRecurrenceScheduleByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _recurrenceScheduleService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetRecurrenceScheduleByFilterResult>(result.Error);

            return Result.Success(new GetRecurrenceScheduleByFilterResult(result.Value.Items, result.Value.Pagination));
        }
    }
}
