using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Commands.Create
{
    public class CreateMealCommandHandler : ICommandHandler<CreateMealCommand, CreateMealResult>
    {
        private readonly IMealService _mealService;
        private readonly IDiaryService _diaryService;

        public CreateMealCommandHandler(IMealService mealService, IDiaryService diaryService)
        {
            _mealService = mealService;
            _diaryService = diaryService;
        }

        public async Task<Result<CreateMealResult>> Handle(CreateMealCommand command, CancellationToken cancellationToken)
        {
            var diary = await _diaryService.GetByIdAsync(command.DiaryId, cancellationToken);
            if (diary == null)
                return Result.Failure<CreateMealResult>(diary?.Error!);

            var dto = new CreateMealDTO(command.Name, command.Description);

            var result = await _diaryService.AddMealToDiaryAsync(command.DiaryId, dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateMealResult>(result.Error!);

            return Result.Success(new CreateMealResult(result.Value));
        }
    }
}
