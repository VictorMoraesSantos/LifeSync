using MediatR;

namespace TaskManager.Application.TaskItems.Commands.DeleteTaskItem
{
    public record DeleteTaskItemCommand(int Id) : IRequest<DeleteTaskItemResult>;
    public record DeleteTaskItemResult(bool IsDeleted);
}