using MediatR;
using Nutrition.Application.DTOs.Liquid;

namespace Nutrition.Application.UseCases.Liquid.Queries.GetByDiary
{
    public record GetLiquidsByDiaryQuery(int DiaryId) : IRequest<GetLiquidsByDiaryResult>;
    public record GetLiquidsByDiaryResult(IEnumerable<LiquidDTO> Liquids);
}
