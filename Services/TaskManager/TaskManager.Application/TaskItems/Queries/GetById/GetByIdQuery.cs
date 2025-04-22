using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskItems.Queries.GetById
{
    public record GetByIdQuery(int Id) : IRequest<GetByIdResponse>;
    public record GetByIdResponse(TaskItemDTO TaskItem);
}