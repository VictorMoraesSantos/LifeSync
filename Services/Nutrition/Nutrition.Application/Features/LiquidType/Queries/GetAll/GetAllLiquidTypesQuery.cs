using BuildingBlocks.CQRS.Requests.Queries;
using Nutrition.Application.DTOs.LiquidType;

namespace Nutrition.Application.Features.LiquidType.Queries.GetAll
{
    public record GetAllLiquidTypesQuery : IQuery<GetAllLiquidTypesResult>;
    public record GetAllLiquidTypesResult(IEnumerable<LiquidTypeDTO> LiquidTypes);
}
