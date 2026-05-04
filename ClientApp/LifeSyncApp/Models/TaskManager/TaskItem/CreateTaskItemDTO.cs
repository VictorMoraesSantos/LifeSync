using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.Models.TaskManager
{
    public record CreateTaskItemDTO(
        string Title,
        string Description,
        Priority Priority,
        DateOnly DueDate,
        int UserId,
        List<int>? TaskLabelsId);
}
