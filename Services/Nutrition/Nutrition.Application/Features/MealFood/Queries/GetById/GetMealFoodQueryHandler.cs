using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.MealFood.Queries.GetById
{
    public class GetMealFoodQueryHandler : IQueryHandler<GetMealFoodQuery, GetMealFoodResult>
    {
        private readonly IMealFoodService _mealFoodService;

        public GetMealFoodQueryHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }

        public async Task<Result<GetMealFoodResult>> Handle(GetMealFoodQuery query, CancellationToken cancellationToken)
        {
            var result = await _mealFoodService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result<GetMealFoodResult>.Failure(result.Error!);

            return Result.Success(new GetMealFoodResult(result.Value!));
        }
    }
}
