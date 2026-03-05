using BuildingBlocks.CQRS.Requests.Queries;
using Nutrition.Application.DTOs.LiquidType;

namespace Nutrition.Application.Features.LiquidType.Queries.GetById
{
    public record GetLiquidTypeQuery(int Id) : IQuery<GetLiquidTypeResult>;
    public record GetLiquidTypeResult(LiquidTypeDTO LiquidType);
}
