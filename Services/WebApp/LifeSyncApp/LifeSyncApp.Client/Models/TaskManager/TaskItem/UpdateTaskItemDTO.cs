namespace LifeSyncApp.Client.Models.TaskManager.TaskItem
{
    public record UpdateTaskItemDTO(
        int Id,
        string Title,
        string Description,
        Status Status,
        Priority Priority,
        DateOnly DueDate);
}
