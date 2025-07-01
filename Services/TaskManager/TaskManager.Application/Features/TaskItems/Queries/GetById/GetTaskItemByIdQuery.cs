using BuildingBlocks.CQRS.Queries;
using TaskManager.Application.DTOs.TaskItem;

namespace TaskManager.Application.Features.TaskItems.Queries.GetById
{
    public record GetTaskItemByIdQuery(int Id) : IQuery<GetTaskItemByIdResult>;
    public record GetTaskItemByIdResult(TaskItemDTO TaskItem);
}