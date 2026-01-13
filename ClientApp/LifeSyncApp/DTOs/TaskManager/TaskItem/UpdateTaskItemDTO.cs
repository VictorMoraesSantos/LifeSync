using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.DTOs.TaskManager.TaskItem
{
    public record UpdateTaskItemDTO(
        string Title,
        string Description,
        Status Status,
        Priority Priority,
        DateOnly DueDate);
}
