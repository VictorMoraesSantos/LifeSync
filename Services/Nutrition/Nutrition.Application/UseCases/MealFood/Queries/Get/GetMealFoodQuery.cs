using MediatR;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.UseCases.MealFood.Queries.Get
{
    public record GetMealFoodQuery(int Id) : IRequest<GetMealFoodResponse>;
    public record GetMealFoodResponse(MealFoodDTO MealFood);
}
