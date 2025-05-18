using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Filters
{
    public record TaskItemFilterDTO(
        int? UserId,
        string? TitleContains,
        Status? Status,
        Priority? Priority,
        DateOnly? DueDate,
        int? LabelId);
}
