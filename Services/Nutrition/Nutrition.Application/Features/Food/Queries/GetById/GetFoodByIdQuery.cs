using BuildingBlocks.CQRS.Requests.Queries;
using Nutrition.Application.DTOs.Food;

namespace Nutrition.Application.Features.Food.Queries.GetById
{
    public record GetFoodByIdQuery(int Id) : IQuery<GetFoodByIdResult>;
    public record GetFoodByIdResult(FoodDTO Food);
}
