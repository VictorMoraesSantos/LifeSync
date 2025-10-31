using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using TaskManager.Application.DTOs.TaskItem;

namespace TaskManager.Application.Features.TaskItems.Queries.GetByFilter
{
    public record GetByFilterQuery(TaskItemFilterDTO Filter) : IQuery<GetByFilterResult>;
    public record GetByFilterResult(IEnumerable<TaskItemDTO> Items, PaginationData Pagination);
}
