using MediatR;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.UseCases.Meal.Queries.Get
{
    public record GetMealQuery(int Id) : IRequest<GetMealQueryResponse>;
    public record GetMealQueryResponse(MealDTO Meal);
}
