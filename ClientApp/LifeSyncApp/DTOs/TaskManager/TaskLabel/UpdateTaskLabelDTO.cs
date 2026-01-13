using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.DTOs.TaskManager.TaskLabel
{
    public record UpdateTaskLabelDTO(
        string Name,
        LabelColor LabelColor);
}
