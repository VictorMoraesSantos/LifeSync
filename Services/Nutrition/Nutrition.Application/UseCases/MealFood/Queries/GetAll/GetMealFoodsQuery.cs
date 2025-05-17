using MediatR;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.UseCases.MealFood.Queries.GetAll
{
    public record GetMealFoodsQuery() : IRequest<GetMealFoodsResult>;
    public record GetMealFoodsResult(IEnumerable<MealFoodDTO> MealFoods);
}
