using MediatR;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.UseCases.MealFood.Queries.GetByMeal
{
    public record GetByMealQuery(int MealId) : IRequest<GetByMealResponse>;
    public record GetByMealResponse(IEnumerable<MealFoodDTO> MealFoods);
}
