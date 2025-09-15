using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.CompletedExercise.Commands.DeleteCompletedExercise
{
    public record DeleteCompletedExerciseCommandHandler : ICommandHandler<DeleteCompletedExerciseCommand, DeleteCompletedExerciseResult>
    {
        private readonly ICompletedExerciseService _completedExerciseService;

        public DeleteCompletedExerciseCommandHandler(ICompletedExerciseService completedExerciseService)
        {
            _completedExerciseService = completedExerciseService;
        }

        public async Task<Result<DeleteCompletedExerciseResult>> Handle(DeleteCompletedExerciseCommand command, CancellationToken cancellationToken)
        {
            var result = await _completedExerciseService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteCompletedExerciseResult>(result.Error!);

            return Result.Success(new DeleteCompletedExerciseResult(result.Value!));
        }
    }
}
