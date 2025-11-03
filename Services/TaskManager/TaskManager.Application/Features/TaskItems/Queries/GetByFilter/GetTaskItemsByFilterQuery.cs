using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using TaskManager.Application.DTOs.TaskItem;

namespace TaskManager.Application.Features.TaskItems.Queries.GetByFilter
{
    public record GetTaskItemsByFilterQuery(TaskItemFilterDTO Filter) : IQuery<GetTaskItemsByFilterResult>;
    public record GetTaskItemsByFilterResult(IEnumerable<TaskItemDTO> Items, PaginationData Pagination);
}
