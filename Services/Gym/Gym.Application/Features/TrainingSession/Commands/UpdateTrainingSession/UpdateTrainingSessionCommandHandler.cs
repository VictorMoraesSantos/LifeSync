using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.TrainingSession;

namespace Gym.Application.Features.TrainingSession.Commands.UpdateTrainingSession
{
    public class UpdateTrainingSessionCommandHandler : ICommandHandler<UpdateTrainingSessionCommand, UpdateTrainingSessionResponse>
    {
        private readonly ITrainingSessionService _trainingSessionService;

        public UpdateTrainingSessionCommandHandler(ITrainingSessionService trainingSessionService)
        {
            _trainingSessionService = trainingSessionService;
        }

        public async Task<Result<UpdateTrainingSessionResponse>> Handle(UpdateTrainingSessionCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateTrainingSessionDTO(
                command.Id,
                command.RoutineId,
                command.StartTime,
                command.EndTime,
                command.Notes);

            var result = await _trainingSessionService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result<UpdateTrainingSessionResponse>.Failure(result.Error!);

            return Result<UpdateTrainingSessionResponse>.Success(new UpdateTrainingSessionResponse(result.Value!));
        }
    }
}
