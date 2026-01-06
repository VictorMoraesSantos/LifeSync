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
                entity.Items.Select(i => i.ToDTO()).ToList());
            return dto;
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
