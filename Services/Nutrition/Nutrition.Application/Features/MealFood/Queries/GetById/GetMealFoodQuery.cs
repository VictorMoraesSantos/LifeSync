using BuildingBlocks.CQRS.Requests.Queries;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.MealFood.Queries.GetById
{
    public record GetMealFoodQuery(int Id) : IQuery<GetMealFoodResult>;
    public record GetMealFoodResult(MealFoodDTO MealFood);
}
