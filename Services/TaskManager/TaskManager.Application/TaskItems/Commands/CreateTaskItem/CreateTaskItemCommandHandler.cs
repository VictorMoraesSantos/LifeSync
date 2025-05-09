﻿using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.TaskItems.Commands.CreateTaskItem
{
    public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, CreateTaskItemResponse>
    {
        private readonly ITaskItemService _taskItemService;

        public CreateTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<CreateTaskItemResponse> Handle(CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new BadRequestException(nameof(command), "Command cannot be null");

            int result = await _taskItemService.CreateTaskItemAsync(
                command.Title,
                command.Description,
                (int)command.Priority,
                command.DueDate,
                command.UserId,
                cancellationToken
            );

            CreateTaskItemResponse response = new(result);
            return response;
        }
    }
}
