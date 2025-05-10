using MediatR;
using Nutrition.Application.DTOs.Diaries;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Diaries.Queries.GetByUser
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
            GetAllDiariesByUserIdResult response = new(result);
            return response;
        }
    }
}
