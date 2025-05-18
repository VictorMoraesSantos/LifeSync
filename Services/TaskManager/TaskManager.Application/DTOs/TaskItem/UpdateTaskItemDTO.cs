using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskItem
{
    public record UpdateTaskItemDTO(int Id, string Title, string Description, Status Status, Priority Priority, DateOnly DueDate);
}
