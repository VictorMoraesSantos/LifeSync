using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Filters
{
    public record TaskLabelFilterDTO(
        int? UserId,
        int? TaskItemId,
        string? NameContains,
        LabelColor? LabelColor);
}
