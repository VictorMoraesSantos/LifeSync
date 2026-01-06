using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskLabel.TaskLabel
{
    public record CreateTaskLabelDTO(
        string Name,
        LabelColor LabelColor,
        int UserId);
}
