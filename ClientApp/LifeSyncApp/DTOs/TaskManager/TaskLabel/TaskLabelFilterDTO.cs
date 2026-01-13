using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.DTOs.TaskManager.TaskLabel
{
    public record TaskLabelFilterDTO(
        int? Id,
        int? UserId,
        int? ItemId,
        string? NameContains,
        LabelColor? LabelColor,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize);
}
