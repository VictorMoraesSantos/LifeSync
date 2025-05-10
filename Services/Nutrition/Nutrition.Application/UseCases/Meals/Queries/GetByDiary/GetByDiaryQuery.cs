using MediatR;
using Nutrition.Application.DTOs.Meals;

namespace Nutrition.Application.UseCases.Meals.Queries.GetByDiary
{
    public record GetByDiaryQuery(int Id): IRequest<GetByDiaryResponse>;
    public record GetByDiaryResponse(IEnumerable<MealDTO> Meals);
}
