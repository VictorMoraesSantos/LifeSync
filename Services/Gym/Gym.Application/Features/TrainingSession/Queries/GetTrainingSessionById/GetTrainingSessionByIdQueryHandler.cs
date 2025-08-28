using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.TrainingSession.Queries.GetTrainingSessionById
{
    public class GetTrainingSessionByIdQueryHandler : IQueryHandler<GetTrainingSessionByIdQuery, GetTrainingSessionByIdResult>
    {
        private readonly ITrainingSessionService _trainingSessionService;

        public GetTrainingSessionByIdQueryHandler(ITrainingSessionService trainingSessionService)
        {
            _trainingSessionService = trainingSessionService;
        }

        public async Task<Result<GetTrainingSessionByIdResult>> Handle(GetTrainingSessionByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _trainingSessionService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetTrainingSessionByIdResult>(result.Error!);

            return Result.Success(new GetTrainingSessionByIdResult(result.Value!));
        }
    }
}
