using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.DTOs.TaskManager.TaskLabel
{
    public record CreateTaskLabelDTO(
        string Name,
        LabelColor LabelColor,
        int UserId);
}
