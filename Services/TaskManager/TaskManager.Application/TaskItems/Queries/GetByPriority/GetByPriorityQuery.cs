using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.TaskItems.Queries.GetByPriority
{
    public record GetByPriorityQuery(int UserId, Priority Priority) : IRequest<GetByPriorityResult>;
    public record GetByPriorityResult(IEnumerable<TaskItemDTO> TaskItems);
}
