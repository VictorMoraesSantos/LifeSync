using MediatR;
using Nutrition.Application.DTOs.Diaries;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Diaries.Commands.CreateDiary
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
            CreateDiaryDTO diaryDTO = new(command.userId, command.date);
            var result = await _diaryService.CreateAsync(diaryDTO, cancellationToken);
            throw new NotImplementedException();
        }
    }
}
