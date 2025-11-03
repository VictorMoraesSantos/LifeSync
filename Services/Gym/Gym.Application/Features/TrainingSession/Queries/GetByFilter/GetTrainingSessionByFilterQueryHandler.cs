using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.TrainingSession.Queries.GetByFilter
{
    public class GetTrainingSessionByFilterQueryHandler : IQueryHandler<GetTrainingSessionByFilterQuery, GetTrainingSessionByFilterResult>
    {
        private readonly ITrainingSessionService _trainingSessionService;

        public GetTrainingSessionByFilterQueryHandler(ITrainingSessionService trainingSessionService)
        {
            _trainingSessionService = trainingSessionService;
        }

        public async Task<Result<GetTrainingSessionByFilterResult>> Handle(GetTrainingSessionByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _trainingSessionService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetTrainingSessionByFilterResult>(result.Error);

            return Result.Success(new GetTrainingSessionByFilterResult(result.Value.Items, result.Value.Pagination));
        }
    }
}
