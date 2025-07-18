using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.Routine.Commands.DeleteRoutine
{
    public class DeleteRoutineCommandHandler : ICommandHandler<DeleteRoutineCommand, DeleteRoutineResponse>
    {
        private readonly IRoutineService _routineService;

        public DeleteRoutineCommandHandler(IRoutineService routineService)
        {
            _routineService = routineService;
        }

        public async Task<Result<DeleteRoutineResponse>> Handle(DeleteRoutineCommand command, CancellationToken cancellationToken)
        {
            var result = await _routineService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteRoutineResponse>(result.Error!);

            return Result.Success(new DeleteRoutineResponse(result.Value!));
        }
    }
}
