using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.TrainingSession.Commands.DeleteTrainingSessions
{
    public record DeleteTrainingSessionsCommand(int Id) : ICommand<DeleteTrainingSessionsResponse>;
    public record DeleteTrainingSessionsResponse(bool IsSuccess);
}
