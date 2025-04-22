using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskItems.Queries.GetByLabel
{
    public record GetByLabelQuery(int UserId, int LabelId) : IRequest<GetByLabelResponse>;
    public record GetByLabelResponse(IEnumerable<TaskItemDTO> TaskItems);
}
