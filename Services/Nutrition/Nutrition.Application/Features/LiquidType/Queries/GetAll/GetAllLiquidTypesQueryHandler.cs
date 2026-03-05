using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.LiquidType.Queries.GetAll
{
    public class GetAllLiquidTypesQueryHandler : IQueryHandler<GetAllLiquidTypesQuery, GetAllLiquidTypesResult>
    {
        private readonly ILiquidTypeService _liquidTypeService;

        public GetAllLiquidTypesQueryHandler(ILiquidTypeService liquidTypeService)
        {
            _liquidTypeService = liquidTypeService;
        }

        public async Task<Result<GetAllLiquidTypesResult>> Handle(GetAllLiquidTypesQuery query, CancellationToken cancellationToken)
        {
            var result = await _liquidTypeService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllLiquidTypesResult>(result.Error!);

            return Result.Success(new GetAllLiquidTypesResult(result.Value!));
        }
    }
}
