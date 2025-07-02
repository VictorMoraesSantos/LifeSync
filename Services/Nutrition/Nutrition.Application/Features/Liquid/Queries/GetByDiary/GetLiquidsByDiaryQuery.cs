using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.Liquid;

namespace Nutrition.Application.Features.Liquid.Queries.GetByDiary
{
    public record GetLiquidsByDiaryQuery(int DiaryId) : IQuery<GetLiquidsByDiaryResult>;
    public record GetLiquidsByDiaryResult(IEnumerable<LiquidDTO> Liquids);
}
