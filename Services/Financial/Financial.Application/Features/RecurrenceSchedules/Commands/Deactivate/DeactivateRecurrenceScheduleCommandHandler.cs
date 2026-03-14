using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.Features.RecurrenceSchedules.Commands.Update;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Deactivate
{
    public class DeactivateRecurrenceScheduleCommandHandler : ICommandHandler<UpdateRecurrenceScheduleCommand, UpdateRecurrenceScheduleResult>
    {
        private readonly IRecurrenceScheduleService _recurrenceScheduleService;

        public DeactivateRecurrenceScheduleCommandHandler(IRecurrenceScheduleService recurrenceScheduleService)
        {
            _recurrenceScheduleService = recurrenceScheduleService;
        }

        public async Task<Result<UpdateRecurrenceScheduleResult>> Handle(UpdateRecurrenceScheduleCommand command, CancellationToken cancellationToken)
        {
            var result = await _recurrenceScheduleService.DeactiveScheduleAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateRecurrenceScheduleResult>(result.Error!);

            return Result.Success(new UpdateRecurrenceScheduleResult(result.Value));
        }
    }
}
