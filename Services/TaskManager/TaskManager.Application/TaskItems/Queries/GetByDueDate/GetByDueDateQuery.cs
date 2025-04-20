using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskItems.Queries.GetByDueDate
{
    public record GetByDueDateQuery(int UserId, DateOnly DueDate) : IRequest<IEnumerable<TaskItemDTO>>;
}