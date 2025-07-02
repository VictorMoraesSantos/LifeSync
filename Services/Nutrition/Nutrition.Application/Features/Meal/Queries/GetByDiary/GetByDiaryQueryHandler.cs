using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Queries.GetByDiary
{
    public class GetByDiaryQueryHandler : IQueryHandler<GetByDiaryQuery, GetByDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public GetByDiaryQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<Result<GetByDiaryResult>> Handle(GetByDiaryQuery query, CancellationToken cancellationToken)
        {
            var result = await _diaryService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetByDiaryResult>(result.Error!);

            return Result.Success(new GetByDiaryResult(result.Value!.Meals));
        }
    }
}
