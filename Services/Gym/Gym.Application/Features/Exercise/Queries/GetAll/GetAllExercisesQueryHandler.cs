using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.Exercise.Queries.GetAll
{
    public class GetAllExercisesQueryHandler : IQueryHandler<GetAllExercisesQuery, GetAllExercisesResult>
    {
        private readonly IExerciseService _exerciseService;

        public GetAllExercisesQueryHandler(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        public async Task<Result<GetAllExercisesResult>> Handle(GetAllExercisesQuery query, CancellationToken cancellationToken)
        {
            var result = await _exerciseService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllExercisesResult>(result.Error!);

            return Result.Success(new GetAllExercisesResult(result.Value!));
        }
    }
}
