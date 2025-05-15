using MediatR;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.MealFood.Queries.Get
{
    public class GetMealFoodQueryHandler : IRequestHandler<GetMealFoodQuery, GetMealFoodResponse>
    {
        private readonly IMealFoodService _mealFoodService;

        public GetMealFoodQueryHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }

        public async Task<GetMealFoodResponse> Handle(GetMealFoodQuery query, CancellationToken cancellationToken)
        {
            MealFoodDTO result = await _mealFoodService.GetByIdAsync(query.Id, cancellationToken);
            GetMealFoodResponse response = new(result);
            return response;
        }
    }
}
