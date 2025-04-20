using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskItems.Commands.DeleteTaskItem
{
    public class DeleteTaskItemCommandHandler : IRequestHandler<DeleteTaskItemCommand, bool>
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public DeleteTaskItemCommandHandler(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<bool> Handle(DeleteTaskItemCommand command, CancellationToken cancellationToken)
        {
            TaskItem? taskItem = await _taskItemRepository.GetById(command.Id, cancellationToken);
            if (taskItem == null)
                throw new NotFoundException(nameof(TaskItem), command.Id);

            taskItem.MarkAsDeleted();

            await _taskItemRepository.Update(taskItem, cancellationToken);

            return true;
        }
    }
}
