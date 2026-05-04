using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.Models.TaskManager
{
    public record UpdateTaskLabelDTO(
        string Name,
        LabelColor LabelColor);
}
