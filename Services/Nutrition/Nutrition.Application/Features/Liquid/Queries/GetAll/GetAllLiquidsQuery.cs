using MediatR;
using Nutrition.Application.DTOs.Liquid;

namespace Nutrition.Application.Features.Liquid.Queries.GetAll
{
    public record GetAllLiquidsQuery : IRequest<GetAllLiquidsResult>;
    public record GetAllLiquidsResult(IEnumerable<LiquidDTO> Liquids);
}
