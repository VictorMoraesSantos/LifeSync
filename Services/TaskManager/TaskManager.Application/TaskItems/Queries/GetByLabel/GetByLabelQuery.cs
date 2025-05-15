using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskItems.Queries.GetByLabel
{
    public record GetByLabelQuery(int UserId, int LabelId) : IRequest<GetByLabelResult>;
    public record GetByLabelResult(IEnumerable<TaskItemDTO> TaskItems);
}
