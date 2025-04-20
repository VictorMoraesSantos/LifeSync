using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.TaskItems.Queries.GetByStatus
{
    public record GetByStatusQuery(int UserId, Status Status) : IRequest<IEnumerable<TaskItemDTO>>;
}
