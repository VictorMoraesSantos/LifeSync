using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.MealFood.Queries.GetByFilter
{
    public class GetByFilterQueryHandler : IQueryHandler<GetByFilterQuery, GetByFilterResult>
    {
        private readonly IMealFoodService _mealFoodService;

        public GetByFilterQueryHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }

        public async Task<Result<GetByFilterResult>> Handle(GetByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _mealFoodService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetByFilterResult>(result.Error!);

            return Result.Success(new GetByFilterResult(result.Value.Items, result.Value.Pagination!));
        }
    }
}
