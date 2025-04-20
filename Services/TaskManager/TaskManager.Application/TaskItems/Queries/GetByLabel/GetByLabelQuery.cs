using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskItems.Queries.GetByLabel
{
    public record GetByLabelQuery(int UserId, TaskLabelDTO Label) : IRequest<IEnumerable<TaskItemDTO>>;
}
