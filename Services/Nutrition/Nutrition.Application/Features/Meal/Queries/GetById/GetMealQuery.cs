using BuildingBlocks.CQRS.Requests.Queries;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.Features.Meal.Queries.GetById
{
    public record GetMealQuery(int Id) : IQuery<GetMealResult>;
    public record GetMealResult(MealDTO Meal);
}
