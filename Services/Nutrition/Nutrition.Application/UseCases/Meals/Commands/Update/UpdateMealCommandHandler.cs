﻿using MediatR;
using Nutrition.Application.DTOs.Meals;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Meals.Commands.Update
{
    public class UpdateMealCommandHandler : IRequestHandler<UpdateMealCommand, UpdateMealResponse>
    {
        private readonly IMealService _mealService;

        public UpdateMealCommandHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<UpdateMealResponse> Handle(UpdateMealCommand command, CancellationToken cancellationToken)
        {
            UpdateMealDTO dto = new(command.Id, command.Name, command.Description);
            bool result = await _mealService.UpdateAsync(dto, cancellationToken);
            UpdateMealResponse response = new(result);
            return response;
        }
    }
}
