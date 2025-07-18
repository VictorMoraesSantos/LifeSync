using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.RoutineExercise.Queries.GetRoutineExerciseById
{
    public class GetRoutineExerciseByIdQueryHandler : IQueryHandler<GetRoutineExerciseByIdQuery, GetRoutineExerciseByIdResponse>
    {
        private readonly IRoutineExerciseService _routineExerciseService;

        public GetRoutineExerciseByIdQueryHandler(IRoutineExerciseService routineExerciseService)
        {
            _routineExerciseService = routineExerciseService;
        }

        public async Task<Result<GetRoutineExerciseByIdResponse>> Handle(GetRoutineExerciseByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _routineExerciseService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetRoutineExerciseByIdResponse>(result.Error!);
            
            return Result.Success(new GetRoutineExerciseByIdResponse(result.Value!));
        }
    }
}
