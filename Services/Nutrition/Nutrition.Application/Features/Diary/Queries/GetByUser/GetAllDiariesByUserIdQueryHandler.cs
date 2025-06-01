using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Diary.Queries.GetByUser
{
    public class GetAllDiariesByUserIdQueryHandler : IRequestHandler<GetAllDiariesByUserIdQuery, GetAllDiariesByUserIdResult>
    {
        private readonly IDiaryService _diaryService;

        public GetAllDiariesByUserIdQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<GetAllDiariesByUserIdResult> Handle(GetAllDiariesByUserIdQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<DiaryDTO> result = await _diaryService.GetAllByUserIdAsync(query.UserId, cancellationToken);

            return new GetAllDiariesByUserIdResult(result);
        }
    }
}
