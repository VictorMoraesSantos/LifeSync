using MediatR;
using Nutrition.Application.DTOs.Liquid;

namespace Nutrition.Application.UseCases.Liquid.Queries.Get
{
    public record GetLiquidQuery(int Id) : IRequest<GetLiquidResult>;
    public record GetLiquidResult(LiquidDTO Liquid);
}
