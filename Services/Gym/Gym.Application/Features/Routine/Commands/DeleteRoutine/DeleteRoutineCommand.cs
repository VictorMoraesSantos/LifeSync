using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.Routine.Commands.DeleteRoutine
{
    public record DeleteRoutineCommand(int Id) : ICommand<DeleteRoutineResponse>;
    public record DeleteRoutineResponse(bool IsSuccess);
}
