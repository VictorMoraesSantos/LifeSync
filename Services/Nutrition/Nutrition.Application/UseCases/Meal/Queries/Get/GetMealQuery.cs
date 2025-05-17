using MediatR;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.UseCases.Meal.Queries.Get
{
    public record GetMealQuery(int Id) : IRequest<GetMealResult>;
    public record GetMealResult(MealDTO Meal);
}
