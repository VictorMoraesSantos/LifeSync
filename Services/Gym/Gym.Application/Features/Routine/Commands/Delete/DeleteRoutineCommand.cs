using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.Routine.Commands.DeleteRoutine
{
    public record DeleteRoutineCommand(int Id) : ICommand<DeleteRoutineResult>;
    public record DeleteRoutineResult(bool IsSuccess);
}
