using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Diary.Commands.Create
{
    public class CreateDiaryCommandHandler : ICommandHandler<CreateDiaryCommand, CreateDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public CreateDiaryCommandHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<Result<CreateDiaryResult>> Handle(CreateDiaryCommand command, CancellationToken cancellationToken)
        {
            CreateDiaryDTO dto = new(command.userId, command.date);

            var result = await _diaryService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateDiaryResult>(result.Error!);

            return Result.Success(new CreateDiaryResult(result.Value!));
        }
    }
}
