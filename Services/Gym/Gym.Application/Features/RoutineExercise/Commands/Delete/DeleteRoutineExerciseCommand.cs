using BuildingBlocks.CQRS.Requests.Commands;

namespace Gym.Application.Features.RoutineExercise.Commands.Delete
{
    public record DeleteRoutineExerciseCommand(int Id) : ICommand<DeleteRoutineExerciseResult>;
    public record DeleteRoutineExerciseResult(bool IsSuccess);
}
