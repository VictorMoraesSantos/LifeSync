using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.CompletedExercise;

namespace Gym.Application.Features.CompletedExercise.Commands.UpdateCompletedExercise
{
    public class UpdateCompletedExerciseCommandHandler : ICommandHandler<UpdateCompletedExerciseCommand, UpdateCompletedExerciseResult>
    {
        private readonly ICompletedExerciseService _completedExerciseService;

        public UpdateCompletedExerciseCommandHandler(ICompletedExerciseService completedExerciseService)
        {
            _completedExerciseService = completedExerciseService;
        }

        public async Task<Result<UpdateCompletedExerciseResult>> Handle(UpdateCompletedExerciseCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateCompletedExerciseDTO(
                command.Id,
                command.SetsCompleted,
                command.RepetitionsCompleted,
                command.RestBetweenSets,
                command.WeightUsed,
                command.Notes);
            var result = await _completedExerciseService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateCompletedExerciseResult>(result.Error!);

            return Result.Success(new UpdateCompletedExerciseResult(result.Value!));
        }
    }
}
