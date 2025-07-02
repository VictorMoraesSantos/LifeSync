using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.DailyProgress.Queries.GetAll
{
    public class GetDailyProgressesQueryHandler : IQueryHandler<GetDailyProgressesQuery, GetDailyProgressesResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public GetDailyProgressesQueryHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<Result<GetDailyProgressesResult>> Handle(GetDailyProgressesQuery query, CancellationToken cancellationToken)
        {
            var result = await _dailyProgressService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetDailyProgressesResult>(result.Error!);

            return Result.Success(new GetDailyProgressesResult(result.Value!));
        }
    }
}
