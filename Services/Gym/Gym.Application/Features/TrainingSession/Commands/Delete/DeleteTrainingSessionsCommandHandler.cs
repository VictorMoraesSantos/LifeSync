using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.TrainingSession.Commands.Delete
{
    public class DeleteTrainingSessionsCommandHandler : ICommandHandler<DeleteTrainingSessionsCommand, DeleteTrainingSessionsResult>
    {
        private readonly ITrainingSessionService _trainingSessionService;

        public DeleteTrainingSessionsCommandHandler(ITrainingSessionService trainingSessionService)
        {
            _trainingSessionService = trainingSessionService;
        }

        public async Task<Result<DeleteTrainingSessionsResult>> Handle(DeleteTrainingSessionsCommand command, CancellationToken cancellationToken)
        {
            var result = await _trainingSessionService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteTrainingSessionsResult>(result.Error!);

            return Result.Success(new DeleteTrainingSessionsResult(result.Value!));
        }
    }
}
