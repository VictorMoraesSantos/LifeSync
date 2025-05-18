using MediatR;
using TaskManager.Application.DTOs.TaskLabel;

namespace TaskManager.Application.TaskLabels.Queries.GetByUserId
{
    public record GetTaskLabelByUserIdQuery(int UserId) : IRequest<GetTaskLabelByUserIdResult>;
    public record GetTaskLabelByUserIdResult(IEnumerable<TaskLabelDTO>? TaskLabelDTOs);
}
