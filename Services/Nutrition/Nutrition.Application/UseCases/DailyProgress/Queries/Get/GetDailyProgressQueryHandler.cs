using MediatR;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;

namespace Nutrition.Application.UseCases.DailyProgress.Queries.Get
{
    public class GetDailyProgressQueryHandler : IRequestHandler<GetDailyProgressQuery, GetDailyProgressResult>
    {
        private readonly IDailyProgressService _dailyProgressService;

        public GetDailyProgressQueryHandler(IDailyProgressService dailyProgressService)
        {
            _dailyProgressService = dailyProgressService;
        }

        public async Task<GetDailyProgressResult> Handle(GetDailyProgressQuery query, CancellationToken cancellationToken)
        {
            DailyProgressDTO? result = await _dailyProgressService.GetByIdAsync(query.Id, cancellationToken);
            GetDailyProgressResult response = new(result);
            return response;
        }
    }
}
