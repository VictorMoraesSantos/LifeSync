using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mapping
{
    public static class TaskLabelMapper
    {
        public static TaskLabelDTO ToDTO(this TaskLabel taskLabel)
        {
            TaskLabelDTO taskLabelDTO = new(taskLabel.Id, taskLabel.CreatedAt, taskLabel.UpdatedAt, taskLabel.Name, taskLabel.LabelColor, taskLabel.UserId, taskLabel.TaskItemId);
            return taskLabelDTO;
        }

        public static TaskLabel ToEntity(this TaskLabelDTO taskLabelDTO)
        {
            TaskLabel taskLabel = new(taskLabelDTO.Name, taskLabelDTO.Color, taskLabelDTO.UserId, taskLabelDTO.TaskItemId);
            return taskLabel;
        }
    }
}
