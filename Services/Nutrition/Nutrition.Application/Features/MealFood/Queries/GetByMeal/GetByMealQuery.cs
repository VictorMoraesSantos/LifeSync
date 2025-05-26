using MediatR;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.MealFood.Queries.GetByMeal
{
    public record GetByMealQuery(int MealId) : IRequest<GetByMealResult>;
    public record GetByMealResult(IEnumerable<MealFoodDTO> MealFoods);
}
