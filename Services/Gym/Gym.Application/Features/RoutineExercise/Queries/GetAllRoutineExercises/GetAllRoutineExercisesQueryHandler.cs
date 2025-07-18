using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.RoutineExercise.Queries.GetAllExercises
{
    public class GetAllRoutineExercisesQueryHandler : IQueryHandler<GetAllRoutineExercisesQuery, GetAllRoutineExercisesResponse>
    {
        private readonly IRoutineExerciseService _routineExerciseService;

        public GetAllRoutineExercisesQueryHandler(IRoutineExerciseService routineExerciseService)
        {
            _routineExerciseService = routineExerciseService;
        }

        public async Task<Result<GetAllRoutineExercisesResponse>> Handle(GetAllRoutineExercisesQuery query, CancellationToken cancellationToken)
        {
            var result = await _routineExerciseService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllRoutineExercisesResponse>(result.Error!);

            return Result.Success(new GetAllRoutineExercisesResponse(result.Value!));
        }
    }
}
