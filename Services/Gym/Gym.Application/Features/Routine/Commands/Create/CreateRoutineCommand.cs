using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.Routine.Commands.Create
{
    public record CreateRoutineCommand(
        string Name,
        string Description)
        : ICommand<CreateRoutineResult>;
    public record CreateRoutineResult(int Id);
}
