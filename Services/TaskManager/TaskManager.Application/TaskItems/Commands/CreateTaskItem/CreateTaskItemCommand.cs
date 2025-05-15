using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.TaskItems.Commands.CreateTaskItem
{
    public record CreateTaskItemCommand(
        string Title,
        string Description,
        Priority Priority,
        DateOnly DueDate,
        int UserId
    ) : IRequest<CreateTaskItemResult>;

    public record CreateTaskItemResult(int TaskId);
}
