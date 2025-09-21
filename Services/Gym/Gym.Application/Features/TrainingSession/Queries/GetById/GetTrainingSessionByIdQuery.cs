using BuildingBlocks.CQRS.Queries;
using Gym.Application.DTOs.TrainingSession;

namespace Gym.Application.Features.TrainingSession.Queries.GetTrainingSessionById
{
    public record GetTrainingSessionByIdQuery(int Id) : IQuery<GetTrainingSessionByIdResult>;
    public record GetTrainingSessionByIdResult(TrainingSessionDTO TrainingSession);
}
