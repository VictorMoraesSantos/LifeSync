using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskLabel.TaskLabel
{
    public record UpdateTaskLabelDTO(int Id, string Name, LabelColor LabelColor);
}
