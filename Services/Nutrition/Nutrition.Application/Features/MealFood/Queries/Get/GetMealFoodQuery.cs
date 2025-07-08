using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.MealFood.Queries.Get
{
    public record GetMealFoodQuery(int Id) : IQuery<GetMealFoodResult>;
    public record GetMealFoodResult(MealFoodDTO MealFood);
}
