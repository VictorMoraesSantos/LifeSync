using BuildingBlocks.CQRS.Queries;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.DTOs.TaskLabel;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetByFilter
{
    public record GetByFilterQuery(TaskLabelFilterDTO filter) : IQuery<GetByFilterResult>;
    public record GetByFilterResult(IEnumerable<TaskLabelDTO> TaskLabels);
}
