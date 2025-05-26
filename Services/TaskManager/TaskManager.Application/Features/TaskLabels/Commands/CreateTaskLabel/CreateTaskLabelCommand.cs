using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.TaskLabels.Commands.CreateTaskLabel
{
    public record CreateTaskLabelCommand(string Name, LabelColor LabelColor, int UserId, int TaskItemId)
        : IRequest<CreateTaskLabelResult>;
    public record CreateTaskLabelResult(int Id);
}
