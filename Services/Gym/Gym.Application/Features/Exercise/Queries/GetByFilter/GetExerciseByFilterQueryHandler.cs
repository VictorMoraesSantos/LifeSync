using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.Exercise.Queries.GetByFilter
{
    public class GetExerciseByFilterQueryHandler : IQueryHandler<GetExerciseByFilterQuery, GetExerciseByFilterResult>
    {
        private readonly IExerciseService _exerciseService;

        public GetExerciseByFilterQueryHandler(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        public async Task<Result<GetExerciseByFilterResult>> Handle(GetExerciseByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _exerciseService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetExerciseByFilterResult>(result.Error);

            return Result.Success(new GetExerciseByFilterResult(result.Value.Items, result.Value.Pagination));
        }
    }
}
