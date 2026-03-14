using BuildingBlocks.CQRS.Requests.Commands;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Delete
{
    public record DeleteRecurrenceScheduleCommand(int Id) : ICommand<DeleteRecurrenceScheduleResult>;
    public record DeleteRecurrenceScheduleResult(bool IsSuccess);
}
