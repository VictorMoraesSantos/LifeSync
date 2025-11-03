using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Liquid.Queries.GetByFilter
{
    public class GetLiquidByFilterQueryHandler : IQueryHandler<GetLiquidByFilterQuery, GetLiquidByFilterResult>
    {
        private readonly ILiquidService _liquidService;

        public GetLiquidByFilterQueryHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<Result<GetLiquidByFilterResult>> Handle(GetLiquidByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _liquidService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetLiquidByFilterResult>(result.Error!);

            return Result.Success(new GetLiquidByFilterResult(result.Value.Items, result.Value.Pagination!));
        }
    }
}
