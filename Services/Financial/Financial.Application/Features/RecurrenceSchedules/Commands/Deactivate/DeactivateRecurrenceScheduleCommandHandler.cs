using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Deactivate
{
    public class DeactivateRecurrenceScheduleCommandHandler : ICommandHandler<DeactivateRecurrenceScheduleCommand, DeactivateRecurrenceScheduleResult>
    {
        private readonly IRecurrenceScheduleService _recurrenceScheduleService;

        public DeactivateRecurrenceScheduleCommandHandler(IRecurrenceScheduleService recurrenceScheduleService)
        {
            _recurrenceScheduleService = recurrenceScheduleService;
        }

        public async Task<Result<DeactivateRecurrenceScheduleResult>> Handle(DeactivateRecurrenceScheduleCommand command, CancellationToken cancellationToken)
        {
            var result = await _recurrenceScheduleService.DeactiveScheduleAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeactivateRecurrenceScheduleResult>(result.Error!);

            return Result.Success(new DeactivateRecurrenceScheduleResult(result.Value));
        }
    }
}
