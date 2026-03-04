using BuildingBlocks.CQRS.Requests.Queries;
using Nutrition.Application.DTOs.Food;

namespace Nutrition.Application.Features.Food.Queries.GetAll
{
    public record GetAllFoodsQuery() : IQuery<GetAllFoodsResult>;
    public record GetAllFoodsResult(IEnumerable<FoodDTO> Foods);
}
