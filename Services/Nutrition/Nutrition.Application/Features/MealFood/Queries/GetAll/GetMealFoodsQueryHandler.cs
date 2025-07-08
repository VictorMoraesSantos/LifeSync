using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.MealFood.Queries.GetAll
{
    public class GetMealFoodsQueryHandler : IQueryHandler<GetMealFoodsQuery, GetMealFoodsResult>
    {
        private readonly IMealFoodService _mealFoodService;

        public GetMealFoodsQueryHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }

        public async Task<Result<GetMealFoodsResult>> Handle(GetMealFoodsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mealFoodService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result<GetMealFoodsResult>.Failure(result.Error!);

            return Result.Success(new GetMealFoodsResult(result.Value!));
        }
    }
}
