using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.RoutineExercise.Commands.DeleteRoutineExercise
{
    public class DeleteRoutineExerciseCommandHandler : ICommandHandler<DeleteRoutineExerciseCommand, DeleteRoutineExerciseResult>
    {
        private readonly IRoutineExerciseService _routineExerciseService;

        public DeleteRoutineExerciseCommandHandler(IRoutineExerciseService routineExerciseService)
        {
            _routineExerciseService = routineExerciseService;
        }

        public async Task<Result<DeleteRoutineExerciseResult>> Handle(DeleteRoutineExerciseCommand command, CancellationToken cancellationToken)
        {
            var result = await _routineExerciseService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteRoutineExerciseResult>(result.Error!);

            return Result.Success(new DeleteRoutineExerciseResult(result.Value!));
        }
    }
}
