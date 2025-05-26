using MediatR;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Queries.Get
{
    public class GetMealQueryHandler : IRequestHandler<GetMealQuery, GetMealResult>
    {
        private readonly IMealService _mealService;

        public GetMealQueryHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<GetMealResult> Handle(GetMealQuery query, CancellationToken cancellationToken)
        {
            MealDTO? result = await _mealService.GetByIdAsync(query.Id, cancellationToken);

            return new GetMealResult(result);
        }
    }
}
