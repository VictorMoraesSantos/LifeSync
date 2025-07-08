using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.MealFood.Queries.GetByMeal
{
    public class GetByMealQueryHandler : IQueryHandler<GetByMealQuery, GetByMealResult>
    {
        private readonly IMealService _mealService;

        public GetByMealQueryHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<Result<GetByMealResult>> Handle(GetByMealQuery query, CancellationToken cancellationToken)
        {
            var result = await _mealService.GetByIdAsync(query.MealId, cancellationToken);
            if (!result.IsSuccess)
                return Result<GetByMealResult>.Failure(result.Error!);

            return Result.Success(new GetByMealResult(result.Value!.MealFoods));
        }
    }
}
