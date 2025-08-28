using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.RoutineExercise.Queries.GetRoutineExerciseById
{
    public class GetRoutineExerciseByIdQueryHandler : IQueryHandler<GetRoutineExerciseByIdQuery, GetRoutineExerciseByIdResult>
    {
        private readonly IRoutineExerciseService _routineExerciseService;

        public GetRoutineExerciseByIdQueryHandler(IRoutineExerciseService routineExerciseService)
        {
            _routineExerciseService = routineExerciseService;
        }

        public async Task<Result<GetRoutineExerciseByIdResult>> Handle(GetRoutineExerciseByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _routineExerciseService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetRoutineExerciseByIdResult>(result.Error!);

            return Result.Success(new GetRoutineExerciseByIdResult(result.Value!));
        }
    }
}
