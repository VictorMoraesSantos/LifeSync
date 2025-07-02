using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.Features.Meal.Queries.Get
{
    public record GetMealQuery(int Id) : IQuery<GetMealResult>;
    public record GetMealResult(MealDTO Meal);
}
