﻿using MediatR;
using Nutrition.Application.DTOs.Meals;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Meals.Queries.GetAll
{
    public class GetMealsQueryHandler : IRequestHandler<GetMealsQuery, GetMealsQueryResponse>
    {
        private readonly IMealService _mealService;

        public GetMealsQueryHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<GetMealsQueryResponse> Handle(GetMealsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<MealDTO> result = await _mealService.GetAllAsync(cancellationToken);
            GetMealsQueryResponse response = new(result);
            return response;
        }
    }
}