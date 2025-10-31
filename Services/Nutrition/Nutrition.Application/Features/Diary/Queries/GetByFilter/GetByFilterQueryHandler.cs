using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Diary.Queries.GetByFilter
{
    public class GetByFilterQueryHandler : IQueryHandler<GetByFilterQuery, GetByFilterResult>
    {
        private readonly IDiaryService _diaryService;

        public GetByFilterQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<Result<GetByFilterResult>> Handle(GetByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _diaryService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetByFilterResult>(result.Error!);

            return Result.Success(new GetByFilterResult(result.Value.Items, result.Value.Pagination!));
        }
    }
}
