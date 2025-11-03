
using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.MealFood.Queries.GetByFilter
{
    public record GetMealFoodByFilterQuery(MealFoodQueryFilterDTO Filter) : IQuery<GetMealFoodByFilterResult>;
    public record GetMealFoodByFilterResult(IEnumerable<MealFoodDTO> Items, PaginationData Pagination);
}
