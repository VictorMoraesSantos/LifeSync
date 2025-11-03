using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.CompletedExercise.Queries.GetByFilter
{
    public class GetCompletedExerciseByFilterQueryHandler : IQueryHandler<GetCompletedExerciseByFilterQuery, GetCompletedExerciseByFilterResult>
    {
        private readonly ICompletedExerciseService _completedExerciseService;

        public GetCompletedExerciseByFilterQueryHandler(ICompletedExerciseService completedExerciseService)
        {
            _completedExerciseService = completedExerciseService;
        }

        public async Task<Result<GetCompletedExerciseByFilterResult>> Handle(GetCompletedExerciseByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _completedExerciseService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetCompletedExerciseByFilterResult>(result.Error);

            return Result.Success(new GetCompletedExerciseByFilterResult(result.Value.Items, result.Value.Pagination));
        }
    }
}
