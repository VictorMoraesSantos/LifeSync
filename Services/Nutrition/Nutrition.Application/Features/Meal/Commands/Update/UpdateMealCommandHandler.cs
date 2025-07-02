using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Commands.Update
{
    public class UpdateMealCommandHandler : ICommandHandler<UpdateMealCommand, UpdateMealResult>
    {
        private readonly IMealService _mealService;

        public UpdateMealCommandHandler(IMealService mealService)
        {
            _mealService = mealService;
        }

        public async Task<Result<UpdateMealResult>> Handle(UpdateMealCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateMealDTO(command.Id, command.Name, command.Description);

            var result = await _mealService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateMealResult>(result.Error!);

            return Result.Success(new UpdateMealResult(result.Value));
        }
    }
}
