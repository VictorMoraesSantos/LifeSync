using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Gym.Application.DTOs.Exercise;

namespace Gym.Application.Features.Exercise.Queries.GetByFilter
{
    public record GetExerciseByFilterQuery(ExerciseFilterDTO Filter) : IQuery<GetExerciseByFilterResult>;
    public record GetExerciseByFilterResult(IEnumerable<ExerciseDTO> Items, PaginationData Pagination);
}
