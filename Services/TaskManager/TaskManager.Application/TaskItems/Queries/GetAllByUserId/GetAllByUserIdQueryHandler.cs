﻿using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Queries.GetAllByUserId
{
    public class GetAllByUserIdQueryHandler : IRequestHandler<GetAllByUserIdQuery, GetAllByUserIdResponse>
    {
        private readonly ITaskItemService _taskItemService;

        public GetAllByUserIdQueryHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<GetAllByUserIdResponse> Handle(GetAllByUserIdQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
                throw new BadRequestException(nameof(query), "Query cannot be null");

            IEnumerable<TaskItemDTO> result = await _taskItemService.GetTaskItemsByUserIdAsync(query.UserId, cancellationToken);
            GetAllByUserIdResponse response = new(result);
            return response;
        }
    }

}
