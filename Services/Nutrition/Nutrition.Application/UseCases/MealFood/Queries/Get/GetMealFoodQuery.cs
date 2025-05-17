using MediatR;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.UseCases.MealFood.Queries.Get
{
    public record GetMealFoodQuery(int Id) : IRequest<GetMealFoodResult>;
    public record GetMealFoodResult(MealFoodDTO MealFood);
}
