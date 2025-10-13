using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.TrainingSession.Queries.GetByUserId
{
    public class GetTrainingSessionsByUserIdQueryHandler : IQueryHandler<GetTrainingSessionsByUserIdQuery, GetTrainingSessionsByUserIdResult>
    {
        private readonly ITrainingSessionService _trainingSessionService;

        public GetTrainingSessionsByUserIdQueryHandler(ITrainingSessionService trainingSessionService)
        {
            _trainingSessionService = trainingSessionService;
        }

        public async Task<Result<GetTrainingSessionsByUserIdResult>> Handle(GetTrainingSessionsByUserIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _trainingSessionService.GetByUserIdAsync(query.UserId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetTrainingSessionsByUserIdResult>(result.Error!);

            return Result.Success(new GetTrainingSessionsByUserIdResult(result.Value!));
        }
    }
}
