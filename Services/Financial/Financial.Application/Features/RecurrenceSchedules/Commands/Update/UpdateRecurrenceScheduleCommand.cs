using BuildingBlocks.CQRS.Requests.Commands;
using Financial.Domain.Enums;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Update
{
    public record UpdateRecurrenceScheduleCommand(
        int Id,
        RecurrenceFrequency Frequency,
        DateTime? EndDate = null,
        int? MaxOccurrences = null)
        : ICommand<UpdateRecurrenceScheduleResult>;
    public record UpdateRecurrenceScheduleResult(bool IsSuccess);

}
