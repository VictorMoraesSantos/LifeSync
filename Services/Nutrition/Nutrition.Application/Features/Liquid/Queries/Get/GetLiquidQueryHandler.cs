using MediatR;
using Nutrition.Application.DTOs.Liquid;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Liquid.Queries.Get
{
    public class GetLiquidQueryHandler : IRequestHandler<GetLiquidQuery, GetLiquidResult>
    {
        private readonly ILiquidService _liquidService;

        public GetLiquidQueryHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<GetLiquidResult> Handle(GetLiquidQuery query, CancellationToken cancellationToken)
        {
            LiquidDTO? result = await _liquidService.GetByIdAsync(query.Id, cancellationToken);

            return new GetLiquidResult(result);
        }
    }
}
