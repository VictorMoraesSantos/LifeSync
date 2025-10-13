using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.CompletedExercise;

namespace Gym.Application.Features.CompletedExercise.Commands.Create
{
    public class CreateCompletedExerciseCommandHandler : ICommandHandler<CreateCompletedExerciseCommand, CreateCompletedExerciseResult>
    {
        private readonly ICompletedExerciseService _completedExerciseService;

        public CreateCompletedExerciseCommandHandler(ICompletedExerciseService completedExerciseService)
        {
            _completedExerciseService = completedExerciseService;
        }

        public async Task<Result<CreateCompletedExerciseResult>> Handle(CreateCompletedExerciseCommand command, CancellationToken cancellationToken)
        {
            var dto = new CreateCompletedExerciseDTO(
                command.TrainingSessionId,
                command.ExerciseId,
                command.RoutineExerciseId,
                command.SetsCompleted,
                command.RepetitionsCompleted,
                command.WeightUsed,
                command.Notes);

            var result = await _completedExerciseService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateCompletedExerciseResult>(result.Error!);

            return Result.Success(new CreateCompletedExerciseResult(result.Value!));
        }
    }
}
