using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.Exercise;

namespace Gym.Application.Features.Exercise.Commands.CreateExercise
{
    public class CreateExerciseCommandHandler : ICommandHandler<CreateExerciseCommand, CreateExerciseResult>
    {
        private readonly IExerciseService _exerciseService;

        public CreateExerciseCommandHandler(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        public async Task<Result<CreateExerciseResult>> Handle(CreateExerciseCommand command, CancellationToken cancellationToken)
        {
            var dto = new CreateExerciseDTO(
                command.Name,
                command.Description,
                command.MuscleGroup,
                command.Type,
                command.EquipmentType);

            var result = await _exerciseService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateExerciseResult>(result.Error!);

            return Result.Success(new CreateExerciseResult(result.Value!));
        }
    }
}
