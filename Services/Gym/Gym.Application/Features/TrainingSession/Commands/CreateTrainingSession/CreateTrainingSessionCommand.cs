using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.TrainingSession.Commands.CreateTrainingSession
{
    public record CreateTrainingSessionCommand(
        int UserId,
        int RoutineId,
        DateTime StartTime,
        DateTime EndTime)
        : ICommand<CreateTrainingSessionResponse>;
    public record CreateTrainingSessionResponse(int Id);
}
