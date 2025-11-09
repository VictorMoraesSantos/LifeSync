namespace LifeSyncApp.Client.Models.TaskManager.TaskLabel
{
    public record UpdateTaskLabelDTO(
        int Id,
        string Name,
        LabelColor LabelColor);
}
