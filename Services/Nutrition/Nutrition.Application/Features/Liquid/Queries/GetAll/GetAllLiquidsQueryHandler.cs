using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Liquid.Queries.GetAll
{
    public class GetAllLiquidsQueryHandler : IQueryHandler<GetAllLiquidsQuery, GetAllLiquidsResult>
    {
        private readonly ILiquidService _liquidService;

        public GetAllLiquidsQueryHandler(ILiquidService liquidService)
        {
            _liquidService = liquidService;
        }

        public async Task<Result<GetAllLiquidsResult>> Handle(GetAllLiquidsQuery query, CancellationToken cancellationToken)
        {
            var result = await _liquidService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllLiquidsResult>(result.Error!);

            return Result.Success(new GetAllLiquidsResult(result.Value!));
        }
    }
}
