using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Liquid.Queries.GetByFilter
{
    public class GetByFilterQueryHandler : IQueryHandler<GetByFilterQuery, GetByFilterResult>
    {
        private readonly ILiquidService _liquidService;

        public GetByFilterQueryHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<Result<GetByFilterResult>> Handle(GetByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _liquidService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetByFilterResult>(result.Error!);

            return Result.Success(new GetByFilterResult(result.Value.Items, result.Value.Pagination!));
        }
    }
}
