using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.LiquidType.Queries.GetById
{
    public class GetLiquidTypeQueryHandler : IQueryHandler<GetLiquidTypeQuery, GetLiquidTypeResult>
    {
        private readonly ILiquidTypeService _liquidTypeService;

        public GetLiquidTypeQueryHandler(ILiquidTypeService liquidTypeService)
        {
            _liquidTypeService = liquidTypeService;
        }

        public async Task<Result<GetLiquidTypeResult>> Handle(GetLiquidTypeQuery query, CancellationToken cancellationToken)
        {
            var result = await _liquidTypeService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetLiquidTypeResult>(result.Error!);

            return Result.Success(new GetLiquidTypeResult(result.Value!));
        }
    }
}
