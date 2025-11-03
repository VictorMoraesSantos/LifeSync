using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Queries.GetByFilter
{
    public class GetMealByFilterQueryHandler : IQueryHandler<GetMealByFilterQuery, GetMealByFilterResult>
    {
        private readonly IMealService _mealService;

        public GetMealByFilterQueryHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<Result<GetMealByFilterResult>> Handle(GetMealByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _mealService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetMealByFilterResult>(result.Error!);

            return Result.Success(new GetMealByFilterResult(result.Value.Items, result.Value.Pagination!));
        }
    }
}
