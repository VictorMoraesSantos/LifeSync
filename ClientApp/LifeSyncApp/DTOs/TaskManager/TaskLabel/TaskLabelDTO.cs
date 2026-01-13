using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.DTOs.TaskManager.TaskLabel
{
    public record TaskLabelDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        LabelColor LabelColor,
        int UserId);
}
