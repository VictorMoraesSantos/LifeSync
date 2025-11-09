namespace LifeSyncApp.Client.Models.TaskManager.TaskLabel
{
    public record TaskLabelDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        LabelColor Color,
        int UserId,
        int TaskItemId);
}
