using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskLabels.Queries.GetAll
{
    public class GetAllTaskLabelsQuery() : IRequest<IEnumerable<TaskLabelDTO>>;
}
