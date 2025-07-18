using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.RoutineExercise.Commands.DeleteRoutineExercise
{
    public record DeleteRoutineExerciseCommand(int Id) : ICommand<DeleteRoutineExerciseCommandResponse>;
    public record DeleteRoutineExerciseCommandResponse(bool IsSuccess);
}
