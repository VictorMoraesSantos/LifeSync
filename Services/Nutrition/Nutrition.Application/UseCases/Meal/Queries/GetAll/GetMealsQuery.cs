using MediatR;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.UseCases.Meal.Queries.GetAll
{
    public record GetMealsQuery : IRequest<GetMealsResult>;
    public record GetMealsResult(IEnumerable<MealDTO> Meals);
}
