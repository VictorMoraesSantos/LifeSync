using BuildingBlocks.CQRS.Queries;
using TaskManager.Application.DTOs.TaskItem;

namespace TaskManager.Application.Features.TaskItems.Queries.GetAll
{
    public record GetAllTaskItemsQuery() : IQuery<GetAllTaskItemsResult>;
    public record GetAllTaskItemsResult(IEnumerable<TaskItemDTO> TaskItems);
}