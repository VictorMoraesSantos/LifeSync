using BuildingBlocks.CQRS.Commands;

namespace Gym.Application.Features.TrainingSession.Commands.Delete
{
    public record DeleteTrainingSessionsCommand(int Id) : ICommand<DeleteTrainingSessionsResult>;
    public record DeleteTrainingSessionsResult(bool IsSuccess);
}
