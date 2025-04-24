using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskLabels.Queries.GetByName
{
    public record GetTaskLabelByNameQuery(int UserId, string Name) : IRequest<GetTaskLabelByNameResponse>;
    public record GetTaskLabelByNameResponse(IEnumerable<TaskLabelDTO>? TaskLabelDTOs);
}
