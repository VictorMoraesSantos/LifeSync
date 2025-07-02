using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.DailyProgress.Queries.GetByUser
{
    public class GetAllDailyProgressesByUserIdQueryHandler : IQueryHandler<GetAllDailyProgressesByUserIdQuery, GetAllDailyProgressesByUserIdResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public GetAllDailyProgressesByUserIdQueryHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<Result<GetAllDailyProgressesByUserIdResult>> Handle(GetAllDailyProgressesByUserIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _dailyProgressService.GetByUserIdAsync(query.UserId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllDailyProgressesByUserIdResult>(result.Error!);

            return Result.Success(new GetAllDailyProgressesByUserIdResult(result.Value!));
        }
    }
}
