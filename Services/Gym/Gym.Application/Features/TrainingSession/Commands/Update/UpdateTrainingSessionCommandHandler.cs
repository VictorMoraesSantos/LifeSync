using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.TrainingSession;

namespace Gym.Application.Features.TrainingSession.Commands.Update
{
    public class UpdateTrainingSessionCommandHandler : ICommandHandler<UpdateTrainingSessionCommand, UpdateTrainingSessionResult>
    {
        private readonly ITrainingSessionService _trainingSessionService;

        public UpdateTrainingSessionCommandHandler(ITrainingSessionService trainingSessionService)
        {
            _trainingSessionService = trainingSessionService;
        }

        public async Task<Result<UpdateTrainingSessionResult>> Handle(UpdateTrainingSessionCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateTrainingSessionDTO(
                command.Id,
                command.RoutineId,
                command.StartTime,
                command.EndTime,
                command.Notes);

            var result = await _trainingSessionService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateTrainingSessionResult>(result.Error!);

            return Result.Success(new UpdateTrainingSessionResult(result.Value!));
        }
    }
}
