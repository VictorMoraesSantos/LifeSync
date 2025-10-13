using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;
using Gym.Application.DTOs.TrainingSession;

namespace Gym.Application.Features.TrainingSession.Commands.Create
{
    public class CreateTrainingSessionCommandHandler : ICommandHandler<CreateTrainingSessionCommand, CreateTrainingSessionResult>
    {
        private readonly ITrainingSessionService _trainingSessionService;
        public CreateTrainingSessionCommandHandler(ITrainingSessionService trainingSessionService)
        {
            _trainingSessionService = trainingSessionService;
        }
        public async Task<Result<CreateTrainingSessionResult>> Handle(CreateTrainingSessionCommand command, CancellationToken cancellationToken)
        {
            var dto = new CreateTrainingSessionDTO(
                command.UserId,
                command.RoutineId,
                command.StartTime,
                command.EndTime);

            var result = await _trainingSessionService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CreateTrainingSessionResult>(result.Error!);

            return Result.Success(new CreateTrainingSessionResult(result.Value!));
        }
    }
}
