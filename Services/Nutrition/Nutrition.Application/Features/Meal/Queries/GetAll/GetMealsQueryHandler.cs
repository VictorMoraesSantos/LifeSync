using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Queries.GetAll
{
    public class GetMealsQueryHandler : IQueryHandler<GetMealsQuery, GetMealsResult>
    {
        private readonly IMealService _mealService;

        public GetMealsQueryHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<Result<GetMealsResult>> Handle(GetMealsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mealService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetMealsResult>(result.Error!);

            return Result.Success(new GetMealsResult(result.Value!));
        }
    }
}