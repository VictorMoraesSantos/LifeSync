using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.Diary.Queries.GetByUser
{
    public class GetAllDiariesByUserIdQueryHandler : IQueryHandler<GetAllDiariesByUserIdQuery, GetAllDiariesByUserIdResult>
    {
        private readonly IDiaryService _diaryService;

        public GetAllDiariesByUserIdQueryHandler(IDiaryService diaryService)
        {
            _diaryService = diaryService;
        }

        public async Task<Result<GetAllDiariesByUserIdResult>> Handle(GetAllDiariesByUserIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _diaryService.GetAllByUserIdAsync(query.UserId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllDiariesByUserIdResult>(result.Error!);

            return Result.Success(new GetAllDiariesByUserIdResult(result.Value!));
        }
    }
}
