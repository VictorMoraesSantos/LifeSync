using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.RoutineExercise.Queries.GetAllExercises
{
    public class GetAllRoutineExercisesQueryHandler : IQueryHandler<GetAllRoutineExercisesQuery, GetAllRoutineExercisesResult>
    {
        private readonly IRoutineExerciseService _routineExerciseService;

        public GetAllRoutineExercisesQueryHandler(IRoutineExerciseService routineExerciseService)
        {
            _routineExerciseService = routineExerciseService;
        }

        public async Task<Result<GetAllRoutineExercisesResult>> Handle(GetAllRoutineExercisesQuery query, CancellationToken cancellationToken)
        {
            var result = await _routineExerciseService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllRoutineExercisesResult>(result.Error!);

            return Result.Success(new GetAllRoutineExercisesResult(result.Value!));
        }
    }
}
