using MediatR;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Meals.Commands.Delete
{
    public class DeleteMealCommandHandler : IRequestHandler<DeleteMealCommand, DeleteMealResponse>
    {
        private readonly IMealService _mealService;
     
        public DeleteMealCommandHandler(IMealService mealService)
        {
            _mealService = mealService;
        }
        
        public async Task<DeleteMealResponse> Handle(DeleteMealCommand command, CancellationToken cancellationToken)
        {
            bool result = await _mealService.DeleteAsync(command.Id, cancellationToken);
            DeleteMealResponse response = new(result);
            return response;
        }
    }
}
