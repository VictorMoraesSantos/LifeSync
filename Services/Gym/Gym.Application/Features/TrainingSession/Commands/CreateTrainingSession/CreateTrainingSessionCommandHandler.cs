using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.TrainingSession;

namespace Gym.Application.Features.TrainingSession.Commands.CreateTrainingSession
{
    public class CreateTrainingSessionCommandHandler : ICommandHandler<CreateTrainingSessionCommand, CreateTrainingSessionResponse>
    {
        private readonly ITrainingSessionService _trainingSessionService;
        public CreateTrainingSessionCommandHandler(ITrainingSessionService trainingSessionService)
        {
            _trainingSessionService = trainingSessionService;
        }
        public async Task<Result<CreateTrainingSessionResponse>> Handle(CreateTrainingSessionCommand command, CancellationToken cancellationToken)
        {
            var dto = new CreateTrainingSessionDTO(
                command.UserId,
                command.RoutineId,
                command.StartTime,
                command.EndTime);

            var result = await _trainingSessionService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result<CreateTrainingSessionResponse>.Failure(result.Error!);

            return Result<CreateTrainingSessionResponse>.Success(new CreateTrainingSessionResponse(result.Value!));
        }
    }
}
