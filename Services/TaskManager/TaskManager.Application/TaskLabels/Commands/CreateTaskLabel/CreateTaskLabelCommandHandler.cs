using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskLabels.Commands.CreateTaskLabel
{
    public class CreateTaskLabelCommandHandler : IRequestHandler<CreateTaskLabelCommand, int>
    {
        private readonly ITaskLabelRepository _taskLabelRepository;

        public CreateTaskLabelCommandHandler(ITaskLabelRepository taskLabelRepository)
        {
            _taskLabelRepository = taskLabelRepository;
        }

        public async Task<int> Handle(CreateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new BadRequestException("Command cannot be null");

            TaskLabel taskLabel = new(command.Name, command.LabelColor, command.UserId, command.TaskItemId);
            await _taskLabelRepository.Create(taskLabel, cancellationToken);

            return taskLabel.Id;
        }
    }
}
