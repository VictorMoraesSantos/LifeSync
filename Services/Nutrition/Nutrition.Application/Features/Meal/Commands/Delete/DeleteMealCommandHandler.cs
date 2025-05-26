using MediatR;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Commands.Delete
{
    public class DeleteMealCommandHandler : IRequestHandler<DeleteMealCommand, DeleteMealResult>
    {
        private readonly IMealService _mealService;

        public DeleteMealCommandHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<DeleteMealResult> Handle(DeleteMealCommand command, CancellationToken cancellationToken)
        {
            bool result = await _mealService.DeleteAsync(command.Id, cancellationToken);

            return new DeleteMealResult(result);
        }
    }
}
