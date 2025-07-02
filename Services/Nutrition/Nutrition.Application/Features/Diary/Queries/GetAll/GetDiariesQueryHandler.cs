using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Diary.Queries.GetAll
{
    public class GetDiariesQueryHandler : IQueryHandler<GetDiariesQuery, GetDiariesResult>
    {
        private readonly IDiaryService _diaryService;

        public GetDiariesQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<Result<GetDiariesResult>> Handle(GetDiariesQuery query, CancellationToken cancellationToken)
        {
            var result = await _diaryService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetDiariesResult>(result.Error!);

            return Result.Success(new GetDiariesResult(result.Value!));
        }
    }
}
