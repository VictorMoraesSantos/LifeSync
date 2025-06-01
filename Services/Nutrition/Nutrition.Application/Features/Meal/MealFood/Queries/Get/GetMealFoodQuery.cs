using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.Meal.MealFood.Queries.Get
{
    public record GetMealFoodQuery(int Id) : IRequest<GetMealFoodResult>;
    public record GetMealFoodResult(MealFoodDTO MealFood);
}
