using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.LiquidType.Queries.GetByFilter
{
    public class GetLiquidTypeByFilterQueryHandler : IQueryHandler<GetLiquidTypeByFilterQuery, GetLiquidTypeByFilterResult>
    {
        private readonly ILiquidTypeService _liquidTypeService;

        public GetLiquidTypeByFilterQueryHandler(ILiquidTypeService liquidTypeService)
        {
            _liquidTypeService = liquidTypeService;
        }

        public async Task<Result<GetLiquidTypeByFilterResult>> Handle(GetLiquidTypeByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _liquidTypeService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetLiquidTypeByFilterResult>(result.Error!);

            return Result.Success(new GetLiquidTypeByFilterResult(result.Value.Items, result.Value.Pagination!));
        }
    }
}
