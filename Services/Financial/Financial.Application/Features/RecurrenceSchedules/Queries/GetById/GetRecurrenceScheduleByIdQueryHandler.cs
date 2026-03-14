using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetById
{
    public class GetRecurrenceScheduleByIdQueryHandler : IQueryHandler<GetRecurrenceScheduleByIdQuery, GetRecurrenceScheduleByIdResult>
    {
        private readonly IRecurrenceScheduleService _recurrenceScheduleService;

        public GetRecurrenceScheduleByIdQueryHandler(IRecurrenceScheduleService recurrenceScheduleService)
        {
            _recurrenceScheduleService = recurrenceScheduleService;
        }

        public async Task<Result<GetRecurrenceScheduleByIdResult>> Handle(GetRecurrenceScheduleByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _recurrenceScheduleService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetRecurrenceScheduleByIdResult>(result.Error!);

            return Result.Success(new GetRecurrenceScheduleByIdResult(result.Value!));
        }
    }
}
