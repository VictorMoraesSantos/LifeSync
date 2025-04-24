using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskLabels.Queries.GetById
{
    public record GetTaskLabelByIdQuery(int Id) : IRequest<GetTaskLabelByIdResponse>;
    public record GetTaskLabelByIdResponse(TaskLabelDTO? TaskLabel);
}
