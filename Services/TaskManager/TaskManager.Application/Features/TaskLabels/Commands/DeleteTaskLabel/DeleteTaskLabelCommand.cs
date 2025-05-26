using MediatR;

namespace TaskManager.Application.Features.TaskLabels.Commands.DeleteTaskLabel
{
    public record DeleteTaskLabelCommand(int Id) : IRequest<DeleteTaskLabelResult>;
    public record DeleteTaskLabelResult(bool IsDeleted);
}
