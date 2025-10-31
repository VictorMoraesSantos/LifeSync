using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.MealFood.Queries.GetByFilter
{
    public record GetByFilterQuery(MealFoodQueryFilterDTO Filter) : IQuery<GetByFilterResult>;
    public record GetByFilterResult(IEnumerable<MealFoodDTO> Items, PaginationData Pagination);
}
