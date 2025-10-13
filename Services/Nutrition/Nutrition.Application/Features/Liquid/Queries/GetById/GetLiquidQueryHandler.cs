using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Liquid.Queries.GetById
{
    public class GetLiquidQueryHandler : IQueryHandler<GetLiquidQuery, GetLiquidResult>
    {
        private readonly ILiquidService _liquidService;

        public GetLiquidQueryHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<Result<GetLiquidResult>> Handle(GetLiquidQuery query, CancellationToken cancellationToken)
        {
            var result = await _liquidService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetLiquidResult>(result.Error!);

            return Result.Success(new GetLiquidResult(result.Value!));
        }
    }
}
