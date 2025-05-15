using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskItems.Queries.GetByTitle
{
    public record GetByTitleQuery(int UserId, string Title) : IRequest<GetByTitleResult>;
    public record GetByTitleResult(IEnumerable<TaskItemDTO> TaskItems);
}
