using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.TaskLabels.Commands.UpdateTaskLabel
{
    public record UpdateTaskLabelCommand(
        int Id,
        string Name,
        LabelColor LabelColor,
        int UserId,
        int TaskItemId)
        : IRequest<bool>;
}
