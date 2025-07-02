using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Diary.Queries.Get
{
    public class GetDiaryQueryHandler : IQueryHandler<GetDiaryQuery, GetDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public GetDiaryQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<Result<GetDiaryResult>> Handle(GetDiaryQuery query, CancellationToken cancellationToken)
        {
            var result = await _diaryService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetDiaryResult>(result.Error!);

            return Result.Success(new GetDiaryResult(result.Value!));
        }
    }
}
