using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskItems.Queries.GetAll
{
    public record GetAllTaskItemsQuery() : IRequest<GetAllTaskItemsResult>;
    public record GetAllTaskItemsResult(IEnumerable<TaskItemDTO> TaskItems);
}