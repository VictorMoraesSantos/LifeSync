using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.Models.TaskManager
{
    public record CreateTaskLabelDTO(
        string Name,
        LabelColor LabelColor,
        int UserId);
}
