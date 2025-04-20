using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskLabels.Queries.GetByName
{
    public record GetByNameQuery(int UserId, string Name) : IRequest<IEnumerable<TaskLabelDTO>?>;
}
