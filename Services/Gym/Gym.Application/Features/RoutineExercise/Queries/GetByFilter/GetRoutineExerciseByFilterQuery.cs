using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Gym.Application.DTOs.RoutineExercise;

namespace Gym.Application.Features.RoutineExercise.Queries.GetByFilter
{
    public record GetRoutineExerciseByFilterQuery(RoutineExerciseFilterDTO Filter) : IQuery<GetRoutineExerciseByFilterResult>;
    public record GetRoutineExerciseByFilterResult(IEnumerable<RoutineExerciseDTO> Items, PaginationData Pagination);
}
