using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs
{
    public record TaskItemDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Title,
        string Description,
        Status Status,
        Priority Priority,
        DateOnly DueDate,
        int UserId,
        List<TaskLabelDTO> Labels
    );
}

