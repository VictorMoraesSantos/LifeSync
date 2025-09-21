using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.Routine;

namespace Gym.Application.Features.Routine.Commands.UpdateRoutineCommand
{
    public class UpdateRoutineCommandHandler : ICommandHandler<UpdateRoutineCommand, UpdateRoutineResult>
    {
        private readonly IRoutineService _routineService;

        public UpdateRoutineCommandHandler(IRoutineService routineService)
        {
            _routineService = routineService;
        }

        public async Task<Result<UpdateRoutineResult>> Handle(UpdateRoutineCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateRoutineDTO(
                command.Id,
                command.Name,
                command.Description);

            var result = await _routineService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateRoutineResult>(result.Error!);

            return Result.Success(new UpdateRoutineResult(result.Value!));
        }
    }
}
