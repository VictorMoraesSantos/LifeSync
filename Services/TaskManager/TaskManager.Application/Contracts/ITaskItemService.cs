using BuildingBlocks.Results;
using Core.Application.Interfaces;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.DTOs.TaskItem;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskItemService
        : IReadService<TaskItemDTO, int>,
        ICreateService<CreateTaskItemDTO>,
        IUpdateService<UpdateTaskItemDTO>,
        IDeleteService<int>
    {
        Task<Result<(IEnumerable<TaskItemDTO> Items, PaginationData Pagination)>> GetByFilterAsync(TaskItemFilterDTO filter, CancellationToken cancellationToken);
    }
}
