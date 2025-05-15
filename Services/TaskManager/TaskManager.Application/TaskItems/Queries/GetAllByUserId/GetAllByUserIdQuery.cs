using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskItems.Queries.GetAllByUserId
{
    public record GetAllByUserIdQuery(int UserId) : IRequest<GetAllByUserIdResult>;
    public record GetAllByUserIdResult(IEnumerable<TaskItemDTO> TaskItems);
}