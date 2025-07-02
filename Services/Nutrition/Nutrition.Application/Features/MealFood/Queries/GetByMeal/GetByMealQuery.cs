using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.MealFood.Queries.GetByMeal
{
    public record GetByMealQuery(int MealId) : IQuery<GetByMealResult>;
    public record GetByMealResult(IEnumerable<MealFoodDTO> MealFoods);
}
