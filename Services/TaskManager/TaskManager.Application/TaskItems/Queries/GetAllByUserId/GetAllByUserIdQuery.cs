﻿using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskItems.Queries.GetAllByUserId
{
    public record GetAllByUserIdQuery(int UserId) : IRequest<GetAllByUserIdResponse>;
    public record GetAllByUserIdResponse(IEnumerable<TaskItemDTO> TaskItems);
}