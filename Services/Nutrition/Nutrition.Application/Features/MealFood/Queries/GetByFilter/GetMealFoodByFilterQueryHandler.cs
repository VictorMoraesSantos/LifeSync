using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.MealFood.Queries.GetByFilter
{
    public class GetMealFoodByFilterQueryHandler : IQueryHandler<GetMealFoodByFilterQuery, GetMealFoodByFilterResult>
    {
        private readonly IMealFoodService _mealFoodService;

        public GetMealFoodByFilterQueryHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }

        public async Task<Result<GetMealFoodByFilterResult>> Handle(GetMealFoodByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _mealFoodService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetMealFoodByFilterResult>(result.Error!);

            return Result.Success(new GetMealFoodByFilterResult(result.Value.Items, result.Value.Pagination!));
        }
    }
}
