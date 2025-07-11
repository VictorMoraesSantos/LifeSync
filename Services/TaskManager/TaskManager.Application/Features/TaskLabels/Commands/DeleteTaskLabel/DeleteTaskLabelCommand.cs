﻿using BuildingBlocks.CQRS.Commands;

namespace TaskManager.Application.Features.TaskLabels.Commands.DeleteTaskLabel
{
    public record DeleteTaskLabelCommand(int Id) : ICommand<DeleteTaskLabelResult>;
    public record DeleteTaskLabelResult(bool IsDeleted);
}
