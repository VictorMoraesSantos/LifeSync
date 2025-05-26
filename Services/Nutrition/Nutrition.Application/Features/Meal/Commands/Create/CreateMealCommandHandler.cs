using MediatR;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Commands.Create
{
    public class CreateMealCommandHandler : IRequestHandler<CreateMealCommand, CreateMealResult>
    {
        private readonly IMealService _mealService;
        private readonly IDiaryService _diaryService;

        public CreateMealCommandHandler(IMealService mealService, IDiaryService diaryService)
        {
            _mealService = mealService;
            _diaryService = diaryService;
        }

        public async Task<CreateMealResult> Handle(CreateMealCommand command, CancellationToken cancellationToken)
        {
            DiaryDTO? diary = await _diaryService.GetByIdAsync(command.DiaryId, cancellationToken);
            if (diary == null)
                return new CreateMealResult(false);

            CreateMealDTO dto = new(command.Name, command.Description);

            bool result = await _diaryService.AddMealToDiaryAsync(command.DiaryId, dto, cancellationToken);

            return new CreateMealResult(result);
        }
    }
}
