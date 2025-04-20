using BuildingBlocks.Exceptions;
using MediatR;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.TaskLabels.Commands.UpdateTaskLabel
{
    public class UpdateTaskLabelCommandHandler : IRequestHandler<UpdateTaskLabelCommand, bool>
    {
        private readonly ITaskLabelRepository _taskLabelRepository;
        
        public UpdateTaskLabelCommandHandler(ITaskLabelRepository taskLabelRepository)
        {
            _taskLabelRepository = taskLabelRepository;
        }
        
        public async Task<bool> Handle(UpdateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new BadRequestException("Command cannot be null");
        
            TaskLabel? taskLabel = await _taskLabelRepository.GetById(command.Id, cancellationToken);
            if (taskLabel == null)
                throw new NotFoundException(nameof(taskLabel), command.Id);
            
            taskLabel.Update(command.Name, command.LabelColor, command.TaskItemId);

            await _taskLabelRepository.Update(taskLabel, cancellationToken);

            return true;
        }
    }   
}
