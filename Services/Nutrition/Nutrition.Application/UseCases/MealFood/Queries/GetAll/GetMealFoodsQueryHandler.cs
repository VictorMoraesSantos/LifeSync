using MediatR;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.MealFood.Queries.GetAll
{
    public class GetMealFoodsQueryHandler : IRequestHandler<GetMealFoodsQuery, GetMealFoodsResponse>
    {
        private readonly IMealFoodService _mealFoodService;
     
        public GetMealFoodsQueryHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }
        
        public async Task<GetMealFoodsResponse> Handle(GetMealFoodsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<MealFoodDTO> result = await _mealFoodService.GetAllAsync(cancellationToken);
            GetMealFoodsResponse response = new(result);
            return response;
        }
    }
}
