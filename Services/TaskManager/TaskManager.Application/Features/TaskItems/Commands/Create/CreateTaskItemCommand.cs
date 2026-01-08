using BuildingBlocks.CQRS.Commands;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.TaskItems.Commands.Create
{
    public record CreateTaskItemCommand(
        string Title,
        string Description,
        Priority Priority,
        DateOnly DueDate,
        int UserId,
        List<int>? TaskLabelsId = null
    ) : ICommand<CreateTaskItemResult>;

    public record CreateTaskItemResult(int Id);
}
