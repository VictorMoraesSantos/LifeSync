using MediatR;
using Nutrition.Application.DTOs.Diaries;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Meals.Queries.GetByDiary
{
    public class GetByDiaryQueryHandler : IRequestHandler<GetByDiaryQuery, GetByDiaryResponse>
    {
        private readonly IDiaryService _diaryService;

        public GetByDiaryQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<GetByDiaryResponse> Handle(GetByDiaryQuery query, CancellationToken cancellationToken)
        {
            DiaryDTO? result = await _diaryService.GetByIdAsync(query.Id, cancellationToken);
            GetByDiaryResponse response = new(result.Meals);
            return response;
        }
    }
}
