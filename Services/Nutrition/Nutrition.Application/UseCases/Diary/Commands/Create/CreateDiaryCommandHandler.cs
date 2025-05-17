using MediatR;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Diary.Commands.Create
{
    public class CreateDiaryCommandHandler : IRequestHandler<CreateDiaryCommand, CreateDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public CreateDiaryCommandHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<CreateDiaryResult> Handle(CreateDiaryCommand command, CancellationToken cancellationToken)
        {
            CreateDiaryDTO dto = new(command.userId, command.date);
            bool result = await _diaryService.CreateAsync(dto, cancellationToken);
            CreateDiaryResult response = new(result);
            return response;
        }
    }
}
