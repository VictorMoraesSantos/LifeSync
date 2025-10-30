using Core.Application.Interfaces;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskLabelService
        : IReadService<TaskLabelDTO, int, TaskLabelFilterDTO>,
        ICreateService<CreateTaskLabelDTO>,
        IUpdateService<UpdateTaskLabelDTO>,
        IDeleteService<int>
    { }
}
