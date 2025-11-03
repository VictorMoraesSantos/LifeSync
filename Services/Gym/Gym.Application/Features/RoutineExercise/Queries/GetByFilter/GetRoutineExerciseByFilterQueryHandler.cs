using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.RoutineExercise.Queries.GetByFilter
{
    public class GetRoutineExerciseByFilterQueryHandler : IQueryHandler<GetRoutineExerciseByFilterQuery, GetRoutineExerciseByFilterResult>
    {
        private readonly IRoutineExerciseService _routineExerciseService;

        public GetRoutineExerciseByFilterQueryHandler(IRoutineExerciseService routineExerciseService)
        {
            _routineExerciseService = routineExerciseService;
        }

        public async Task<Result<GetRoutineExerciseByFilterResult>> Handle(GetRoutineExerciseByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _routineExerciseService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetRoutineExerciseByFilterResult>(result.Error);

            return Result.Success(new GetRoutineExerciseByFilterResult(result.Value.Items, result.Value.Pagination));
        }
    }
}
