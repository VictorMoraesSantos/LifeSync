using MediatR;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Diary.Commands.Delete
{
    public class DeleteDiaryCommandHandler : IRequestHandler<DeleteDiaryCommand, DeleteDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public DeleteDiaryCommandHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<DeleteDiaryResult> Handle(DeleteDiaryCommand command, CancellationToken cancellationToken)
        {
            bool result = await _diaryService.DeleteAsync(command.Id);

            return new DeleteDiaryResult(result);
        }
    }
}
