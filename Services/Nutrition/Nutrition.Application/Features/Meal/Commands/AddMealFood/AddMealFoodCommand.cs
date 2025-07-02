using BuildingBlocks.CQRS.Commands;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.Meal.Commands.AddMealFood
{
    public record AddMealFoodCommand(int MealId, CreateMealFoodDTO MealFood) : ICommand<AddMealFoodResult>;
    public record AddMealFoodResult(bool IsSuccess);
}
