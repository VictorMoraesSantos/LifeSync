using MediatR;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.DTOs.TaskLabel;

namespace TaskManager.Application.TaskLabels.Queries.GetByFilter
{
    public record GetByFilterQuery(TaskLabelFilterDTO filter) : IRequest<GetByFilterResult>;
    public record GetByFilterResult(IEnumerable<TaskLabelDTO> TaskLabels);
}
