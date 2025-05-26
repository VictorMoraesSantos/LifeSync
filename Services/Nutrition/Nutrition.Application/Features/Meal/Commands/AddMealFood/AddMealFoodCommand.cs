using MediatR;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.Meal.Commands.AddMealFood
{
    public record AddMealFoodCommand(int MealId, CreateMealFoodDTO MealFood) : IRequest<AddMealFoodResult>;
    public record AddMealFoodResult(bool IsSuccess);
}
