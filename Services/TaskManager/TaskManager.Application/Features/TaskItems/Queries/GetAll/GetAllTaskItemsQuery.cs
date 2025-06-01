using BuildingBlocks.CQRS.Request;
using TaskManager.Application.DTOs.TaskItem;

namespace TaskManager.Application.Features.TaskItems.Queries.GetAll
{
    public record GetAllTaskItemsQuery() : IRequest<GetAllTaskItemsResult>;
    public record GetAllTaskItemsResult(IEnumerable<TaskItemDTO> TaskItems);
}