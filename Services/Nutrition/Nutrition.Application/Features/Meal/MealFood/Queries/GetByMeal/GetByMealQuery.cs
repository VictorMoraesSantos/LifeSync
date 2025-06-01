using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.Meal.MealFood.Queries.GetByMeal
{
    public record GetByMealQuery(int MealId) : IRequest<GetByMealResult>;
    public record GetByMealResult(IEnumerable<MealFoodDTO> MealFoods);
}
