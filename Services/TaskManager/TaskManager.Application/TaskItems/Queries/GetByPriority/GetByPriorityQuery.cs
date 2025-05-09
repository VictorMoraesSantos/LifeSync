﻿using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.TaskItems.Queries.GetByPriority
{
    public record GetByPriorityQuery(int UserId, Priority Priority) : IRequest<GetByPriorityResponse>;
    public record GetByPriorityResponse(IEnumerable<TaskItemDTO> TaskItems);
}
