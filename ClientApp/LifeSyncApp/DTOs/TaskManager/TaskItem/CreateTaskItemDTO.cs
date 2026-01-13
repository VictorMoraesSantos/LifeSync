using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.DTOs.TaskManager.TaskItem
{
    public record CreateTaskItemDTO(
        string Title,
        string Description,
        Priority Priority,
        DateOnly DueDate,
        int UserId,
        List<int>? TaskLabelsId);
}
