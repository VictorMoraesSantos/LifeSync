using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Diary.Commands.Update
{
    public class UpdateDiaryCommandHandler : ICommandHandler<UpdateDiaryCommand, UpdateDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public UpdateDiaryCommandHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<Result<UpdateDiaryResult>> Handle(UpdateDiaryCommand command, CancellationToken cancellationToken)
        {
            UpdateDiaryDTO dto = new(command.Id, command.Date);

            var result = await _diaryService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateDiaryResult>(result.Error!);

            return Result.Success(new UpdateDiaryResult(result.Value!));
        }
    }
}
