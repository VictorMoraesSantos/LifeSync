using MediatR;
using TaskManager.Application.DTOs.TaskLabel;

namespace TaskManager.Application.TaskLabels.Queries.GetByName
{
    public record GetTaskLabelByNameQuery(int UserId, string Name) : IRequest<GetTaskLabelByNameResult>;
    public record GetTaskLabelByNameResult(IEnumerable<TaskLabelDTO>? TaskLabelDTOs);
}
