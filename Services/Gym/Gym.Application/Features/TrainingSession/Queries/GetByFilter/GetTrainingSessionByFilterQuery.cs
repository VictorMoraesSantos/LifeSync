using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Gym.Application.DTOs.TrainingSession;

namespace Gym.Application.Features.TrainingSession.Queries.GetByFilter
{
    public record GetTrainingSessionByFilterQuery(TrainingSessionFilterDTO Filter) : IQuery<GetTrainingSessionByFilterResult>;
    public record GetTrainingSessionByFilterResult(IEnumerable<TrainingSessionDTO> Items, PaginationData Pagination);
}
