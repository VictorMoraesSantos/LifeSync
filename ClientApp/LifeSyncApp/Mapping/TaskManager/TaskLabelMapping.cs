using LifeSyncApp.DTOs.TaskManager.TaskLabel;
using LifeSyncApp.Models.TaskManager;

namespace LifeSyncApp.Mapping.TaskManager
{
    public static class TaskLabelMapping
    {
        public static TaskLabelDTO ToDTO(this TaskLabel taskLabel)
        {
            var dto = new TaskLabelDTO(
                taskLabel.Id,
                taskLabel.CreatedAt,
                taskLabel.UpdatedAt,
                taskLabel.Name,
                taskLabel.Color,
                taskLabel.UserId);
            return dto;
        }

        public static List<TaskLabelDTO> ToDTOList(this IEnumerable<TaskLabel> taskLabels)
        {
            var dtos = taskLabels.Select(tl => tl.ToDTO()).ToList();
            return dtos;
        }

        public static CreateTaskLabelDTO ToCreateDTO(this TaskLabel taskLabel)
        {
            var dto = new CreateTaskLabelDTO(
                taskLabel.Name,
                taskLabel.Color,
                taskLabel.UserId);
            return dto;
        }

        public static UpdateTaskLabelDTO ToUpdateDTO(this TaskLabel taskLabel)
        {
            var dto = new UpdateTaskLabelDTO(
                taskLabel.Name,
                taskLabel.Color);
            return dto;
        }
    }
}
