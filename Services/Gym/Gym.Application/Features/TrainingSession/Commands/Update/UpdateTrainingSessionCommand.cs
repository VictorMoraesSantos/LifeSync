using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.TrainingSession.Commands.Update
{
    public record UpdateTrainingSessionCommand(
        int Id,
        int RoutineId,
        DateTime StartTime,
        DateTime EndTime,
        string Notes)
        : ICommand<UpdateTrainingSessionResult>;
    public record UpdateTrainingSessionResult(bool IsSuccess);
}
