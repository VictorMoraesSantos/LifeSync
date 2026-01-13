using Core.Application.DTO;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskItem
{
    public record TaskItemSimpleDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Title,
        string Description,
        Status Status,
        Priority Priority,
        DateOnly DueDate,
        int UserId)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
