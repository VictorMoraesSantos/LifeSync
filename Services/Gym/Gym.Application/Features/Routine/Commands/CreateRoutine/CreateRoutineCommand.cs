using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.Routine.Commands.CreateRoutine
{
    public record CreateRoutineCommand(
        string Name,
        string Description)
        : ICommand<CreateRoutineResponse>;
    public record CreateRoutineResponse(int Id);
}
