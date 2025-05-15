using MediatR;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.UseCases.MealFood.Queries.GetAll
{
    public record GetMealFoodsQuery() : IRequest<GetMealFoodsResponse>;
    public record GetMealFoodsResponse(IEnumerable<MealFoodDTO> MealFoods);
}
