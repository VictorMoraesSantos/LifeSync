using MediatR;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.DailyProgress.Queries.GetAll
{
    public class GetDailyProgressesQueryHandler : IRequestHandler<GetDailyProgressesQuery, GetDailyProgressesResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public GetDailyProgressesQueryHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<GetDailyProgressesResult> Handle(GetDailyProgressesQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<DailyProgressDTO> result = await _dailyProgressService.GetAllAsync(cancellationToken);
            GetDailyProgressesResult response = new(result);
            return response;
        }
    }
}
