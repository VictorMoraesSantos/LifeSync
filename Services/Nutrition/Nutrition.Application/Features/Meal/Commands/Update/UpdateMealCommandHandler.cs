using MediatR;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Commands.Update
{
    public class UpdateMealCommandHandler : IRequestHandler<UpdateMealCommand, UpdateMealResult>
    {
        private readonly IMealService _mealService;

        public UpdateMealCommandHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<UpdateMealResult> Handle(UpdateMealCommand command, CancellationToken cancellationToken)
        {
            UpdateMealDTO dto = new(command.Id, command.Name, command.Description);

            bool result = await _mealService.UpdateAsync(dto, cancellationToken);

            return new UpdateMealResult(result);
        }
    }
}
