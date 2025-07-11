﻿using BuildingBlocks.CQRS.Queries;
using Nutrition.Application.DTOs.MealFood;

namespace Nutrition.Application.Features.MealFood.Queries.GetAll
{
    public record GetMealFoodsQuery() : IQuery<GetMealFoodsResult>;
    public record GetMealFoodsResult(IEnumerable<MealFoodDTO> MealFoods);
}
