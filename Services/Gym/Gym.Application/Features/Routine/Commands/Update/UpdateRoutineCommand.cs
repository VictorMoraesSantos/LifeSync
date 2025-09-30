using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.Routine.Commands.UpdateRoutine
{
    public record UpdateRoutineCommand(
        int Id,
        string Name,
        string Description)
        : ICommand<UpdateRoutineResult>;
    public record UpdateRoutineResult(bool IsSuccess);
}
