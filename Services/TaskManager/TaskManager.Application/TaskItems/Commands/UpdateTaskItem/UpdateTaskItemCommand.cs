using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.TaskItems.Commands.UpdateTaskItem
{
    public record UpdateTaskItemCommand(
        int Id,
        string Title,
        string Description,
        Status Status,
        Priority Priority,
        DateOnly DueDate
    ) : IRequest<UpdateTaskItemCommandResult>;
    public record UpdateTaskItemCommandResult(bool IsUpdated);
}