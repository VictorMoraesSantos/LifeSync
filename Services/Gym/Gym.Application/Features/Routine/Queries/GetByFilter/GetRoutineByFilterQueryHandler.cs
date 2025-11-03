using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.Routine.Queries.GetByFilter
{
    public class GetRoutineByFilterQueryHandler : IQueryHandler<GetRoutineByFilterQuery, GetRoutineByFilterResult>
    {
        private readonly IRoutineService _routineService;

        public GetRoutineByFilterQueryHandler(IRoutineService routineService)
        {
            _routineService = routineService;
        }

        public async Task<Result<GetRoutineByFilterResult>> Handle(GetRoutineByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _routineService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetRoutineByFilterResult>(result.Error);

            return Result.Success(new GetRoutineByFilterResult(result.Value.Items, result.Value.Pagination));
        }
    }
}
