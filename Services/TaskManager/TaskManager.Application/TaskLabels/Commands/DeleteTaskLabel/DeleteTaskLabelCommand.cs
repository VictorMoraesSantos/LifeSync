using MediatR;

namespace TaskManager.Application.TaskLabels.Commands.DeleteTaskLabel
{
    public record DeleteTaskLabelCommand(int Id) : IRequest<bool>;
}
