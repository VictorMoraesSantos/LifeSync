using LifeSyncApp.DTOs.TaskManager.TaskItem;
using LifeSyncApp.Models.TaskManager;

namespace LifeSyncApp.Mapping.TaskManager
{
    public static class TaskItemMapping
    {
        public static TaskItemDTO ToDTO(this TaskItem item)
        {
            var dto = new TaskItemDTO(
                item.Id,
                item.CreatedAt,
                item.UpdatedAt,
                item.Title,
                item.Description,
                item.Status,
                item.Priority,
                item.DueDate,
                item.UserId,
                item.Labels.Select(l => l.ToDTO()).ToList());
            return dto;
        }

        public static List<TaskItemDTO> ToDTOList(this IEnumerable<TaskItem> items)
        {
            var dtos = items.Select(i => i.ToDTO()).ToList();
            return dtos;
        }

        public static CreateTaskItemDTO ToCreateDTO(this TaskItem item)
        {
            var dto = new CreateTaskItemDTO(
                item.Title,
                item.Description,
                item.Priority,
                item.DueDate,
                item.UserId,
                item.Labels.Select(l => l.Id).ToList());
            return dto;
        }

        public static UpdateTaskItemDTO ToUpdateDTO(this TaskItem item)
        {
            var dto = new UpdateTaskItemDTO(
                item.Title,
                item.Description,
                item.Status,
                item.Priority,
                item.DueDate,
                item.Labels?.Select(l => l.Id).ToList());
            return dto;
        }


    }
}
