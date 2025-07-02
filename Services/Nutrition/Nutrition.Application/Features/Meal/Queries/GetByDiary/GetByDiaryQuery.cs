using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.Features.Meal.Queries.GetByDiary
{
    public record GetByDiaryQuery(int Id) : IQuery<GetByDiaryResult>;
    public record GetByDiaryResult(IEnumerable<MealDTO> Meals);
}
