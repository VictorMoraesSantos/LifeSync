using BuildingBlocks.CQRS.Request;
using TaskManager.Application.DTOs.TaskItem;

namespace TaskManager.Application.Features.TaskItems.Queries.GetById
{
    public record GetTaskItemByIdQuery(int Id) : IRequest<GetTaskItemByIdResult>;
    public record GetTaskItemByIdResult(TaskItemDTO TaskItem);
}