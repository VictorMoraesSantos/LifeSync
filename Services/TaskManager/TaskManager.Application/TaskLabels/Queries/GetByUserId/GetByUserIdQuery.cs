using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskLabels.Queries.GetByUserId
{
    public record GetByUserIdQuery(int UserId) : IRequest<IEnumerable<TaskLabelDTO>?>;
}
