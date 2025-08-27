using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.Exercise;

namespace Gym.Application.Features.Exercise.Commands.UpdateExercise
{
    public class UpdateExerciseCommandHandler : ICommandHandler<UpdateExerciseCommand, UpdateExerciseResult>
    {
        private readonly IExerciseService _exerciseService;

        public UpdateExerciseCommandHandler(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        public async Task<Result<UpdateExerciseResult>> Handle(UpdateExerciseCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateExerciseDTO(
                command.Id,
                command.Name,
                command.Description,
                command.MuscleGroup,
                command.ExerciseType,
                command.EquipmentType);

            var result = await _exerciseService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateExerciseResult>(result.Error!);

            return Result.Success(new UpdateExerciseResult(result.Value!));
        }
    }
}
