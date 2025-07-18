using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.Exercise.Queries.GetById
{
    public class GetExerciseByIdQueryHandler : IQueryHandler<GetExerciseByIdQuery, GetExerciseByIdResult>
    {
        private readonly IExerciseService _exerciseService;

        public GetExerciseByIdQueryHandler(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        public async Task<Result<GetExerciseByIdResult>> Handle(GetExerciseByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _exerciseService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetExerciseByIdResult>(result.Error!);

            return Result.Success(new GetExerciseByIdResult(result.Value!));
        }
    }
}
