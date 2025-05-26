using MediatR;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.Features.Meal.Queries.GetByDiary
{
    public record GetByDiaryQuery(int Id) : IRequest<GetByDiaryResult>;
    public record GetByDiaryResult(IEnumerable<MealDTO> Meals);
}
