using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskItems.Queries.GetAll
{
    public record GetAllTaskItemsQuery() : IRequest<GetAllTaskItemsResponse>;
    public record GetAllTaskItemsResponse(IEnumerable<TaskItemDTO> TaskItems);
}