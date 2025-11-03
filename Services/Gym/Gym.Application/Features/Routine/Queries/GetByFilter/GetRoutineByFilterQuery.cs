using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Gym.Application.DTOs.Routine;

namespace Gym.Application.Features.Routine.Queries.GetByFilter
{
    public record GetRoutineByFilterQuery(RoutineFilterDTO Filter) : IQuery<GetRoutineByFilterResult>;
    public record GetRoutineByFilterResult(IEnumerable<RoutineDTO> Items, PaginationData Pagination);
}
