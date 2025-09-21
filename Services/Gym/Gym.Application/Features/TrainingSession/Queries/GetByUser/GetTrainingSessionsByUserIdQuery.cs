using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.TrainingSession;

namespace Gym.Application.Features.TrainingSession.Queries.GetTrainingSessionsByUserId
{
    public record GetTrainingSessionsByUserIdQuery(int UserId) : IQuery<GetTrainingSessionsByUserIdResult>;
    public record GetTrainingSessionsByUserIdResult(IEnumerable<TrainingSessionDTO> TrainingSessions);
}
