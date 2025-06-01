using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Diary.Queries.Get
{
    public class GetDiaryQueryHandler : IRequestHandler<GetDiaryQuery, GetDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public GetDiaryQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<GetDiaryResult> Handle(GetDiaryQuery query, CancellationToken cancellationToken)
        {
            DiaryDTO? result = await _diaryService.GetByIdAsync(query.Id, cancellationToken);

            return new GetDiaryResult(result);
        }
    }
}
