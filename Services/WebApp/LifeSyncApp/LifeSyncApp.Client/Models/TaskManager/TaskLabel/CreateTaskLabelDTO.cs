namespace LifeSyncApp.Client.Models.TaskManager.TaskLabel
{
    public record CreateTaskLabelDTO(
        string Name,
        LabelColor LabelColor,
        int UserId,
        int TaskItemId);
}
