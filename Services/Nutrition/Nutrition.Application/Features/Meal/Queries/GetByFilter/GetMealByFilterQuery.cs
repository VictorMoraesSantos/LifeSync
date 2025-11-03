using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.Features.Meal.Queries.GetByFilter
{
    public record GetMealByFilterQuery(MealQueryFilterDTO Filter) : IQuery<GetMealByFilterResult>;
    public record GetMealByFilterResult(IEnumerable<MealDTO> Items, PaginationData Pagination);
}
