using BuildingBlocks.CQRS.Commands;

namespace TaskManager.Application.Features.TaskItems.Commands.Delete
{
    public record DeleteTaskItemCommand(int Id) : ICommand<DeleteTaskItemResult>;
    public record DeleteTaskItemResult(bool IsDeleted);
}