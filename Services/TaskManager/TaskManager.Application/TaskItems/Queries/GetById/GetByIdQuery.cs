using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskItems.Queries.GetById
{
    public record GetByIdQuery(int Id) : IRequest<GetByIdResult>;
    public record GetByIdResult(TaskItemDTO TaskItem);
}