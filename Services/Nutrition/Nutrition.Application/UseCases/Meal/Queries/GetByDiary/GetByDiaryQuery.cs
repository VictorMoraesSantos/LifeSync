using MediatR;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.UseCases.Meal.Queries.GetByDiary
{
    public record GetByDiaryQuery(int Id): IRequest<GetByDiaryResponse>;
    public record GetByDiaryResponse(IEnumerable<MealDTO> Meals);
}
