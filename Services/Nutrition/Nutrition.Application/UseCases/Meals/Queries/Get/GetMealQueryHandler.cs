using MediatR;
using Nutrition.Application.DTOs.Meals;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Meals.Queries.Get
{
    public class GetMealQueryHandler : IRequestHandler<GetMealQuery, GetMealQueryResponse>
    {
        private readonly IMealService _mealService;

        public GetMealQueryHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<GetMealQueryResponse> Handle(GetMealQuery query, CancellationToken cancellationToken)
        {
            MealDTO? result = await _mealService.GetByIdAsync(query.Id, cancellationToken);
            GetMealQueryResponse response = new(result);
            return response;
        }
    }
}
