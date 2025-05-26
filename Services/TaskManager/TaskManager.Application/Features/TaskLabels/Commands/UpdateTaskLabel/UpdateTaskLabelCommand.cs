using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.TaskLabels.Commands.UpdateTaskLabel
{
    public record UpdateTaskLabelCommand(int Id, string Name, LabelColor LabelColor)
        : IRequest<UpdateTaskLabelResult>;
    public record UpdateTaskLabelResult(bool IsUpdated);
}
