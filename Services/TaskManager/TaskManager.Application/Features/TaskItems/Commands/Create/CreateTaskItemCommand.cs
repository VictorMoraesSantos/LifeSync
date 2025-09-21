﻿using BuildingBlocks.CQRS.Commands;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.TaskItems.Commands.CreateTaskItem
{
    public record CreateTaskItemCommand(
        string Title,
        string Description,
        Priority Priority,
        DateOnly DueDate,
        int UserId
    ) : ICommand<CreateTaskItemResult>;

    public record CreateTaskItemResult(int Id);
}
