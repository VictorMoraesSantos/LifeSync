﻿using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.TaskLabels.Commands.CreateTaskLabel
{
    public record CreateTaskLabelCommand(string Name, LabelColor LabelColor, int UserId, int TaskItemId)
        : IRequest<CreateTaskLabelResponse>;
    public record CreateTaskLabelResponse(int id);
}
