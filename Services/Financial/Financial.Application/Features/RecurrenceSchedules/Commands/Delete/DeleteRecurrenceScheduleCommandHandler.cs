using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.RecurrenceSchedules.Commands.Delete
{
    public class DeleteRecurrenceScheduleCommandHandler : ICommandHandler<DeleteRecurrenceScheduleCommand, DeleteRecurrenceScheduleResult>
    {
        private readonly IRecurrenceScheduleService _recurrenceScheduleService;

        public DeleteRecurrenceScheduleCommandHandler(IRecurrenceScheduleService recurrenceScheduleService)
        {
            _recurrenceScheduleService = recurrenceScheduleService;
        }

        public async Task<Result<DeleteRecurrenceScheduleResult>> Handle(DeleteRecurrenceScheduleCommand command, CancellationToken cancellationToken)
        {
            var result = await _recurrenceScheduleService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteRecurrenceScheduleResult>(result.Error);

            return Result.Success(new DeleteRecurrenceScheduleResult(result.Value));
        }
    }
}
