using BuildingBlocks.CQRS.Requests.Commands;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Deactivate
{
    public record DeactivateRecurrenceScheduleCommand(int Id) : ICommand<DeactivateRecurrenceScheduleResult>;
    public record DeactivateRecurrenceScheduleResult(bool IsSuccess);
}
