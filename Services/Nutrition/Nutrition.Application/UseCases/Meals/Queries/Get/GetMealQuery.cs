using MediatR;
using Nutrition.Application.DTOs.Meals;

namespace Nutrition.Application.UseCases.Meals.Queries.Get
{
    public record GetMealQuery(int Id) : IRequest<GetMealQueryResponse>;
    public record GetMealQueryResponse(MealDTO Meal);
}
