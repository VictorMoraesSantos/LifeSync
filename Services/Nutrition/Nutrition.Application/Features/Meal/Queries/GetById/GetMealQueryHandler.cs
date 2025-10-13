using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Queries.GetById
{
    public class GetMealQueryHandler : IQueryHandler<GetMealQuery, GetMealResult>
    {
        private readonly IMealService _mealService;

        public GetMealQueryHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<Result<GetMealResult>> Handle(GetMealQuery query, CancellationToken cancellationToken)
        {
            var result = await _mealService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetMealResult>(result.Error!);

            return Result.Success(new GetMealResult(result.Value!));
        }
    }
}
