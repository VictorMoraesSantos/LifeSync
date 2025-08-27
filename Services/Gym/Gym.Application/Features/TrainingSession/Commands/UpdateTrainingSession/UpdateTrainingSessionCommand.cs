using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.TrainingSession.Commands.UpdateTrainingSession
{
    public record UpdateTrainingSessionCommand(
        int Id,
        int RoutineId,
        DateTime StartTime,
        DateTime EndTime,
        string Notes)
        : ICommand<UpdateTrainingSessionResponse>;
    public record UpdateTrainingSessionResponse(bool IsSuccess);
}
