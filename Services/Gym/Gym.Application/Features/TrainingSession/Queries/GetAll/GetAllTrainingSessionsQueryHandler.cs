
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.TrainingSession.Queries.GetAll
{
    public class GetAllTrainingSessionsQueryHandler : IQueryHandler<GetAllTrainingSessionsQuery, GetAllTrainingSessionsResult>
    {
        private readonly ITrainingSessionService _trainingSessionService;

        public GetAllTrainingSessionsQueryHandler(ITrainingSessionService trainingSessionService)
        {
            _trainingSessionService = trainingSessionService;
        }

        public async Task<Result<GetAllTrainingSessionsResult>> Handle(GetAllTrainingSessionsQuery query, CancellationToken cancellationToken)
        {
            var result = await _trainingSessionService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllTrainingSessionsResult>(result.Error!);

            return Result.Success(new GetAllTrainingSessionsResult(result.Value!));
        }
    }
}
