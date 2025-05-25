using MediatR;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Diary.Queries.GetAll
{
    public class GetDiariesQueryHandler : IRequestHandler<GetDiariesQuery, GetDiariesResult>
    {
        private readonly IDiaryService _diaryService;

        public GetDiariesQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }
        
        public async Task<GetDiariesResult> Handle(GetDiariesQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<DiaryDTO> result = await _diaryService.GetAllAsync(cancellationToken);

            return new GetDiariesResult(result);
        }
    }
}
