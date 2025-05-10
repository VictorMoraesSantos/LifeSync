using MediatR;
using Nutrition.Application.DTOs.Diaries;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Diaries.Queries.GetAll
{
    public class GetDiariesQueryHandler : IRequestHandler<GetDiariesQuery, GetDiariesQueryResult>
    {
        private readonly IDiaryService _diaryService;

        public GetDiariesQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }
        
        public async Task<GetDiariesQueryResult> Handle(GetDiariesQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<DiaryDTO> result = await _diaryService.GetAllAsync(cancellationToken);
            GetDiariesQueryResult response = new(result);
            return response;
        }
    }
}
