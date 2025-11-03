using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Gym.Application.DTOs.CompletedExercise;

namespace Gym.Application.Features.CompletedExercise.Queries.GetByFilter
{
    public record GetCompletedExerciseByFilterQuery(CompletedExerciseFilterDTO Filter) : IQuery<GetCompletedExerciseByFilterResult>;
    public record GetCompletedExerciseByFilterResult(IEnumerable<CompletedExerciseDTO> Items, PaginationData Pagination);
}
