using MediatR;
using TaskManager.Application.DTOs.TaskItem;

namespace TaskManager.Application.TaskItems.Queries.GetById
{
    public record GetTaskItemByIdQuery(int Id) : IRequest<GetTaskItemByIdResult>;
    public record GetTaskItemByIdResult(TaskItemDTO TaskItem);
}