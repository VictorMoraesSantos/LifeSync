
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Diary.Commands.Delete
{
    public class DeleteDiaryCommandHandler : ICommandHandler<DeleteDiaryCommand, DeleteDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public DeleteDiaryCommandHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<Result<DeleteDiaryResult>> Handle(DeleteDiaryCommand command, CancellationToken cancellationToken)
        {
            var result = await _diaryService.DeleteAsync(command.Id);
            if (!result.IsSuccess)
                return Result.Failure<DeleteDiaryResult>(result.Error!);

            return Result.Success(new DeleteDiaryResult(result.Value!));
        }
    }
}
