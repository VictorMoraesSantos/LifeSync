using MediatR;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.UseCases.MealFood.Queries.GetByMeal
{
    public record GetByMealQuery(int MealId) : IRequest<GetByMealResult>;
    public record GetByMealResult(IEnumerable<MealFoodDTO> MealFoods);
}
