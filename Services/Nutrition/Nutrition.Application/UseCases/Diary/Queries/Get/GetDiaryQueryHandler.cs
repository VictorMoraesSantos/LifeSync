using MediatR;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Diary.Queries.Get
{
    public class GetDiaryQueryHandler : IRequestHandler<GetDiaryQuery, GetDiaryQueryResult>
    {
        private readonly IDiaryService _diaryService;
     
        public GetDiaryQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }
        
        public async Task<GetDiaryQueryResult> Handle(GetDiaryQuery query, CancellationToken cancellationToken)
        {
            DiaryDTO? result = await _diaryService.GetByIdAsync(query.Id, cancellationToken);
            GetDiaryQueryResult response = new(result);
            return response;
        }
    }
}
