using Core.Application.DTO;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskItem
{
    public record TaskItemDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Title,
        string Description,
        Status Status,
        Priority Priority,
        DateOnly DueDate,
        int UserId,
        List<TaskLabelDTO> Labels
        ) : DTOBase(Id, CreatedAt, UpdatedAt);
}

