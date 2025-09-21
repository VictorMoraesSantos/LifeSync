using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.RoutineExercise.Commands.DeleteRoutineExercise
{
    public record DeleteRoutineExerciseCommand(int Id) : ICommand<DeleteRoutineExerciseResult>;
    public record DeleteRoutineExerciseResult(bool IsSuccess);
}
