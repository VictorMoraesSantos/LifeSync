using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.Models.TaskManager
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
        List<TaskLabelDTO> Labels);
}