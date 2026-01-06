using BuildingBlocks.CQRS.Commands;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.TaskLabels.Commands.Create
{
    public record CreateTaskLabelCommand(
        string Name,
        LabelColor LabelColor,
        int UserId)
        : ICommand<CreateTaskLabelResult>;
    public record CreateTaskLabelResult(int Id);
}
