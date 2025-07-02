using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.Liquid;

namespace Nutrition.Application.Features.Liquid.Queries.GetAll
{
    public record GetAllLiquidsQuery : IQuery<GetAllLiquidsResult>;
    public record GetAllLiquidsResult(IEnumerable<LiquidDTO> Liquids);
}
