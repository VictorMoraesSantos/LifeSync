using BuildingBlocks.CQRS.Request;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.Features.DailyProgress.Queries.GetByUser
{
    public class GetAllDailyProgressesByUserIdQueryHandler : IRequestHandler<GetAllDailyProgressesByUserIdQuery, GetAllDailyProgressesByUserIdResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public GetAllDailyProgressesByUserIdQueryHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<GetAllDailyProgressesByUserIdResult> Handle(GetAllDailyProgressesByUserIdQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<DailyProgressDTO> result = await _dailyProgressService.GetByUserIdAsync(query.UserId, cancellationToken);

            return new GetAllDailyProgressesByUserIdResult(result);
        }
    }
}
