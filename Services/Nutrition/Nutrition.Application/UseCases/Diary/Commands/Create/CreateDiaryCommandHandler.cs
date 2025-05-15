using MediatR;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Diary.Commands.Create
{
    public class CreateDiaryCommandHandler : IRequestHandler<CreateDiaryCommand, CreateDiaryCommandResult>
    {
        private readonly IDiaryService _diaryService;

        public CreateDiaryCommandHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<CreateDiaryCommandResult> Handle(CreateDiaryCommand command, CancellationToken cancellationToken)
        {
            CreateDiaryDTO dto = new(command.userId, command.date);
            bool result = await _diaryService.CreateAsync(dto, cancellationToken);
            CreateDiaryCommandResult response = new(result);
            return response;
        }
    }
}
