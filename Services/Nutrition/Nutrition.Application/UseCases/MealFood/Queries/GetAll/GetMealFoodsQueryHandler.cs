using MediatR;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.MealFood.Queries.GetAll
{
    public class GetMealFoodsQueryHandler : IRequestHandler<GetMealFoodsQuery, GetMealFoodsResult>
    {
        private readonly IMealFoodService _mealFoodService;
     
        public GetMealFoodsQueryHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }
        
        public async Task<GetMealFoodsResult> Handle(GetMealFoodsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<MealFoodDTO> result = await _mealFoodService.GetAllAsync(cancellationToken);
            GetMealFoodsResult response = new(result);
            return response;
        }
    }
}
