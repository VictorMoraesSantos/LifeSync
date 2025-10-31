﻿using Core.Application.Interfaces;
using TaskManager.Application.DTOs.TaskItem;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskItemService
        : IReadService<TaskItemDTO, int, TaskItemFilterDTO>,
        ICreateService<CreateTaskItemDTO>,
        IUpdateService<UpdateTaskItemDTO>,
        IDeleteService<int>
    { }
}
