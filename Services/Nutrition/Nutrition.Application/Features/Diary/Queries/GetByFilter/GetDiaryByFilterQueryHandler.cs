using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Diary.Queries.GetByFilter
{
    public class GetDiaryByFilterQueryHandler : IQueryHandler<GetDiaryByFilterQuery, GetDiaryByFilterResult>
    {
        private readonly IDiaryService _diaryService;

        public GetDiaryByFilterQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<Result<GetDiaryByFilterResult>> Handle(GetDiaryByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _diaryService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetDiaryByFilterResult>(result.Error!);

            return Result.Success(new GetDiaryByFilterResult(result.Value.Items, result.Value.Pagination!));
        }
    }
}
