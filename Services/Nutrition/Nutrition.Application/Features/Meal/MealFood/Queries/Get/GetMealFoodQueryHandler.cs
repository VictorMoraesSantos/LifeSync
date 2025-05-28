using MediatR;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.MealFood.Queries.Get
{
    public class GetMealFoodQueryHandler : IRequestHandler<GetMealFoodQuery, GetMealFoodResult>
    {
        private readonly IMealFoodService _mealFoodService;

        public GetMealFoodQueryHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }

        public async Task<GetMealFoodResult> Handle(GetMealFoodQuery query, CancellationToken cancellationToken)
        {
            MealFoodDTO? result = await _mealFoodService.GetByIdAsync(query.Id, cancellationToken);

            return new GetMealFoodResult(result);
        }
    }
}
