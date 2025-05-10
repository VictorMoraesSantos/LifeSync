using MediatR;
using Nutrition.Application.DTOs.Diaries;
using Nutrition.Application.DTOs.Meals;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Meals.Commands.Create
{
    public class CreateMealCommandHandler : IRequestHandler<CreateMealCommand, CreateMealResponse>
    {
        private readonly IMealService _mealService;
        private readonly IDiaryService _diaryService;

        public CreateMealCommandHandler(IMealService mealService, IDiaryService diaryService)
        {
            _mealService = mealService;
            _diaryService = diaryService;
        }

        public async Task<CreateMealResponse> Handle(CreateMealCommand command, CancellationToken cancellationToken)
        {
            DiaryDTO? diary = await _diaryService.GetByIdAsync(command.DiaryId, cancellationToken);
            if (diary == null)
                return new CreateMealResponse(false);

            CreateMealDTO dto = new(command.DiaryId, command.Name, command.Description);
            bool result = await _mealService.CreateAsync(dto, cancellationToken);
            CreateMealResponse response = new(result);
            return response;
        }
    }
}
