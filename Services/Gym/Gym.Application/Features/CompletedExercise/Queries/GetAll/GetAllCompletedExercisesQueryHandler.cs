using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.CompletedExercise.Commands.GetAll
{
    public class GetAllCompletedExercisesQueryHandler : IQueryHandler<GetAllCompletedExercisesQuery, GetAllCompletedExercisesResult>
    {
        private readonly ICompletedExerciseService _completedExerciseService;

        public GetAllCompletedExercisesQueryHandler(ICompletedExerciseService completedExerciseService)
        {
            _completedExerciseService = completedExerciseService;
        }

        public async Task<Result<GetAllCompletedExercisesResult>> Handle(GetAllCompletedExercisesQuery query, CancellationToken cancellationToken)
        {
            var result = await _completedExerciseService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllCompletedExercisesResult>(result.Error!);

            return Result.Success(new GetAllCompletedExercisesResult(result.Value!));
        }
    }
}
