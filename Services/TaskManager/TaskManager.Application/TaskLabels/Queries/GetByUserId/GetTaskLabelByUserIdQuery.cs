using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskLabels.Queries.GetByUserId
{
    public record GetTaskLabelByUserIdQuery(int UserId) : IRequest<GetTaskLabelByUserIdResponse>;
    public record GetTaskLabelByUserIdResponse(IEnumerable<TaskLabelDTO>? TaskLabelDTOs);
}
