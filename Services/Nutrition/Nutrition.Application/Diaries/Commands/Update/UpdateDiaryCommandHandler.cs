using MediatR;
using Nutrition.Application.DTOs.Diaries;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Diaries.Commands.UpdateDiary
{
    public class UpdateDiaryCommandHandler : IRequestHandler<UpdateDiaryCommand, UpdateDiaryCommandResult>
    {
        private readonly IDiaryService _diaryService;

        public UpdateDiaryCommandHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<UpdateDiaryCommandResult> Handle(UpdateDiaryCommand command, CancellationToken cancellationToken)
        {
            UpdateDiaryDTO dto = new(command.Id, command.Date);
            bool result = await _diaryService.UpdateAsync(dto, cancellationToken);
            UpdateDiaryCommandResult response = new(result);
            return response;
        }
    }
}
