using Core.Application.DTO;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskLabel
{
    public record TaskLabelDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        LabelColor Color,
        int UserId,
        List<TaskItemDTO> Items
        ) : DTOBase(Id, CreatedAt, UpdatedAt);
}
