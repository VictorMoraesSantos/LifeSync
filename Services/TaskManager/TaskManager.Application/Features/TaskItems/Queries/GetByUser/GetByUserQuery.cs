using BuildingBlocks.CQRS.Queries;
using TaskManager.Application.DTOs.TaskItem;

namespace TaskManager.Application.Features.TaskItems.Queries.GetByUser
{
    public record GetByUserQuery(int UserId) : IQuery<GetByUserResponse>;
    public record GetByUserResponse(IEnumerable<TaskItemDTO> TaskItems);
}
