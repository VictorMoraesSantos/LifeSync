using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Meal.Queries.GetByDiary
{
    public class GetByDiaryQueryHandler : IRequestHandler<GetByDiaryQuery, GetByDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public GetByDiaryQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<GetByDiaryResult> Handle(GetByDiaryQuery query, CancellationToken cancellationToken)
        {
            DiaryDTO? result = await _diaryService.GetByIdAsync(query.Id, cancellationToken);

            return new GetByDiaryResult(result.Meals);
        }
    }
}
