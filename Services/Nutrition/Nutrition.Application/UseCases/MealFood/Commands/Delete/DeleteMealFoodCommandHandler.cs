using MediatR;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.MealFood.Commands.Delete
{
    public class DeleteMealFoodCommandHandler : IRequestHandler<DeleteMealFoodCommand, DeleteMealFoodResult>
    {
        private readonly IMealFoodService _mealFoodService;

        public DeleteMealFoodCommandHandler(IMealFoodService mealFoodService)
        {
            _mealFoodService = mealFoodService;
        }

        public async Task<DeleteMealFoodResult> Handle(DeleteMealFoodCommand command, CancellationToken cancellationToken)
        {
            bool result = await _mealFoodService.DeleteAsync(command.Id);
            DeleteMealFoodResult response = new(result);
            return response;
        }
    }
}
