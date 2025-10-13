using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.Exercise.Commands.Delete
{
    public class DeleteExerciseCommandHandler : ICommandHandler<DeleteExerciseCommand, DeleteExerciseResult>
    {
        private readonly IExerciseService _exerciseService;

        public DeleteExerciseCommandHandler(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        public async Task<Result<DeleteExerciseResult>> Handle(DeleteExerciseCommand command, CancellationToken cancellationToken)
        {
            var result = await _exerciseService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteExerciseResult>(result.Error!);

            return Result.Success(new DeleteExerciseResult(result.Value!));
        }
    }
}
