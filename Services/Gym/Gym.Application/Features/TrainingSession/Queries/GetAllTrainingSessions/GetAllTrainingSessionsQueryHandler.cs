using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.TrainingSession.Queries.GetAllTrainingSessions
{
    public class GetAllTrainingSessionsQueryHandler : IQueryHandler<GetAllTrainingSessionsQuery, GetAllTrainingSessionsResponse>
    {
        private readonly ITrainingSessionService _trainingSessionService;

        public GetAllTrainingSessionsQueryHandler(ITrainingSessionService trainingSessionService)
        {
            _trainingSessionService = trainingSessionService;
        }

        public async Task<Result<GetAllTrainingSessionsResponse>> Handle(GetAllTrainingSessionsQuery query, CancellationToken cancellationToken)
        {
            var result = await _trainingSessionService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result<GetAllTrainingSessionsResponse>.Failure(result.Error!);

            return Result.Success(new GetAllTrainingSessionsResponse(result.Value!));
        }
    }
}
