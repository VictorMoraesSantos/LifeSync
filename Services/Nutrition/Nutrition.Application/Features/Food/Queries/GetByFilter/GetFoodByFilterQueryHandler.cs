using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Contracts;

namespace Nutrition.Application.Features.Food.Queries.GetByFilter
{
    public class GetFoodByFilterQueryHandler : IQueryHandler<GetFoodByFilterQuery, GetFoodByFilterResult>
    {
        private readonly IFoodService _foodService;

        public GetFoodByFilterQueryHandler(IFoodService foodService)
        {
            _foodService = foodService;
        }

        public async Task<Result<GetFoodByFilterResult>> Handle(GetFoodByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _foodService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess) return Result.Failure<GetFoodByFilterResult>(result.Error!);

            return Result.Success(new GetFoodByFilterResult(result.Value.Items, result.Value.Pagination));
        }
    }
}
