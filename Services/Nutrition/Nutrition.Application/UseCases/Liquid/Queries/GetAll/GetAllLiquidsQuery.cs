using MediatR;
using Nutrition.Application.DTOs.Liquid;

namespace Nutrition.Application.UseCases.Liquid.Queries.GetAll
{
    public record GetAllLiquidsQuery : IRequest<GetAllLiquidsResult>;
    public record GetAllLiquidsResult(IEnumerable<LiquidDTO> Liquids);
}
