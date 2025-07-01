using BuildingBlocks.Results;
using Core.Application.Interfaces;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskLabelService
        : IReadService<TaskLabelDTO, int>,
        ICreateService<CreateTaskLabelDTO>,
        IUpdateService<UpdateTaskLabelDTO>,
        IDeleteService<int>
    {
        Task<Result<IEnumerable<TaskLabelDTO>>> GetByFilterAsync(TaskLabelFilterDTO filter, CancellationToken cancellationToken);
    }
}
