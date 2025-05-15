using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskLabels.Queries.GetById
{
    public record GetTaskLabelByIdQuery(int Id) : IRequest<GetTaskLabelByIdResult>;
    public record GetTaskLabelByIdResult(TaskLabelDTO? TaskLabel);
}
