using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.RoutineExercise;

namespace Gym.Application.Features.RoutineExercise.Commands.Create
{
    public class CreateRoutineExerciseCommandHandler : ICommandHandler<CreateRoutineExerciseCommand, CreateRoutineExerciseResult>
    {
        private readonly IRoutineExerciseService _routineExerciseService;

        public CreateRoutineExerciseCommandHandler(IRoutineExerciseService routineExerciseService)
        {
            _routineExerciseService = routineExerciseService;
        }

        public async Task<Result<CreateRoutineExerciseResult>> Handle(CreateRoutineExerciseCommand command, CancellationToken cancellationToken)
        {
            var dto = new CreateRoutineExerciseDTO(
                command.RoutineId,
                command.ExerciseId,
                command.Sets,
                command.Repetitions,
                command.RestBetweenSets,
                command.RecommendedWeight,
                command.Instructions);

            var result = await _routineExerciseService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateRoutineExerciseResult>(result.Error!);

            return Result.Success(new CreateRoutineExerciseResult(result.Value!));
        }
    }
}
