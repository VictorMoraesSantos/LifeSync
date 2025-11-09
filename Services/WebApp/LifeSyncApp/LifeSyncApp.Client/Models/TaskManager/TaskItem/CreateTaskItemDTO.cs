namespace LifeSyncApp.Client.Models.TaskManager.TaskItem
{
    public record CreateTaskItemDTO(
        string Title,
        string Description,
        Priority Priority,
        DateOnly DueDate,
        int UserId);
}
