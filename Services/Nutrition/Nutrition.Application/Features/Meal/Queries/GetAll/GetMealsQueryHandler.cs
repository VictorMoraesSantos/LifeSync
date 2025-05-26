using MediatR;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Queries.GetAll
{
    public class GetMealsQueryHandler : IRequestHandler<GetMealsQuery, GetMealsResult>
    {
        private readonly IMealService _mealService;

        public GetMealsQueryHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<GetMealsResult> Handle(GetMealsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<MealDTO> result = await _mealService.GetAllAsync(cancellationToken);

            return new GetMealsResult(result);
        }
    }
}