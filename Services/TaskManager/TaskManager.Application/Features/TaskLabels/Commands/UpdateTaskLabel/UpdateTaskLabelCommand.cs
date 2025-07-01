using BuildingBlocks.CQRS.Commands;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.TaskLabels.Commands.UpdateTaskLabel
{
    public record UpdateTaskLabelCommand(
        int Id,
        string Name,
        LabelColor LabelColor)
        : ICommand<UpdateTaskLabelResult>;
    public record UpdateTaskLabelResult(bool IsUpdated);
}
