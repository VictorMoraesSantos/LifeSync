using MediatR;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.DTOs.Liquid;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.Liquid.Queries.GetByDiary
{
    public class GetLiquidsByDiaryQueryHandler : IRequestHandler<GetLiquidsByDiaryQuery, GetLiquidsByDiaryResult>
    {
        private readonly IDiaryService _diaryService;

        public GetLiquidsByDiaryQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<GetLiquidsByDiaryResult> Handle(GetLiquidsByDiaryQuery query, CancellationToken cancellationToken)
        {
            DiaryDTO? result = await _diaryService.GetByIdAsync(query.DiaryId, cancellationToken);
            GetLiquidsByDiaryResult response = new GetLiquidsByDiaryResult(result.Liquids);
            return response;
        }
    }
}
