using BuildingBlocks.CQRS.Commands;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.TaskItems.Commands.Update
{
    public record UpdateTaskItemCommand(
        int Id,
        string Title,
        string Description,
        Status Status,
        Priority Priority,
        DateOnly DueDate
    ) : ICommand<UpdateTaskItemCommandResult>;

    public record UpdateTaskItemCommandResult(bool IsUpdated);
}