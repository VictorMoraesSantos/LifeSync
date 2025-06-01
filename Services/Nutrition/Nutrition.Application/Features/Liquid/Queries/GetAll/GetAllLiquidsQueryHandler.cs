using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.Liquid;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Liquid.Queries.GetAll
{
    public class GetAllLiquidsQueryHandler : IRequestHandler<GetAllLiquidsQuery, GetAllLiquidsResult>
    {
        private readonly ILiquidService _liquidService;

        public GetAllLiquidsQueryHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<GetAllLiquidsResult> Handle(GetAllLiquidsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<LiquidDTO?> result = await _liquidService.GetAllAsync(cancellationToken);

            return new GetAllLiquidsResult(result);
        }
    }
}
