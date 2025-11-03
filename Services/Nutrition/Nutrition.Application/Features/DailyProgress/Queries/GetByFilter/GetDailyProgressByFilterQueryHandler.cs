using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.DailyProgress.Queries.GetByFilter
{
    public class GetDailyProgressByFilterQueryHandler : IQueryHandler<GetDailyProgressByFilterQuery, GetDailyProgressByFilterResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public GetDailyProgressByFilterQueryHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<Result<GetDailyProgressByFilterResult>> Handle(GetDailyProgressByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _dailyProgressService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetDailyProgressByFilterResult>(result.Error!);

            return Result.Success(new GetDailyProgressByFilterResult(result.Value.Items, result.Value.Pagination!));
        }
    }
}
