using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.Meal.MealFood.Queries.GetAll
{
    public record GetMealFoodsQuery() : IRequest<GetMealFoodsResult>;
    public record GetMealFoodsResult(IEnumerable<MealFoodDTO> MealFoods);
}
