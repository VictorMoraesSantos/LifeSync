using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.TrainingSession.Commands.DeleteTrainingSessions
{
    public class DeleteTrainingSessionsCommandHandler : ICommandHandler<DeleteTrainingSessionsCommand, DeleteTrainingSessionsResponse>
    {
        private readonly ITrainingSessionService _trainingSessionService;

        public DeleteTrainingSessionsCommandHandler(ITrainingSessionService trainingSessionService)
        {
            _trainingSessionService = trainingSessionService;
        }

        public async Task<Result<DeleteTrainingSessionsResponse>> Handle(DeleteTrainingSessionsCommand command, CancellationToken cancellationToken)
        {
            var result = await _trainingSessionService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result<DeleteTrainingSessionsResponse>.Failure(result.Error!);

            return Result<DeleteTrainingSessionsResponse>.Success(new DeleteTrainingSessionsResponse(result.Value!));
        }
    }
}
