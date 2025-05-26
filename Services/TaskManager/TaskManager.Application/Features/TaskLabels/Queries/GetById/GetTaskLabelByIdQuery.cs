using MediatR;
using TaskManager.Application.DTOs.TaskLabel;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetById
{
    public record GetTaskLabelByIdQuery(int Id) : IRequest<GetTaskLabelByIdResult>;
    public record GetTaskLabelByIdResult(TaskLabelDTO? TaskLabel);
}
