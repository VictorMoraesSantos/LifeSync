using BuildingBlocks.CQRS.Requests.Queries;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.Food;

namespace Nutrition.Application.Features.Food.Queries.GetByFilter
{
    public record GetFoodByFilterQuery(FoodQueryFilterDTO Filter) : IQuery<GetFoodByFilterResult>;
    public record GetFoodByFilterResult(IEnumerable<FoodDTO> Items, PaginationData Pagination);
}
