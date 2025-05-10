using MediatR;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Diaries.Commands.DeleteDiary
{
    public class DeleteDiaryCommandHandler : IRequestHandler<DeleteDiaryCommand, DeleteDiaryCommandResult>
    {
        private readonly IDiaryService _diaryService;

        public DeleteDiaryCommandHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<DeleteDiaryCommandResult> Handle(DeleteDiaryCommand command, CancellationToken cancellationToken)
        {
            bool result = await _diaryService.DeleteAsync(command.Id);
            DeleteDiaryCommandResult response = new(true);
            return response;
        }
    }
}
