using MediatR;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.MealFood.Queries.GetByMeal
{
    public class GetByMealQueryHandler : IRequestHandler<GetByMealQuery, GetByMealResult>
    {
        private readonly IMealService _mealService;
     
        public GetByMealQueryHandler(IMealService mealService)
        {
            _mealService = mealService;
        }
        
        public async Task<GetByMealResult> Handle(GetByMealQuery query, CancellationToken cancellationToken)
        {
            MealDTO? result = await _mealService.GetByIdAsync(query.MealId, cancellationToken);
            GetByMealResult response = new(result.MealFoods);
            return response;
        }
    }
}
