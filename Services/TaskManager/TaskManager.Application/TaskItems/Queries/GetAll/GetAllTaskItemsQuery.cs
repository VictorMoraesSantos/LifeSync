using MediatR;
using TaskManager.Application.DTOs.TaskItem;

namespace TaskManager.Application.TaskItems.Queries.GetAll
{
    public record GetAllTaskItemsQuery() : IRequest<GetAllTaskItemsResult>;
    public record GetAllTaskItemsResult(IEnumerable<TaskItemDTO> TaskItems);
}