using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using TaskManager.Application.DTOs.TaskLabel;

namespace TaskManager.Application.Features.TaskLabels.Queries.GetByFilter
{
    public record GetTaskLabelsByFilterQuery(TaskLabelFilterDTO filter) : IQuery<GetTaskLabelsByFilterResult>;
    public record GetTaskLabelsByFilterResult(IEnumerable<TaskLabelDTO> TaskLabels, PaginationData Pagination);
}
