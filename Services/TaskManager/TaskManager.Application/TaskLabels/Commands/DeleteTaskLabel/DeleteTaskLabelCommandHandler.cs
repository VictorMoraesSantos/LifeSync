using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskLabels.Commands.DeleteTaskLabel
{
    public class DeleteTaskLabelCommandHandler : IRequestHandler<DeleteTaskLabelCommand, bool>
    {
        private readonly ITaskLabelRepository _taskLabelRepository;

        public DeleteTaskLabelCommandHandler(ITaskLabelRepository taskLabelRepository)
        {
            _taskLabelRepository = taskLabelRepository;
        }

        public async Task<bool> Handle(DeleteTaskLabelCommand command, CancellationToken cancellationToken)
        {
            TaskLabel? taskLabel = await _taskLabelRepository.GetById(command.Id, cancellationToken);
            if (taskLabel == null)
                throw new NotFoundException(nameof(TaskLabel), command.Id);

            taskLabel.MarkAsDeleted();

            await _taskLabelRepository.Update(taskLabel, cancellationToken);

            return true;
        }
    }
}
