using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Update
{
    public class UpdateRecurrenceScheduleCommandHandler : ICommandHandler<UpdateRecurrenceScheduleCommand, UpdateRecurrenceScheduleResult>
    {
        private readonly IRecurrenceScheduleService _recurrenceScheduleService;

        public UpdateRecurrenceScheduleCommandHandler(IRecurrenceScheduleService recurrenceScheduleService)
        {
            _recurrenceScheduleService = recurrenceScheduleService;
        }

        public async Task<Result<UpdateRecurrenceScheduleResult>> Handle(UpdateRecurrenceScheduleCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateRecurrenceScheduleDTO(
                command.Id,
                command.Frequency,
                command.EndDate,
                command.MaxOccurrences);

            var result = await _recurrenceScheduleService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateRecurrenceScheduleResult>(result.Error!);

            return Result.Success(new UpdateRecurrenceScheduleResult(result.Value));
        }
    }
}
