using BuildingBlocks.CQRS.Commands;
using Gym.Application.DTOs.Routine;

namespace Gym.Application.Features.Routine.Commands.CreateRoutine
{
    public record CreateRoutineCommand(
        int UserId,
        string Name,
        string Description)
        : ICommand<CreateRoutineResponse>;
    public record CreateRoutineResponse(int Id);
}
