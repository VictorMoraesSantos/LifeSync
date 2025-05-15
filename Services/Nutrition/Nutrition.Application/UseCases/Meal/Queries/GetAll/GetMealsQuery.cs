using MediatR;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.UseCases.Meal.Queries.GetAll
{
    public record GetMealsQuery : IRequest<GetMealsQueryResponse>;
    public record GetMealsQueryResponse(IEnumerable<MealDTO> Meals);
}
