using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.Features.Meal.Queries.Get
{
    public record GetMealQuery(int Id) : IRequest<GetMealResult>;
    public record GetMealResult(MealDTO Meal);
}
