using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.RecurrenceSchedules.Queries.GetByUserId
{
    public class GetRecurrenceScheduleByUserIdQueryHandler : IQueryHandler<GetRecurrenceScheduleByUserIdQuery, GetRecurrenceScheduleByUserIdResult>
    {
        private readonly IRecurrenceScheduleService _recurrenceScheduleService;

        public GetRecurrenceScheduleByUserIdQueryHandler(IRecurrenceScheduleService recurrenceScheduleService)
        {
            _recurrenceScheduleService = recurrenceScheduleService;
        }

        public async Task<Result<GetRecurrenceScheduleByUserIdResult>> Handle(GetRecurrenceScheduleByUserIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _recurrenceScheduleService.GetActiveByUserIdAsync(query.UserId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetRecurrenceScheduleByUserIdResult>(result.Error!);

            return Result.Success(new GetRecurrenceScheduleByUserIdResult(result.Value!));
        }
    }
}
