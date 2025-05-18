using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskItem
{
    public record CreateTaskItemDTO(string Title, string Description, Priority Priority, DateOnly DueDate, int UserId);
}
