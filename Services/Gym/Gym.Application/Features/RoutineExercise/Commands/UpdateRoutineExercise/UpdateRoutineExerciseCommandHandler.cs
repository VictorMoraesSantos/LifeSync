using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.CompletedExercise;
using Gym.Application.DTOs.RoutineExercise;

namespace Gym.Application.Features.RoutineExercise.Commands.UpdateRoutineExercise
{
    public class UpdateRoutineExerciseCommandHandler : ICommandHandler<UpdateRoutineExerciseCommand, UpdateRoutineExerciseCommandResponse>
    {
        private readonly IRoutineExerciseService _routineExerciseService;

        public UpdateRoutineExerciseCommandHandler(IRoutineExerciseService routineExerciseService)
        {
            _routineExerciseService = routineExerciseService;
        }

        public async Task<Result<UpdateRoutineExerciseCommandResponse>> Handle(UpdateRoutineExerciseCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateRoutineExerciseDTO(
                command.Id,
                command.Sets,
                command.Repetitions,
                command.RestBetweenSets,
                command.RecommendedWeight,
                command.Instructions);

            var result = await _routineExerciseService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateRoutineExerciseCommandResponse>(result.Error!);

            return Result.Success(new UpdateRoutineExerciseCommandResponse(result.Value!));
        }
    }
}
