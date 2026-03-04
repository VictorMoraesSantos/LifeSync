using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Contracts;

namespace Nutrition.Application.Features.Food.Queries.GetAll
{
    public class GetAllFoodsQueryHandler : IQueryHandler<GetAllFoodsQuery, GetAllFoodsResult>
    {
        private readonly IFoodService _foodService;

        public GetAllFoodsQueryHandler(IFoodService foodService)
        {
            _foodService = foodService;
        }

        public async Task<Result<GetAllFoodsResult>> Handle(GetAllFoodsQuery query, CancellationToken cancellationToken)
        {
            var result = await _foodService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess) return Result.Failure<GetAllFoodsResult>(result.Error!);

            return Result.Success(new GetAllFoodsResult(result.Value!));
        }
    }
}
