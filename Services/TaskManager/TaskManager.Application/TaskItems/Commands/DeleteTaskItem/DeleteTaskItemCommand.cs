using MediatR;

namespace TaskManager.Application.TaskItems.Commands.DeleteTaskItem
{
    public record DeleteTaskItemCommand(int Id) : IRequest<DeleteTaskItemResponse>;
    public record DeleteTaskItemResponse(bool IsDeleted);
}