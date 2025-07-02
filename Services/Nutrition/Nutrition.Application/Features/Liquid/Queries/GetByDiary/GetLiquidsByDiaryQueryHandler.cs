using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Liquid.Queries.GetByDiary
{
    public class GetLiquidsByDiaryQueryHandler : IQueryHandler<GetLiquidsByDiaryQuery, GetLiquidsByDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public GetLiquidsByDiaryQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<Result<GetLiquidsByDiaryResult>> Handle(GetLiquidsByDiaryQuery query, CancellationToken cancellationToken)
        {
            var result = await _diaryService.GetByIdAsync(query.DiaryId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetLiquidsByDiaryResult>(result.Error!);

            return Result.Success(new GetLiquidsByDiaryResult(result.Value!.Liquids));
        }
    }
}
