using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.RoutineExercise.Commands.DeleteRoutineExercise
{
    public class DeleteRoutineExerciseCommandHandler : ICommandHandler<DeleteRoutineExerciseCommand, DeleteRoutineExerciseCommandResponse>
    {
        private readonly IRoutineExerciseService _routineExerciseService;

        public DeleteRoutineExerciseCommandHandler(IRoutineExerciseService routineExerciseService)
        {
            _routineExerciseService = routineExerciseService;
        }

        public async Task<Result<DeleteRoutineExerciseCommandResponse>> Handle(DeleteRoutineExerciseCommand command, CancellationToken cancellationToken)
        {
            var result = await _routineExerciseService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteRoutineExerciseCommandResponse>(result.Error!);

            return Result.Success(new DeleteRoutineExerciseCommandResponse(result.Value!));
        }
    }
}
