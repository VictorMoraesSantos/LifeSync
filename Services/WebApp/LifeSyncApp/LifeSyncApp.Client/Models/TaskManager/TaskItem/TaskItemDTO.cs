using LifeSyncApp.Client.Models.TaskManager.TaskLabel;

namespace LifeSyncApp.Client.Models.TaskManager.TaskItem
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
