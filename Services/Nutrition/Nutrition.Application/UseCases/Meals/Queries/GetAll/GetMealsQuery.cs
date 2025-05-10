using MediatR;
using Nutrition.Application.DTOs.Meals;

namespace Nutrition.Application.UseCases.Meals.Queries.GetAll
{
    public record GetMealsQuery : IRequest<GetMealsQueryResponse>;
    public record GetMealsQueryResponse(IEnumerable<MealDTO> Meals);
}
