using BuildingBlocks.CQRS.Requests.Queries;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.Features.Meal.Queries.GetAll
{
    public record GetMealsQuery : IQuery<GetMealsResult>;
    public record GetMealsResult(IEnumerable<MealDTO> Meals);
}
