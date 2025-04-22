using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskItems.Commands.DeleteTaskItem
{
    public class DeleteTaskItemCommandHandler : IRequestHandler<DeleteTaskItemCommand, DeleteTaskItemResponse>
    {
        private readonly ITaskItemService _taskItemService;

        public DeleteTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<DeleteTaskItemResponse> Handle(DeleteTaskItemCommand command, CancellationToken cancellationToken)
        {

            bool result = await _taskItemService.DeleteTaskItemAsync(command.Id, cancellationToken);
            DeleteTaskItemResponse response = new(result);
            return response;
        }
    }
}
