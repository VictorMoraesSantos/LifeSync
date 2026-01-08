using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mapping
{
    public static class TaskItemMapper
    {
        public static TaskItemDTO ToDTO(this TaskItem entity)
        {
            TaskItemDTO dto = new(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Title,
                entity.Description,
                entity.Status,
                entity.Priority,
                entity.DueDate,
                entity.UserId,
                entity.Labels.Select(l => l.ToSimpleDTO()).ToList());
            return dto;
        }

        public static TaskItemSimpleDTO ToSimpleDTO(this TaskItem entity)
        {
            return new TaskItemSimpleDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Title,
                entity.Description,
                entity.Status,
                entity.Priority,
                entity.DueDate,
                entity.UserId);
        }

        public static TaskItem ToEntity(this CreateTaskItemDTO dto)
        {
            TaskItem entity = new(
                dto.Title,
                dto.Description,
                dto.Priority,
                dto.DueDate,
                dto.UserId,
                null);
            return entity;
        }
    }
}
