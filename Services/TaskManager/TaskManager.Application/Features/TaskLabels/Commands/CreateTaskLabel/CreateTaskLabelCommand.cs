using BuildingBlocks.CQRS.Commands;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.TaskLabels.Commands.CreateTaskLabel
{
    public record CreateTaskLabelCommand(
        string Name,
        LabelColor LabelColor,
        int UserId,
        int TaskItemId)
        : ICommand<CreateTaskLabelResult>;
    public record CreateTaskLabelResult(int Id);
}
