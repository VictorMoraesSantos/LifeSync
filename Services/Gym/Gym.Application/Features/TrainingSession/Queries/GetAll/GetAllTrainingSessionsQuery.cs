using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.TrainingSession;

namespace Gym.Application.Features.TrainingSession.Queries.GetAll
{
    public record GetAllTrainingSessionsQuery() : IQuery<GetAllTrainingSessionsResult>;
    public record GetAllTrainingSessionsResult(IEnumerable<TrainingSessionDTO> TrainingSessions);
}
