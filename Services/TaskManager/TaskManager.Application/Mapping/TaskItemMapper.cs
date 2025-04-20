using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mapping
{
    public static class TaskItemMapper
    {
        public static TaskItemDTO ToDTO(this TaskItem taskItem)
        {
            TaskItemDTO taskItemDTO = new(
                taskItem.Id,
                taskItem.CreatedAt,
                taskItem.UpdatedAt,
                taskItem.Title,
                taskItem.Description,
                taskItem.Status,
                taskItem.Priority,
                taskItem.DueDate,
                taskItem.UserId,
                taskItem.Labels.Select(l => l.ToDTO()).ToList());
            return taskItemDTO;
        }

        public static TaskItem ToEntity(this TaskItemDTO taskItemDTO)
        {
            TaskItem taskItem = new(
                taskItemDTO.Title,
                taskItemDTO.Description,
                taskItemDTO.Priority,
                taskItemDTO.DueDate,
                taskItemDTO.UserId);
            return taskItem;
        }
    }
}
