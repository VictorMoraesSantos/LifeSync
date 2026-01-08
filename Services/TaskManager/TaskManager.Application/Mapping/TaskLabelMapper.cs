using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Application.DTOs.TaskLabel.TaskLabel;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mapping
{
    public static class TaskLabelMapper
    {
        public static TaskLabelDTO ToDTO(this TaskLabel entity)
        {
            TaskLabelDTO dto = new(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name,
                entity.LabelColor,
                entity.UserId,
                entity.Items.Select(i => i.ToSimpleDTO()).ToList()); // ✅ Usa ToSimpleDTO() para evitar ciclo
            return dto;
        }

        public static TaskLabelSimpleDTO ToSimpleDTO(this TaskLabel entity)
        {
            return new TaskLabelSimpleDTO(
                entity.Id,
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.Name,
                entity.LabelColor,
                entity.UserId);
        }

        public static TaskLabel ToEntity(this CreateTaskLabelDTO dto)
        {
            TaskLabel entity = new(
                dto.Name,
                dto.LabelColor,
                dto.UserId);
            return entity;
        }
    }
}
