using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.DailyProgress.Queries.GetById
{
    public class GetDailyProgressQueryHandler : IQueryHandler<GetDailyProgressQuery, GetDailyProgressResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public GetDailyProgressQueryHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<Result<GetDailyProgressResult>> Handle(GetDailyProgressQuery query, CancellationToken cancellationToken)
        {
            var result = await _dailyProgressService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetDailyProgressResult>(result.Error!);

            return Result.Success(new GetDailyProgressResult(result.Value!));
        }
    }
}
