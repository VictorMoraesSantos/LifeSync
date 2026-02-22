using BuildingBlocks.CQRS.Requests.Commands;

namespace TaskManager.Application.Features.TaskLabels.Commands.Delete
{
    public record DeleteTaskLabelCommand(int Id) : ICommand<DeleteTaskLabelResult>;
    public record DeleteTaskLabelResult(bool IsDeleted);
}
